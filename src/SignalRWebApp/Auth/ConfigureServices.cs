using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SignalRWebApp.JwtAuthentications;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SignalRWebApp.Auth;

public static class ConfigureServices
{
    private const string OptionsSectionName = "jwts";

    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(OptionsSectionName);
        var options = section.BindOptions<AuthOptions>();
        services.Configure<JwtOptions>(section);

        services
            .Configure<AuthOptions>(configuration.GetRequiredSection(OptionsSectionName))
            .AddSingleton<IAuthenticator, Authenticator>()
            .AddSingleton<ITokenStorage, HttpContextTokenStorage>()
            .AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Audience = options.Audience;
                o.IncludeErrorDetails = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = options.Issuer,
                    ClockSkew = TimeSpan.Zero,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey))
                };

                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity != null)
                        {
                            // Attempt to retrieve the 'nameidentifier' claim
                            var nameIdentifierClaim = claimsIdentity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
                            if (nameIdentifierClaim != null)
                            {
                                // Set Name property to the value of the 'nameidentifier' claim
                                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, nameIdentifierClaim.Value));
                            }
                            else
                            {
                                Console.WriteLine("NameIdentifier claim not found in the token.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("ClaimsIdentity is null.");
                        }

                        return Task.CompletedTask;
                    }
                 }; 

            });

        //services.AddAuthorization(authorization =>
        //{
        //    authorization.AddPolicy("is-member", policy =>
        //    {
        //        policy.RequireRole("member");
        //    });
        //});

        services.AddScoped<ITokenExtractor, TokenExtractor>();

        return services;
    }
}
