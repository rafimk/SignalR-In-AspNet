namespace SignalRWebApp.Auth;

public interface IAuthenticator
{
    JwtDto CreateToken(Guid userId, string role);
    JwtDto CreateToken(Guid userId, string role, bool isAddMember, bool isAddUser);
    JwtDto CreateToken(string uniqueName, string role, bool isAddMember, bool isAddUser);
    string GetUniqueNameFromToken(string token);
}
