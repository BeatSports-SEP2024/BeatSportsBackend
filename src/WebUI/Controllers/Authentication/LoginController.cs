using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Authentication;

[ApiController]
public class LoginController : ApiControllerBase
{
    private readonly IIdentityService _identityService;

    public LoginController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginModelRequest request)
    {
        if (User.Identity!.IsAuthenticated)
        {
            return BadRequest("Already Login");
        }
        var response = await _identityService.AuthenticateAsync(request);
        return Ok(response);
    }
}
