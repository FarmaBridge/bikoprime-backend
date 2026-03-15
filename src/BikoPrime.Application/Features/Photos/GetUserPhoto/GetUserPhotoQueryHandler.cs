namespace BikoPrime.Application.Features.Photos.GetUserPhoto;

using MediatR;
using BikoPrime.Application.DTOs.Photo;
using BikoPrime.Application.Interfaces;

public class GetUserPhotoQueryHandler : IRequestHandler<GetUserPhotoQuery, UserPhotoDto?>
{
    private readonly IUserPhotoRepository _userPhotoRepository;

    public GetUserPhotoQueryHandler(IUserPhotoRepository userPhotoRepository)
    {
        _userPhotoRepository = userPhotoRepository;
    }

    public async Task<UserPhotoDto?> Handle(GetUserPhotoQuery request, CancellationToken cancellationToken)
    {
        var userPhoto = await _userPhotoRepository.GetLatestByUserIdAsync(request.UserId, cancellationToken);
        
        if (userPhoto == null)
            return null;

        return MapToDto(userPhoto);
    }

    private UserPhotoDto MapToDto(Domain.Entities.UserPhoto userPhoto)
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
