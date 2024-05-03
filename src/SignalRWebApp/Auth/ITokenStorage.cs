namespace SignalRWebApp.Auth;

public interface ITokenStorage
{
    void Set(JwtDto jwt);
    JwtDto Get();
}