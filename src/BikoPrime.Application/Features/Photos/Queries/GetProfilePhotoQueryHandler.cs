namespace BikoPrime.Application.Features.Photos.Queries;

using MediatR;
using Microsoft.Extensions.Logging;
using BikoPrime.Application.Interfaces;

/// <summary>
/// Handler que busca foto do BD (BLOB/BYTEA)
/// 
/// Responsabilidades:
/// - Validar que photoId existe
/// - Buscar bytes do BD
/// - Incrementar AccessCount
/// - Atualizar LastAccessedAt
/// - Retornar DTO com fotografia completa
/// 
/// SEGURANÇA:
/// - Verifica se foto existe
/// - Retorna NULL se não encontrada (Controller → 404)
/// - Nunca expõe caminhos ou StorageKeys
/// </summary>
public class GetProfilePhotoQueryHandler : IRequestHandler<GetProfilePhotoQuery, GetProfilePhotoDto>
{
    private readonly IUserPhotoRepository _photoRepository;
    private readonly ILogger<GetProfilePhotoQueryHandler> _logger;

    public GetProfilePhotoQueryHandler(
        IUserPhotoRepository photoRepository,
        ILogger<GetProfilePhotoQueryHandler> logger)
    {
        _photoRepository = photoRepository;
        _logger = logger;
    }

    public async Task<GetProfilePhotoDto> Handle(
        GetProfilePhotoQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Buscando foto {PhotoId} do banco de dados", request.PhotoId);

        try
        {
            // Buscar foto do BD (inclui bytes em PhotoData)
            var photo = await _photoRepository.GetByIdAsync(request.PhotoId, cancellationToken);

            if (photo == null)
            {
                _logger.LogWarning("Foto {PhotoId} não encontrada", request.PhotoId);
                return null;
            }

            if (photo.PhotoData == null || photo.PhotoData.Length == 0)
            {
                _logger.LogError("Foto {PhotoId} não possui dados binários", request.PhotoId);
                return null;
            }

            // Incrementar AccessCount e atualizar timestamp
            photo.AccessCount++;
            photo.LastAccessedAt = DateTime.UtcNow;
            await _photoRepository.UpdateAsync(photo, cancellationToken);

            // Mapear para DTO (inclui bytes)
            var dto = new GetProfilePhotoDto
            {
                Id = photo.Id,
                UserId = photo.UserId,
                FileName = photo.FileName,
                ContentType = photo.ContentType,
                PhotoData = photo.PhotoData,
                FileSizeBytes = photo.FileSizeBytes,
                UploadedAt = photo.UploadedAt,
                AccessCount = photo.AccessCount,
                LastAccessedAt = photo.LastAccessedAt
            };

            _logger.LogInformation("Foto {PhotoId} retornada com sucesso ({SizeBytes} bytes)", 
                request.PhotoId, photo.PhotoData.Length);

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar foto {PhotoId}", request.PhotoId);
            throw;
        }
    }
}
