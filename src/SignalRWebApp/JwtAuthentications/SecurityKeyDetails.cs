using Microsoft.IdentityModel.Tokens;

namespace SignalRWebApp.JwtAuthentications;

internal sealed record SecurityKeyDetails(SecurityKey Key, string Algorithm);