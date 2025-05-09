using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Movies.API.Auth;

public class ApiKeyAuthFilter : IAuthorizationFilter
{
    private readonly IConfiguration _configuration;

    public ApiKeyAuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiExtractedKey)) return;
        context.Result = new UnauthorizedObjectResult("API Key is missing");

        var apiKey = _configuration["ApiKey"]!;

        if (apiKey != apiExtractedKey)
        {
            context.Result = new UnauthorizedObjectResult("Invalid API Key");
        }
    }
}