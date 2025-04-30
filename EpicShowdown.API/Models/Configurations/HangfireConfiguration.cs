using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EpicShowdown.API.Models.Configurations;

public class HangfireConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public HangfireAuthenticationConfig Authentication { get; set; } = new();

    public class HangfireAuthenticationConfig
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public static void AddHangfireConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var hangfireConfig = new HangfireConfiguration();
        configuration.GetSection("Hangfire").Bind(hangfireConfig);

        services.Configure<HangfireConfiguration>(configuration.GetSection("Hangfire"));

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(hangfireConfig.ConnectionString);
            }));

        services.AddHangfireServer();
    }
}