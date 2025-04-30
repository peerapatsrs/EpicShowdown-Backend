using EpicShowdown.API.Models.Configurations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace EpicShowdown.API.Infrastructure;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string _username;
    private readonly string _password;

    public HangfireAuthorizationFilter(IConfiguration configuration)
    {
        var hangfireConfig = new HangfireConfiguration();
        configuration.GetSection("Hangfire").Bind(hangfireConfig);

        _username = hangfireConfig.Authentication.Username;
        _password = hangfireConfig.Authentication.Password;

        if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
        {
            throw new InvalidOperationException("Hangfire authentication credentials are not configured");
        }
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