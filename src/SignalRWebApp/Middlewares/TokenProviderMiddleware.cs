using Microsoft.Extensions.Primitives;

namespace SignalRWebApp.Middlewares;

public class TokenProviderMiddleware
{
    private readonly RequestDelegate _next;

    public TokenProviderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string token = context.Request.Query["access_token"];
        if (StringValues.IsNullOrEmpty(token))
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var authHeaderValue = authHeader.FirstOrDefault();
                if (!StringValues.IsNullOrEmpty(authHeaderValue) && authHeaderValue.StartsWith("Bearer "))
                {
                    token = authHeaderValue.Substring("Bearer ".Length);
                }
            }
        }

        context.Items["access_token"] = token;

        await _next(context);
    }
}