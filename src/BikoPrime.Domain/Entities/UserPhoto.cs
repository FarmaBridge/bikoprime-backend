namespace BikoPrime.Domain.Entities;

/// <summary>
/// Entity para armazenar metadata de fotos de perfil de usuários.
/// 
/// SEGURANÇA PROFISSIONAL:
/// - StorageKey: UUID único para localizar arquivo no disco (nunca expor ao cliente)
/// - FileHash: SHA256 para deduplicação (mesma foto = uma cópia guardada)
/// - Checksum: Para validação de integridade
/// - IsActive: Soft delete (permite versionamento de fotos)
/// - AccessCount/LastAccessedAt: Para analytics
/// 
/// NUNCA armazene o caminho do arquivo (FilePath) - é uma vulnerabilidade de segurança!
/// </summary>
public class UserPhoto
{
    /// <summary>
    /// PK do registro de foto
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// FK do usuário dono da foto
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Nome original do arquivo (para download)
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Tipo MIME validado (image/jpeg, image/png, image/webp)
    /// </summary>
    public string ContentType { get; set; } = "image/jpeg";

    /// <summary>
    /// Tamanho do arquivo em bytes (para auditoria e limite: máx 5MB)
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// SHA256 hash do arquivo para deduplicação
    /// Se dois usuários uploadem mesma foto, apenas uma cópia é guardada
    /// </summary>
    public string FileHash { get; set; } = string.Empty;

    /// <summary>
    /// Checksum SHA256 para validação de integridade no download
    /// </summary>
    public string Checksum { get; set; } = string.Empty;

    /// <summary>
    /// Se essa é a foto ativa do usuário (soft delete)
    /// Permite versionamento e recuperação de fotos antigas
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Timestamp UTC do upload
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Quando a foto foi deativada (soft delete)
    /// </summary>
    public DateTime? DeactivatedAt { get; set; }

    /// <summary>
    /// Motivo da deativação ("user_replaced_photo", "user_deleted", "admin_removed")
    /// </summary>
    public string? DeactivationReason { get; set; }

    /// <summary>
    /// Contador de acessos para analytics
    /// </summary>
    public long AccessCount { get; set; } = 0;

    /// <summary>
    /// Última vez que essa foto foi acessada (para cache optimization)
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }

    /// <summary>
    /// Dados binários da foto (BLOB/BYTEA) - armazenado 100% no BD
    /// Não usar arquivo no disco
    /// </summary>
    public byte[] PhotoData { get; set; } = Array.Empty<byte>();
}
