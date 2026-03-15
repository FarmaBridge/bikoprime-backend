namespace BikoPrime.API.Controllers;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BikoPrime.Application.Features.Photos.UploadUserPhoto;
using BikoPrime.Application.Features.Photos.GetUserPhoto;
using BikoPrime.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class PhotosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;

    public PhotosController(IMediator mediator, IFileStorageService fileStorageService)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
    }

    /// <summary>
    /// Upload de foto do usuário
    /// </summary>
    [Authorize]
    [HttpPost("upload")]
    public async Task<IActionResult> UploadPhoto(
        IFormFile file,
        [FromQuery] bool isProfilePicture = true)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Usuário não autenticado");

        if (!Guid.TryParse(userId, out var userIdGuid))
            return BadRequest("ID de usuário inválido");

        // Converter IFormFile para byte[]
        byte[] fileContent;
        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            fileContent = memoryStream.ToArray();
        }

        var command = new UploadUserPhotoCommand
        {
            UserId = userIdGuid,
            FileContent = fileContent,
            FileName = file.FileName,
            ContentType = file.ContentType,
            IsProfilePicture = isProfilePicture
        };

        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { message = "Foto enviada com sucesso", photo = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obter foto de perfil do usuário
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserPhoto(Guid userId)
    {
        try
        {
            var query = new GetUserPhotoQuery { UserId = userId };
            var photo = await _mediator.Send(query);

            if (photo == null)
                return NotFound("Foto não encontrada");

            return Ok(new { photo });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Baixar arquivo da foto
    /// </summary>
    [HttpGet("download/{filePath}")]
    public async Task<IActionResult> DownloadPhoto(string filePath)
    {
        try
        {
            // Validar que filePath é seguro
            if (string.IsNullOrEmpty(filePath) || filePath.Contains(".."))
                return BadRequest("Caminho de arquivo inválido");

            var fileStream = await _fileStorageService.GetFileAsync(filePath, CancellationToken.None);
            
            // Determinar content type baseado na extensão
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return File(fileStream, contentType);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
