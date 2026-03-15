using BikoPrime.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BikoPrime.Application.Extensions;

/// <summary>
/// Extensão para registrar serviços de foto na DI
/// </summary>
public static class PhotoServiceExtensions
{
    public static IServiceCollection AddPhotoServices(this IServiceCollection services)
    {
        services.AddScoped<IPhotoValidationService, PhotoValidationService>();
        return services;
    }
}
