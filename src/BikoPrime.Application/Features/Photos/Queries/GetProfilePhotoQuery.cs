namespace BikoPrime.Application.Features.Photos.Queries;

using MediatR;

/// <summary>
/// Query para buscar fotografia de perfil do banco de dados
/// 
/// Fluxo:
/// 1. Recebe photoId
/// 2. Handler busca no BD (bytes em BLOB/BYTEA)
/// 3. Incrementa AccessCount para analytics
/// 4. Retorna DTO com bytes + metadata
/// 
/// IMPORTANTE: PhotoData contém bytes COMPLETOS, não stream
/// Use MemoryStream(result.PhotoData) no Controller para retornar File
/// </summary>
public class GetProfilePhotoQuery : IRequest<GetProfilePhotoDto>
{
    public Guid PhotoId { get; set; }
}

/// <summary>
/// DTO de resposta com foto + metadata mínima
/// </summary>
public class GetProfilePhotoDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "image/jpeg";
    public byte[] PhotoData { get; set; } = Array.Empty<byte>();
    public long FileSizeBytes { get; set; }
    public DateTime UploadedAt { get; set; }
    public long AccessCount { get; set; }
    public DateTime? LastAccessedAt { get; set; }
}
