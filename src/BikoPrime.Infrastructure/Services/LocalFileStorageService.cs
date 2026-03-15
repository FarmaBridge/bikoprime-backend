namespace BikoPrime.Infrastructure.Services;

using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Exceptions;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly string _photoFolder = "photos";

    public LocalFileStorageService()
    {
        // Salvar fotos em: wwwroot/storage/photos/
        _basePath = Path.Combine(AppContext.BaseDirectory, "wwwroot", "storage");
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken)
    {
        try
        {
            // Criar diretório se não existir
            var photoPath = Path.Combine(_basePath, _photoFolder);
            if (!Directory.Exists(photoPath))
                Directory.CreateDirectory(photoPath);

            // Caminho completo do arquivo
            var filePath = Path.Combine(photoPath, fileName);

            // Salvar arquivo
            using (var fileWriter = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await fileStream.CopyToAsync(fileWriter, cancellationToken);
            }

            // Retornar caminho relativo (para armazenar no DB)
            return Path.Combine(_photoFolder, fileName).Replace("\\", "/");
        }
        catch (Exception ex)
        {
            throw new DomainException($"Erro ao salvar arquivo: {ex.Message}", "FILE_SAVE_ERROR");
        }
    }

    public async Task<Stream> GetFileAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            // Validar que o caminho não tenta sair do diretório de fotos (segurança)
            var fullPath = Path.Combine(_basePath, filePath);
            var fullBasePath = Path.Combine(_basePath, _photoFolder);
            
            var resolvedPath = Path.GetFullPath(fullPath);
            var resolvedBasePath = Path.GetFullPath(fullBasePath);

            if (!resolvedPath.StartsWith(resolvedBasePath))
                throw new DomainException("Acesso negado ao arquivo", "ACCESS_DENIED");

            if (!File.Exists(fullPath))
                throw new DomainException("Arquivo não encontrado", "FILE_NOT_FOUND");

            // Retornar stream do arquivo
            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            return await Task.FromResult(stream);
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DomainException($"Erro ao recuperar arquivo: {ex.Message}", "FILE_READ_ERROR");
        }
    }

    public async Task DeleteFileAsync(string filePath, CancellationToken cancellationToken)
    {
        try
        {
            // Validar segurança
            var fullPath = Path.Combine(_basePath, filePath);
            var fullBasePath = Path.Combine(_basePath, _photoFolder);
            
            var resolvedPath = Path.GetFullPath(fullPath);
            var resolvedBasePath = Path.GetFullPath(fullBasePath);

            if (!resolvedPath.StartsWith(resolvedBasePath))
                throw new DomainException("Acesso negado ao arquivo", "ACCESS_DENIED");

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            await Task.CompletedTask;
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DomainException($"Erro ao deletar arquivo: {ex.Message}", "FILE_DELETE_ERROR");
        }
    }

    public bool FileExists(string filePath)
    {
        try
        {
            var fullPath = Path.Combine(_basePath, filePath);
            return File.Exists(fullPath);
        }
        catch
        {
            return false;
        }
    }
}
