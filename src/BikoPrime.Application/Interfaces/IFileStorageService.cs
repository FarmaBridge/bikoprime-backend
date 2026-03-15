namespace BikoPrime.Application.Interfaces;

public interface IFileStorageService
{
    /// <summary>
    /// Salva um arquivo no storage (disco local ou nuvem)
    /// </summary>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken);

    /// <summary>
    /// Recupera um arquivo do storage
    /// </summary>
    Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// Remove um arquivo do storage
    /// </summary>
    Task DeleteFileAsync(string filePath, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se um arquivo existe
    /// </summary>
    bool FileExists(string filePath);
}
