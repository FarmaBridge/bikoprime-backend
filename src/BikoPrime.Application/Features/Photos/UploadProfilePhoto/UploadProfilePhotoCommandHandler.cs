using BikoPrime.Application.DTOs;
using BikoPrime.Application.Interfaces;
using BikoPrime.Application.Services;
using BikoPrime.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BikoPrime.Application.Features.Photos.UploadProfilePhoto;

/// <summary>
/// Handler REFATORADO: Upload de foto → BLOB no BD (SEM arquivo no disco)
/// 
/// Fluxo:
/// 1. Validar arquivo (tipo, tamanho, magic bytes)
/// 2. Ler FileStream para byte[]
/// 3. Calcular hash SHA256 (deduplicação)
/// 4. Criar entity UserPhoto com PhotoData (bytes)
/// 5. Deativar foto anterior (soft delete)
/// 6. Persistir no BD
/// 7. Retornar photoId
/// 
/// IMPORTANTE: PhotoData é armazenado 100% no BD (BYTEA/BLOB)
/// Nenhum arquivo em disco
/// </summary>
public class UploadProfilePhotoCommandHandler : IRequestHandler<UploadProfilePhotoCommand, UploadPhotoResponseDto>
{
    private readonly IUserPhotoRepository _photoRepository;
    private readonly IPhotoValidationService _validationService;
    private readonly ILogger<UploadProfilePhotoCommandHandler> _logger;

    public UploadProfilePhotoCommandHandler(
        IUserPhotoRepository photoRepository,
        IPhotoValidationService validationService,
        ILogger<UploadProfilePhotoCommandHandler> logger)
    {
        _photoRepository = photoRepository;
        _validationService = validationService;
        _logger = logger;
    }

    public async Task<UploadPhotoResponseDto> Handle(
        UploadProfilePhotoCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando upload de foto para usuário {UserId}", request.UserId);

        try
        {
            // 1. VALIDAÇÃO - Rejeita arquivo inválido ANTES de processar
            var validationResult = await _validationService.ValidatePhotoAsync(
                request.FileStream,
                request.FileName,
                request.ContentType,
                cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning(
                    "Validação falhou para usuário {UserId}: {Errors}",
                    request.UserId,
                    string.Join("; ", validationResult.Errors));

                throw new InvalidOperationException(
                    $"Foto inválida: {string.Join(", ", validationResult.Errors)}");
            }

            // 2. LER BYTES DO STREAM
            request.FileStream.Position = 0;
            byte[] photoData;

            using (var memoryStream = new MemoryStream())
            {
                await request.FileStream.CopyToAsync(memoryStream, cancellationToken);
                photoData = memoryStream.ToArray();
            }

            _logger.LogInformation(
                "Foto carregada em memória: {SizeBytes} bytes",
                photoData.Length);

            // 3. CALCULAR HASH para deduplicação
            var fileHash = await _validationService.CalculateFileHashAsync(
                new MemoryStream(photoData),
                cancellationToken);

            _logger.LogInformation("Hash SHA256 calculado: {Hash}", fileHash);

            // 4. VERIFICAR DEDUPLICAÇÃO
            var existingPhotoWithHash = await _photoRepository.GetByFileHashAsync(
                fileHash,
                cancellationToken);

            if (existingPhotoWithHash != null)
            {
                _logger.LogInformation(
                    "Foto duplicada detectada para usuário {UserId}. Reusando registro existente.",
                    request.UserId);
            }

            // 5. CALCULAR CHECKSUM
            var checksum = await _validationService.CalculateChecksumAsync(
                new MemoryStream(photoData),
                cancellationToken);

            // 6. DEATIVAR FOTO ANTERIOR (soft delete)
            await _photoRepository.DeactivateCurrentPhotoAsync(
                request.UserId,
                "user_replaced_photo",
                cancellationToken);

            // 7. CRIAR NOVO REGISTRO COM PhotoData (bytes no BD)
            var newPhoto = new UserPhoto
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                FileName = request.FileName,
                ContentType = request.ContentType,
                FileSizeBytes = photoData.Length,
                FileHash = fileHash,
                Checksum = checksum,
                PhotoData = photoData,  // ARMAZENAR BYTES NO BD
                IsActive = true,
                UploadedAt = DateTime.UtcNow
            };

            await _photoRepository.AddAsync(newPhoto, cancellationToken);
            await _photoRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Foto uploadada com sucesso para usuário {UserId}. PhotoId: {PhotoId} ({SizeBytes} bytes)",
                request.UserId,
                newPhoto.Id,
                photoData.Length);

            // 8. RETORNAR RESPONSE (nunca expor StorageKey)
            return new UploadPhotoResponseDto
            {
                PhotoId = newPhoto.Id,
                PhotoUrl = $"/api/photos/{newPhoto.Id}",
                Message = "Foto de perfil atualizada com sucesso",
                UploadedAt = newPhoto.UploadedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao fazer upload de foto para usuário {UserId}",
                request.UserId);

            throw;
        }
    }
}
