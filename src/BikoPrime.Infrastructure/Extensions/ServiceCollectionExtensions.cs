namespace BikoPrime.Infrastructure.Extensions;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using BikoPrime.Application.Interfaces;
using BikoPrime.Infrastructure.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"] ?? "your-super-secret-key-min-32-chars-long-1234567890";
        var googleClientId = configuration["Google:ClientId"] ?? string.Empty;

        services.AddScoped<ITokenService>(provider => new TokenService(jwtSecret));
        services.AddScoped<IGoogleTokenValidator>(provider => new GoogleTokenValidator(googleClientId));
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"] ?? "your-super-secret-key-min-32-chars-long-1234567890";

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateIssuer = true,
                    ValidIssuer = "BikoPrime",
                    ValidateAudience = true,
                    ValidAudience = "BikoPrimeClients",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        return services;
    }
}
