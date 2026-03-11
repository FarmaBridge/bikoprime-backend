namespace BikoPrime.API.Controllers.Demands;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BikoPrime.Application.Features.Demands.Commands.CreateDemand;
using BikoPrime.Application.Features.Demands.Commands.UpdateDemand;
using BikoPrime.Application.Features.Demands.Commands.DeleteDemand;
using BikoPrime.Application.Features.Demands.Queries.GetAllDemands;
using BikoPrime.Application.Features.Demands.Queries.GetDemandById;
using BikoPrime.Application.Features.Demands.Queries.GetDemandsByCategory;
using BikoPrime.Application.Features.Demands.Queries.GetDemandsByUser;
using BikoPrime.Application.Features.Demands.Queries.SearchDemands;
using BikoPrime.Application.Features.Demands.Queries.GetNearbyDemands;
using BikoPrime.Application.DTOs.Demand;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DemandsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DemandsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(DemandDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateDemand([FromBody] CreateDemandCommand command)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in claims" });

            command.UserId = userId;
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetDemandById), new { id = result.Id }, result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DemandDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllDemands()
    {
        try
        {
            var query = new GetAllDemandsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DemandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDemandById([FromRoute] string id)
    {
        try
        {
            var query = new GetDemandByIdQuery { Id = Guid.Parse(id) };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("category/{category}")]
    [ProducesResponseType(typeof(IEnumerable<DemandDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDemandsByCategory([FromRoute] string category)
    {
        try
        {
            var query = new GetDemandsByCategoryQuery { Category = category };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<DemandDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDemandsByUser([FromRoute] string userId)
    {
        try
        {
            var query = new GetDemandsByUserQuery { UserId = Guid.Parse(userId) };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<DemandDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchDemands([FromQuery] string? keyword, [FromQuery] string? category, [FromQuery] decimal? minBudget, [FromQuery] decimal? maxBudget)
    {
        try
        {
            var query = new SearchDemandsQuery 
            { 
                Keyword = keyword, 
                Category = category, 
                MinBudget = minBudget, 
                MaxBudget = maxBudget 
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("nearby")]
    [ProducesResponseType(typeof(IEnumerable<DemandDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetNearbyDemands([FromQuery] decimal latitude, [FromQuery] decimal longitude, [FromQuery] double radiusKm = 10)
    {
        try
        {
            var query = new GetNearbyDemandsQuery 
            { 
                Latitude = (double)latitude, 
                Longitude = (double)longitude, 
                RadiusKm = (decimal)radiusKm 
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(DemandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDemand([FromRoute] string id, [FromBody] UpdateDemandCommand command)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in claims" });

            command.Id = Guid.Parse(id);
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDemand([FromRoute] string id)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in claims" });

            var command = new DeleteDemandCommand { Id = Guid.Parse(id), UserId = userId };
            await _mediator.Send(command);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
