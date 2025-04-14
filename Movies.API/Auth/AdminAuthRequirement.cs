using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Movies.API.Auth;

public class AdminAuthRequirement : IAuthorizationHandler, IAuthorizationRequirement
{
    private readonly string _apiKey;

    public AdminAuthRequirement(string apiKey)
    {
        _apiKey = apiKey;
    }

    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        if (context.User.HasClaim(AuthConstants.TrustedMemberClaimName, "true"))
        {
            context.Succeed(this);
            return Task.CompletedTask;
        }
        
        var httpContext = context.Resource as HttpContext;
        if (httpContext is null)
        {
            return Task.CompletedTask;
        }

        if (!httpContext.Request.Headers.TryGetValue(AuthConstants.ApiKeyHeaderName, out var apiExtractedKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (_apiKey != apiExtractedKey)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        var identity = (ClaimsIdentity)httpContext.User.Identity!;
        identity.AddClaim(new Claim("userid", Guid.NewGuid().ToString()));
        //Instead of NewGuid(), it should have the id of the user.
        context.Succeed(this);
        return Task.CompletedTask;
    }
}