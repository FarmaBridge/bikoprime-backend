namespace BikoPrime.Application.Features.Photos.UploadUserPhoto;

using MediatR;
using BikoPrime.Application.DTOs.Photo;

public class UploadUserPhotoCommand : IRequest<UserPhotoDto>
{
    public Guid UserId { get; set; }

    public byte[] FileContent { get; set; } = Array.Empty<byte>();

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public bool IsProfilePicture { get; set; } = true;
}
