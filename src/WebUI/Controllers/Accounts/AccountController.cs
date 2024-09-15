using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Features.Accounts.Queries;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Accounts;

public class AccountController : ApiControllerBase
{
    private readonly IMediator _mediator;
    public AccountController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    [CustomAuthorize(RoleEnums.Admin)]
    public async Task<IActionResult> GetAllUser([FromQuery] GetAllAccountCommand request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);
        return Ok(response);
    }
    [HttpGet("account-id")]
    public async Task<IActionResult> GetAccountById([FromQuery] GetAccountByIdCommand request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);
        return Ok(response);
    }
}