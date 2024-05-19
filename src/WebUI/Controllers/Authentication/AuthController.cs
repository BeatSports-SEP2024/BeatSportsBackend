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
    [Route("register/customer")]
    public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerModelRequest request, CancellationToken cancellationToken)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);
        
        return Ok(response);
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken(string username, string accessToken, string token)
    {
        var response = new BeatSportsResponse();
        
        var refreshToken = _identityService.GetRefreshToken(token);
        if (refreshToken == null || accessToken != refreshToken.AccessToken)
        {
            response.Message = "Invalid Refresh Token";
            return Ok(response);
        }else if(refreshToken.TokenExpires < DateTime.Now || accessToken != refreshToken.AccessToken)
        {
            response.Message = "Token expired.";
            return Ok(response);
        }

        var re = await _identityService.SetNewRefreshTokenAsync(username);
        
        return Ok(re);
    }
    
	[HttpPost]
    [Route("register/owner")]
    public async Task<IActionResult> RegisterOwner([FromBody] RegisterOwnerModelRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}
