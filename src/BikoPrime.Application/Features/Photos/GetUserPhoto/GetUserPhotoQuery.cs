namespace BikoPrime.Application.Features.Photos.GetUserPhoto;

using MediatR;
using BikoPrime.Application.DTOs.Photo;

public class GetUserPhotoQuery : IRequest<UserPhotoDto?>
{
    public Guid UserId { get; set; }
}
