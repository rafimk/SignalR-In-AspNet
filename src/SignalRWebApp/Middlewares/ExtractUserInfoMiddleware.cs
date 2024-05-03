using System.IdentityModel.Tokens.Jwt;

namespace SignalRWebApp.Middlewares;

public class ExtractUserInfoMiddleware
{
    private readonly RequestDelegate _next;

    public ExtractUserInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extract user information from Authorization header
        string authorizationHeader = context.Request.Headers["Authorization"];

        if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
        {
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();
            var uniqueName = GetUniqueNameFromToken(token);

            // Set the user information in the request context
            context.Items["UserUniqueName"] = uniqueName;
        }
        else
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for our hub...
            var path = context.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/notifications")))
            {
                var uniqueName = GetUniqueNameFromToken(accessToken!);

                // Set the user information in the request context
                context.Items["UserUniqueName"] = uniqueName;
            }
        }
        // Call the next middleware in the pipeline
        await _next(context);
    }


    public string GetUniqueNameFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        // Read and validate the token
        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        if (securityToken == null)
        {
            throw new ArgumentException("Invalid token.");
        }

        // Extract the claims
        var claims = securityToken.Claims;

        // Find the UniqueName claim and return its value
        var uniqueNameClaim = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName);

        if (uniqueNameClaim == null)
        {
            throw new InvalidOperationException("UniqueName claim not found in token.");
        }

        return uniqueNameClaim.Value;
    }
}