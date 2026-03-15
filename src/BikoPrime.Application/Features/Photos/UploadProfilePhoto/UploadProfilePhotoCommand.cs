using BikoPrime.Application.DTOs;
using MediatR;

namespace BikoPrime.Application.Features.Photos.UploadProfilePhoto;

/// <summary>
/// Command para upload de foto de perfil do usuário
/// 
/// SECURITY: 
/// - Arquivo é validado antes do processamento
/// - StorageKey é gerado no backend (não aceitamos do cliente)
/// - FilePath nunca é exposto
/// - Foto anterior é deativada (soft delete, não deletada)
/// - Deduplicação via FileHash
/// </summary>
public class UploadProfilePhotoCommand : IRequest<UploadPhotoResponseDto>
{
    /// <summary>
    /// Stream do arquivo (contém os bytes da imagem)
    /// </summary>
    public Stream FileStream { get; init; } = null!;

    /// <summary>
    /// Nome do arquivo original (para auditoria)
    /// </summary>
    public string FileName { get; init; } = string.Empty;

    /// <summary>
    /// Content-Type enviado pelo cliente (será validado no Handler)
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// ID do usuário uploadando a foto (vem do JWT/Auth)
    /// </summary>
    public Guid UserId { get; init; }
}
