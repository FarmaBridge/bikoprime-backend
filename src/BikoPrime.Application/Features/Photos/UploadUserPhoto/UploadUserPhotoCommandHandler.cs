namespace BikoPrime.Application.Features.Photos.UploadUserPhoto;

using MediatR;
using Microsoft.AspNetCore.Identity;
using BikoPrime.Application.DTOs.Photo;
using BikoPrime.Application.Interfaces;
using BikoPrime.Domain.Entities;
using BikoPrime.Domain.Exceptions;

public class UploadUserPhotoCommandHandler : IRequestHandler<UploadUserPhotoCommand, UserPhotoDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IUserPhotoRepository _userPhotoRepository;
    private readonly UserManager<User> _userManager;

    public UploadUserPhotoCommandHandler(
        IFileStorageService fileStorageService,
        IUserPhotoRepository userPhotoRepository,
        UserManager<User> userManager)
    {
        _fileStorageService = fileStorageService;
        _userPhotoRepository = userPhotoRepository;
        _userManager = userManager;
    }

    public async Task<UserPhotoDto> Handle(UploadUserPhotoCommand request, CancellationToken cancellationToken)
    {
        // Validar se usuário existe
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            throw new DomainException("Usuário não encontrado", "USER_NOT_FOUND");

        // Validar arquivo
        if (request.FileContent == null || request.FileContent.Length == 0)
            throw new DomainException("Arquivo não fornecido", "FILE_REQUIRED");

        // Validar tipo de arquivo (só imagens)
        var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowedMimeTypes.Contains(request.ContentType))
            throw new DomainException("Tipo de arquivo não permitido. Use JPEG, PNG, WebP ou GIF", "INVALID_FILE_TYPE");

        // Validar tamanho (máx 5MB)
        const long maxFileSizeBytes = 5 * 1024 * 1024; // 5MB
        if (request.FileContent.Length > maxFileSizeBytes)
            throw new DomainException("Arquivo muito grande. Máximo 5MB", "FILE_TOO_LARGE");

        // Salvar arquivo
        var fileName = $"{request.UserId}_{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";
        using var fileStream = new MemoryStream(request.FileContent);
        var filePath = await _fileStorageService.SaveFileAsync(
            fileStream,
            fileName,
            request.ContentType,
            cancellationToken);

        // Se é foto de perfil, remover fotos anteriores
        if (request.IsProfilePicture)
        {
            var previousPhotos = await _userPhotoRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            foreach (var photo in previousPhotos)
            {
                // Tentar deletar arquivo
                try
                {
                    await _fileStorageService.DeleteFileAsync(photo.FilePath, cancellationToken);
                }
                catch
                {
                    // Log but don't fail if file deletion fails
                }

                // Deletar registro
                await _userPhotoRepository.DeleteAsync(photo.Id, cancellationToken);
            }
        }

        // Criar entidade UserPhoto
        var userPhoto = new UserPhoto
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            FileName = request.FileName,
            FileExtension = Path.GetExtension(request.FileName),
            ContentType = request.ContentType,
            FileSizeBytes = request.FileContent.Length,
            FilePath = filePath,
            UploadedAt = DateTime.UtcNow,
            IsProfilePicture = request.IsProfilePicture
        };

        // Persistir
        var savedPhoto = await _userPhotoRepository.AddAsync(userPhoto, cancellationToken);
        await _userPhotoRepository.SaveChangesAsync(cancellationToken);

        // Retornar DTO
        return MapToDto(savedPhoto);
    }

    private UserPhotoDto MapToDto(UserPhoto userPhoto)
    {
        return new UserPhotoDto
        {
            Id = userPhoto.Id,
            UserId = userPhoto.UserId,
            FileName = userPhoto.FileName,
            FileExtension = userPhoto.FileExtension,
            ContentType = userPhoto.ContentType,
            FileSizeBytes = userPhoto.FileSizeBytes,
            FilePath = userPhoto.FilePath,
            UploadedAt = userPhoto.UploadedAt,
            IsProfilePicture = userPhoto.IsProfilePicture
        };
    }
}
