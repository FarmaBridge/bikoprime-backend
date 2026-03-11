namespace BikoPrime.API.Controllers.Auth;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BikoPrime.Application.DTOs.Auth;
using BikoPrime.Application.Features.Auth.Register;
using BikoPrime.Application.Features.Auth.Login;
using BikoPrime.Application.Features.Auth.GoogleAuth;
using BikoPrime.Application.Features.Auth.Logout;
using BikoPrime.Application.Features.Auth.RefreshToken;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        var command = new RegisterCommand
        {
            Name = request.Name,
            UserName = request.UserName,
            Email = request.Email,
            Phone = request.Phone,
            Password = request.Password,
            AvatarUrl = request.AvatarUrl,
            Location = request.Location,
            Bio = request.Bio
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("google")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> GoogleAuth([FromBody] GoogleAuthRequestDto request)
    {
        var command = new GoogleAuthCommand
        {
            IdToken = request.IdToken
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized();
        }

        var command = new LogoutCommand
        {
            UserId = userId,
            RefreshToken = request.RefreshToken
        };

        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var query = new RefreshTokenQuery
        {
            RefreshToken = request.RefreshToken
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
