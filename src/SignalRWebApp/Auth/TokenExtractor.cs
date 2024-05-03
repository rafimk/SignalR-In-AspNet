namespace SignalRWebApp.Auth;

public class TokenExtractor : ITokenExtractor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TokenExtractor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string ExtractTokenValue()
    {
        string authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
        {
            return null; // Or throw an exception, depending on your requirement
        }

        // Extract the token value by removing the "Bearer " prefix
        string token = authorizationHeader.Substring("Bearer ".Length).Trim();

        return token;
    }
}