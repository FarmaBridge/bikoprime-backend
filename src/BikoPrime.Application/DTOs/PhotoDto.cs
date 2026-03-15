namespace BikoPrime.Application.DTOs;

/// <summary>
/// DTO para resposta de upload de foto de perfil
/// 
/// SEGURANÇA: Nunca retorna caminho do arquivo ou StorageKey
/// Apenas retorna photoId (que mapeia internamente para StorageKey)
/// </summary>
public class UploadPhotoResponseDto
{
    /// <summary>
    /// ID da foto (UUID) - usar para construir URL de acesso: /api/photos/{photoId}
    /// </summary>
    public Guid PhotoId { get; set; }

    /// <summary>
    /// URL pública para acessar a foto (construída no backend, nunca no cliente)
    /// Exemplo: /api/photos/550e8400-e29b-41d4-a716-446655440000
    /// </summary>
    public string PhotoUrl { get; set; } = string.Empty;

    /// <summary>
    /// Mensagem de sucesso para notificar usuário
    /// </summary>
    public string Message { get; set; } = "Foto uploadada com sucesso";

    /// <summary>
    /// Timestamp do upload
    /// </summary>
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// DTO para obter foto de um usuário
/// </summary>
public class UserPhotoDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public bool IsActive { get; set; }

    public DateTime UploadedAt { get; set; }

    public long AccessCount { get; set; }
}

/// <summary>
/// DTO para validação de foto antes do upload
/// </summary>
public class PhotoValidationResultDto
{
    public bool IsValid { get; set; }

    public List<string> Errors { get; set; } = new();

    public PhotoValidationResultDto() { }

    public PhotoValidationResultDto(bool isValid, List<string>? errors = null)
    {
        IsValid = isValid;
        Errors = errors ?? new();
    }

    public static PhotoValidationResultDto Valid() => new(true);

    public static PhotoValidationResultDto Invalid(params string[] errors) =>
        new(false, errors.ToList());
}
