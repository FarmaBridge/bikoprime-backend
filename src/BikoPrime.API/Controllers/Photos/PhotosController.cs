namespace BikoPrime.API.Controllers;

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BikoPrime.Application.Features.Photos.UploadProfilePhoto;
using BikoPrime.Application.Features.Photos.Queries;

/// <summary>
/// Controller para gerenciamento de fotos de perfil
/// 
/// PADRÃO CLEAN ARCHITECTURE:
/// - Apenas orquestração via MediatR (sem lógica)
/// - Upload: UploadProfilePhotoCommand → handler persiste bytes no BD
/// - Download: GetProfilePhotoQuery → handler busca bytes do BD
/// - Validação: Application Layer (Handler)
/// - Segurança: JWT obrigatório em todos endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PhotosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PhotosController> _logger;

    public PhotosController(
        IMediator mediator,
        ILogger<PhotosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Endpoint 1: Upload de foto de perfil
    /// 
    /// Command delega para:
    /// - Application Layer (Handler): validação, magic bytes, deduplicação
    /// - Persistence Layer (Repository): persiste bytes no BD
    /// 
    /// Response: { photoId, message, uploadedAt }
    /// </summary>
    [Authorize]
    [HttpPost("profile")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadProfilePhoto(
        [FromForm] IFormFile file,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando upload de foto de perfil");

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized(new { message = "Usuário não autenticado" });

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Arquivo não fornecido" });

            var command = new UploadProfilePhotoCommand
            {
                UserId = userIdGuid,
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType ?? "image/jpeg"
            };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Validação falhou: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload de foto");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Erro ao processar upload" });
        }
    }

    /// <summary>
    /// Endpoint 2: Baixar foto de perfil pelo photoId
    /// 
    /// Query delega para:
    /// - Application Layer (Handler): busca no BD, incrementa AccessCount
    /// - Persistence Layer (Repository): busca bytes do BD
    /// - Retorna File stream com headers apropriados
    /// </summary>
    [Authorize]
    [HttpGet("{photoId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfilePhoto(
        Guid photoId,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Acessando foto {PhotoId}", photoId);

        try
        {
            var query = new GetProfilePhotoQuery { PhotoId = photoId };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { message = "Foto não encontrada" });

            return File(
                new MemoryStream(result.PhotoData),
                result.ContentType ?? "image/jpeg",
                result.FileName,
                enableRangeProcessing: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao baixar foto {PhotoId}", photoId);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Erro ao recuperar foto" });
        }
    }

    /// <summary>
    /// Endpoint 3: Obter metadata da foto de perfil do usuário autenticado
    /// 
    /// Query: GetUserProfilePhotoQuery → retorna apenas metadata, SEM bytes
    /// </summary>
    [Authorize]
    [HttpGet("profile-metadata")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProfilePhotoMetadata(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Obtendo metadata de foto de perfil");

        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userIdGuid))
                return Unauthorized(new { message = "Usuário não autenticado" });

            var query = new GetUserProfilePhotoMetadataQuery { UserId = userIdGuid };
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound(new { message = "Usuário não possui foto de perfil" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter metadata de foto");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Erro ao recuperar metadata" });
        }
    }
}
