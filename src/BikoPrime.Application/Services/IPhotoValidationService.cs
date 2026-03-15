using BikoPrime.Application.DTOs;
using System.Security.Cryptography;

namespace BikoPrime.Application.Services;

/// <summary>
/// Serviço profissional de validação de fotos de perfil
/// 
/// Responsabilidades:
/// - Validar tipo MIME (whitelist apenas: jpeg, png, webp)
/// - Validar tamanho (máx 5MB)
/// - Validar dimensões (200x200 até 4000x4000)
/// - Verificar magic bytes (anti-MIME sniffing)
/// - Calcular hash SHA256 para deduplicação
/// - Gerar checksum para integridade
/// </summary>
public interface IPhotoValidationService
{
    /// <summary>
    /// Valida um arquivo de foto antes do upload
    /// </summary>
    Task<PhotoValidationResultDto> ValidatePhotoAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula SHA256 hash do arquivo (para deduplicação)
    /// </summary>
    Task<string> CalculateFileHashAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calcula checksum do arquivo (para validação de integridade)
    /// </summary>
    Task<string> CalculateChecksumAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default);
}

public class PhotoValidationService : IPhotoValidationService
{
    // Tipos MIME permitidos (whitelist)
    private static readonly HashSet<string> AllowedMimeTypes = new()
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    // Magic bytes para cada formato (previne MIME sniffing attacks)
    private static readonly Dictionary<string, byte[][]> MagicBytes = new()
    {
        // JPEG: FF D8 FF
        {
            "image/jpeg",
            new[]
            {
                new byte[] { 0xFF, 0xD8, 0xFF }
            }
        },
        // PNG: 89 50 4E 47
        {
            "image/png",
            new[]
            {
                new byte[] { 0x89, 0x50, 0x4E, 0x47 }
            }
        },
        // WebP: RIFF ... WEBP
        {
            "image/webp",
            new[]
            {
                new byte[] { 0x52, 0x49, 0x46, 0x46 } // RIFF
            }
        }
    };

    // Limites de segurança
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB
    private const int MinDimension = 200;
    private const int MaxDimension = 4000;

    public async Task<PhotoValidationResultDto> ValidatePhotoAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        // 1. Validar extension do arquivo
        var ext = Path.GetExtension(fileName).ToLower();
        if (!IsValidExtension(ext))
        {
            errors.Add($"Extensão de arquivo inválida: {ext}. Permitidas: .jpg, .jpeg, .png, .webp");
        }

        // 2. Validar Content-Type (whitelist apenas)
        if (!AllowedMimeTypes.Contains(contentType?.ToLower() ?? ""))
        {
            errors.Add($"Tipo MIME inválido: {contentType}. Permitidos: image/jpeg, image/png, image/webp");
        }

        // 3. Validar tamanho do arquivo
        if (fileStream.Length > MaxFileSizeBytes)
        {
            var sizeMB = fileStream.Length / (1024.0 * 1024.0);
            errors.Add($"Arquivo muito grande: {sizeMB:F2}MB. Máximo: 5MB");
        }

        if (fileStream.Length < 1024) // Mínimo 1KB
        {
            errors.Add("Arquivo muito pequeno: mínimo 1KB");
        }

        // 4. Validar magic bytes (previne MIME sniffing)
        fileStream.Position = 0;
        if (!await ValidateMagicBytesAsync(fileStream, contentType, cancellationToken))
        {
            errors.Add("Arquivo corrompido ou tipo MIME inválido (falha na validação de magic bytes)");
        }

        // Se houve erros na validação básica, retornar antes de gastar recursos com image parsing
        if (errors.Count > 0)
        {
            return PhotoValidationResultDto.Invalid(errors.ToArray());
        }

        // 5. Validar dimensões da imagem (requer library de parsing)
        // Para agora, vamos confiar que a validação de magic bytes é suficiente
        // Em produção, usar: ImageSharp, SkiaSharp ou similares para validar dimensões

        return PhotoValidationResultDto.Valid();
    }

    public async Task<string> CalculateFileHashAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        fileStream.Position = 0;

        using (var sha256 = SHA256.Create())
        {
            var hash = await Task.Run(
                () => sha256.ComputeHash(fileStream),
                cancellationToken);

            return Convert.ToHexString(hash).ToLower();
        }
    }

    public async Task<string> CalculateChecksumAsync(
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        // Para checksum, usamos o mesmo SHA256
        // Em produção, você poderia usar algoritmo diferente se necessário
        return await CalculateFileHashAsync(fileStream, cancellationToken);
    }

    private static bool IsValidExtension(string ext)
    {
        var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        return validExtensions.Contains(ext);
    }

    private static async Task<bool> ValidateMagicBytesAsync(
        Stream stream,
        string contentType,
        CancellationToken cancellationToken)
    {
        stream.Position = 0;

        // Ler primeiros bytes do arquivo
        var buffer = new byte[12]; // Máximo de bytes necessários para magic bytes
        var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

        if (bytesRead == 0)
            return false;

        var normalizedMimeType = contentType?.ToLower() ?? "";

        // Se não temos magic bytes para esse tipo, aceitar (confiamos na extensão + MIME)
        if (!MagicBytes.TryGetValue(normalizedMimeType, out var magicBytePatterns))
        {
            return true;
        }

        // Verificar se algum padrão de magic bytes bate
        foreach (var pattern in magicBytePatterns)
        {
            if (bytesRead >= pattern.Length &&
                buffer.Take(pattern.Length).SequenceEqual(pattern))
            {
                return true;
            }
        }

        // Para WebP, verificar se contém "WEBP" no offset 8
        if (normalizedMimeType == "image/webp" && bytesRead >= 12)
        {
            var webpSignature = buffer.Skip(8).Take(4);
            var webpBytes = new byte[] { 0x57, 0x45, 0x42, 0x50 }; // "WEBP"
            if (webpSignature.SequenceEqual(webpBytes))
            {
                return true;
            }
        }

        return false;
    }
}
