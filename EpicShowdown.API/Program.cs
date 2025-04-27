using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using EpicShowdown.API.Data;
using Swashbuckle.AspNetCore.Swagger;
using Hangfire.Dashboard;
using Hangfire.AspNetCore;
using System.Net.Http.Headers;
using EpicShowdown.API.Infrastructure;
using EpicShowdown.API.Models;
using EpicShowdown.API.Repositories;
using EpicShowdown.API.Services;
using AutoMapper;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Fido2NetLib;

var builder = WebApplication.CreateBuilder(args);

// Configure DateTime to use UTC
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // กำหนดให้ JSON Serializer ใช้ UTC DateTime เป็นค่าเริ่มต้น
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Configure Swagger with DateTime format
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EpicShowdown API", Version = "v1" });

    // กำหนด format สำหรับ DateTime ใน Swagger
    c.MapType<DateTime>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date-time",
        Description = "UTC DateTime"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userService = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var userCode = context.Principal?.FindFirst("UserCode")?.Value;

                if (string.IsNullOrEmpty(userCode) || !Guid.TryParse(userCode, out Guid userGuid))
                {
                    context.Fail("Invalid token");
                    return;
                }

                var user = await userService.GetByUserCodeAsync(userGuid);
                if (user == null)
                {
                    context.Fail("User not found");
                    return;
                }

                var identity = context.Principal?.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    // เพิ่ม Role เข้าไปใน Claims
                    var roleClaim = identity.FindFirst("UserRole");
                    if (roleClaim != null)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                    }
                }
            }
        };
    });

// Configure Database with UTC DateTime
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured")
    );
});

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<IPassKeyRepository, PassKeyRepository>();

// Register Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddHttpContextAccessor();

// Register Infrastructure Services
var redisConnection = builder.Configuration.GetConnectionString("RedisConnection") ?? throw new InvalidOperationException("RedisConnection is not configured");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(redisConnection);
    config.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(config);
});

// Configure SignalR
builder.Services.AddSignalR();

// Configure Hangfire
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options =>
    {
        var hangfireConnection = builder.Configuration.GetSection("Hangfire:ConnectionString").Value ?? throw new InvalidOperationException("Hangfire ConnectionString is not configured");
        options.UseNpgsqlConnection(hangfireConnection);
    }));

builder.Services.AddHangfireServer();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddScoped<IPassKeyService, PassKeyService>();
builder.Services.AddFido2(options =>
{
    options.ServerDomain = builder.Configuration["Fido2:ServerDomain"] ?? "localhost";
    options.ServerName = builder.Configuration["Fido2:ServerName"] ?? "EpicShowdown";
    var originsConfig = builder.Configuration.GetSection("Fido2:Origins").Get<string[]>();
    options.Origins = new HashSet<string>(originsConfig ?? new string[] { "https://localhost:8000", "https://localhost:3000" });
    options.TimestampDriftTolerance = Convert.ToInt32(builder.Configuration["Fido2:TimestampDriftTolerance"] ?? "300000");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://localhost:3000")
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// กำหนดให้ใช้ UTC เป็นค่าเริ่มต้นสำหรับ DateTime ในแอปพลิเคชัน
AppContext.SetSwitch("System.Globalization.UseUtcDateTime", true);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configure Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter(app.Configuration) }
});
app.UseCors("AllowFrontend");

app.Run();

// Add Hangfire Authorization Filter
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string _username;
    private readonly string _password;

    public HangfireAuthorizationFilter(IConfiguration configuration)
    {
        var authSection = configuration.GetSection("Hangfire:Authentication");
        _username = authSection.GetValue<string>("Username") ??
            throw new InvalidOperationException("Hangfire Username is not configured");
        _password = authSection.GetValue<string>("Password") ??
            throw new InvalidOperationException("Hangfire Password is not configured");
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        string authHeader = httpContext.Request.Headers["Authorization"].ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Basic "))
        {
            SetUnauthorizedResponse(httpContext);
            return false;
        }

        var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
        var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
        var parts = credentials.Split(':');
        if (parts.Length != 2)
        {
            SetUnauthorizedResponse(httpContext);
            return false;
        }

        var username = parts[0];
        var password = parts[1];

        if (username != _username || password != _password)
        {
            SetUnauthorizedResponse(httpContext);
            return false;
        }

        return true;
    }

    private void SetUnauthorizedResponse(HttpContext httpContext)
    {
        httpContext.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Hangfire Dashboard\"";
        httpContext.Response.StatusCode = 401;
    }
}
