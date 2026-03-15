namespace BikoPrime.Application.Features.Photos.Queries;

using MediatR;
using Microsoft.Extensions.Logging;
using BikoPrime.Application.Interfaces;

/// <summary>
/// Handler que busca APENA metadata da foto (sem PhotoData)
/// 
/// Responsabilidades:
/// - Buscar foto ativa do usuário
/// - Retornar APENAS metadata (sans bytes)
/// - Otimizar para não carregar dados binários
/// </summary>
public class GetUserProfilePhotoMetadataQueryHandler : 
    IRequestHandler<GetUserProfilePhotoMetadataQuery, UserProfilePhotoMetadataDto>
{
    private readonly IUserPhotoRepository _photoRepository;
    private readonly ILogger<GetUserProfilePhotoMetadataQueryHandler> _logger;

    public GetUserProfilePhotoMetadataQueryHandler(
        IUserPhotoRepository photoRepository,
        ILogger<GetUserProfilePhotoMetadataQueryHandler> logger)
    {
        _photoRepository = photoRepository;
        _logger = logger;
    }

    public async Task<UserProfilePhotoMetadataDto> Handle(
        GetUserProfilePhotoMetadataQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando metadata de foto do usuário {UserId}", request.UserId);

        try
        {
            var photo = await _photoRepository.GetLatestByUserIdAsync(request.UserId, cancellationToken);

            if (photo == null)
            {
                _logger.LogInformation("Usuário {UserId} não possui foto de perfil ativa", request.UserId);
                return null;
            }

            var dto = new UserProfilePhotoMetadataDto
            {
                Id = photo.Id,
                FileName = photo.FileName,
                ContentType = photo.ContentType,
                FileSizeBytes = photo.FileSizeBytes,
                UploadedAt = photo.UploadedAt,
                AccessCount = photo.AccessCount
            };

            _logger.LogInformation("Metadata de foto do usuário {UserId} retornada", request.UserId);
            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar metadata de foto do usuário {UserId}", request.UserId);
            throw;
        }
    }
}
