namespace BikoPrime.Application.Features.Photos.Queries;

using MediatR;

/// <summary>
/// Query para obter METADATA da foto de perfil (sem os bytes!)
/// 
/// Usado para:
/// - Verificar se usuário tem foto
/// - Obter photoId para usar em GET /api/photos/{photoId}
/// - Obter metadata basics (tamanho, tipo, data upload)
/// 
/// IMPORTANTE: NÃO retorna PhotoData (bytes)
/// Isso evita carregar fotos grandes quando só quer saber se existe
/// </summary>
public class GetUserProfilePhotoMetadataQuery : IRequest<UserProfilePhotoMetadataDto>
{
    public Guid UserId { get; set; }
}

/// <summary>
/// DTO de metadata APENAS (sem PhotoData)
/// </summary>
public class UserProfilePhotoMetadataDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "image/jpeg";
    public long FileSizeBytes { get; set; }
    public DateTime UploadedAt { get; set; }
    public long AccessCount { get; set; }
}
