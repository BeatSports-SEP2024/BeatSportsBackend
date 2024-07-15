using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Authentication.Command.AuthGoogle;
using BeatSportsAPI.Application.Features.Authentication.Command.ResetPassword.ChangePassword;
using BeatSportsAPI.Application.Features.Authentication.Command.ResetPassword.ResetPasswordByOTP;
using BeatSportsAPI.Application.Features.Authentication.Command.ResetPassword.SendOTPToEmail;
using BeatSportsAPI.Application.Features.Authentication.Queries;
using BeatSportsAPI.Application.Models.Authentication;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers.Authentication;

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
    [SwaggerOperation("Create new customer with default wallet")]
    public async Task<IActionResult> RegisterCustomer([FromForm] RegisterCustomerModelRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpPost]
    [Route("change-password")]
    [SwaggerOperation("Change password")]
    public async Task<IActionResult> ChangePassword([FromQuery] ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpPost]
    [Route("reset-password")]
    [SwaggerOperation("Recovery password by OTP")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordByOTPCommand request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpPost]
    [Route("otp")]
    [SwaggerOperation("Send OTP to user gmail")]
    public async Task<IActionResult> SendOTP([FromForm] SendOTPToEmailRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpPost]
    [Route("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = new BeatSportsResponse();

        var refreshToken = _identityService.GetRefreshToken(request.RefreshToken); //refreshToken
        if (refreshToken == null || request.AccessToken != refreshToken.AccessToken)
        {
            response.Message = "Invalid Refresh Token";
            return BadRequest(response);
        }
        else if (refreshToken.TokenExpires < DateTime.Now || request.AccessToken != refreshToken.AccessToken)
        {
            response.Message = "Token expired.";
            return BadRequest(response);
        }
        var username = _identityService.GetUserIdFromToken(request.AccessToken);
        var re = await _identityService.SetNewRefreshTokenAsync(username);

        return Ok(re);
    }
    [HttpPost]
    [Route("register/owner")]
    [SwaggerOperation("Create new owner with default wallet")]
    public async Task<IActionResult> RegisterOwner([FromBody] RegisterOwnerModelRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    #region Login with google
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest data)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(data);
        return Ok(response);
    }

    #endregion
}
