namespace SignalRWebApp.Services;

public interface IJwtTokenService
{
    string GenerateJwtToken(string mobileNumber);
}