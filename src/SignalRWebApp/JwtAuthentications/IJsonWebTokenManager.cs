﻿namespace SignalRWebApp.JwtAuthentications;

public interface IJsonWebTokenManager
{
    JsonWebToken CreateToken(string userId, string? email = null, string? role = null,
        IDictionary<string, IEnumerable<string>>? claims = null);

    string? ParseUniqueNameFromToken(string token);
}