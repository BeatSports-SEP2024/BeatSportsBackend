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

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    [HttpPost]
    [Route("login")]
    public async Task<BeatSportsResponse> Login([FromBody] LoginModelRequest request)
    {
        var response = await _identityService.AuthenticateAsync(request);
        return new BeatSportsResponse
        {
            Message = "Login Successfully",
        };
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
