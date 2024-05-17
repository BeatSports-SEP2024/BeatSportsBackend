using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Authentication;

[ApiController]
public class AuthController : ApiControllerBase
{
    private readonly IIdentityService _identityService;
    private readonly IMediator _mediator;

    public AuthController(IIdentityService identityService, IMediator mediator)
    {
        _identityService = identityService;
        _mediator = mediator;
    }
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModelRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpPost]
    [Route("register")]
    public async Task<BeatSportsResponse> Register([FromBody] RegisterModelRequest request, CancellationToken cancellationToken)
    {
        var response = await _identityService.RegisterAccountAsync(request, cancellationToken);
        return new BeatSportsResponse
        {
            Message = "Create new user successfully"
        };
    }
}
