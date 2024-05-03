using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SignalRWebApp.Services;

namespace SignalRWebApp.JwtAuthentications;

public static class ConfigureServices
{
    public static IServiceCollection AddJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("jwts");
        var jwtOptions = section.BindOptions<JwtOptions>();
        services.Configure<JwtOptions>(section);
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                // Configure JWT bearer authentication
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }


    public static T BindOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
            => BindOptions<T>(configuration.GetSection(sectionName));

    public static T BindOptions<T>(this IConfigurationSection? section) where T : new()
    {
        var options = new T();
        section?.Bind(options);
        return options;
    }
}