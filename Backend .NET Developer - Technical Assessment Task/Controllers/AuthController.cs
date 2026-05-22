using Application.Common;
using Application.Features.Auth.Commands;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend_.NET_Developer___Technical_Assessment_Task.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Register a new user account.</summary>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<AuthResult>>> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Authenticate and receive a JWT token.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResult>>> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return result.Success ? Ok(result) : Unauthorized(result);
    }
}
