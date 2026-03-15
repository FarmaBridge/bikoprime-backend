using BikoPrime.Application.DTOs;
using BikoPrime.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BikoPrime.Application.Features.Photos.GetUserProfilePhoto;

/// <summary>
/// Query para obter foto ativa do usuário
/// </summary>
public class GetUserProfilePhotoQuery : IRequest<UserPhotoDto?>
{
    public Guid UserId { get; init; }
}

/// <summary>
/// Handler para obter foto de perfil do usuário
/// 
/// SEGURANÇA:
/// - Apenas retorna metadata + photoId
/// - NUNCA retorna StorageKey ou FilePath
/// - A URL real é construída no endpoint GET /api/photos/{photoId}
/// </summary>
public class GetUserProfilePhotoQueryHandler : IRequestHandler<GetUserProfilePhotoQuery, UserPhotoDto?>
{
    private readonly IUserPhotoRepository _photoRepository;
    private readonly ILogger<GetUserProfilePhotoQueryHandler> _logger;

    public GetUserProfilePhotoQueryHandler(
        IUserPhotoRepository photoRepository,
        ILogger<GetUserProfilePhotoQueryHandler> logger)
    {
        _photoRepository = photoRepository;
        _logger = logger;
    }

    public async Task<UserPhotoDto?> Handle(
        GetUserProfilePhotoQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo foto de perfil para usuário {UserId}", request.UserId);

        var photo = await _photoRepository.GetLatestByUserIdAsync(
            request.UserId,
            cancellationToken);

        if (photo == null)
        {
            _logger.LogInformation("Nenhuma foto ativa encontrada para usuário {UserId}", request.UserId);
            return null;
        }

        var dto = new UserPhotoDto
        {
            Id = photo.Id,
            UserId = photo.UserId,
            FileName = photo.FileName,
            ContentType = photo.ContentType,
            FileSizeBytes = photo.FileSizeBytes,
            IsActive = photo.IsActive,
            UploadedAt = photo.UploadedAt,
            AccessCount = photo.AccessCount
        };

        _logger.LogInformation(
            "Foto de perfil obtida com sucesso. PhotoId: {PhotoId}, Size: {Size} bytes",
            dto.Id,
            dto.FileSizeBytes);

        return dto;
    }
}

