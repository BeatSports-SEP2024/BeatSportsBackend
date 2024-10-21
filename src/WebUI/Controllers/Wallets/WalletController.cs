//using BeatSportsAPI.Application.Features.Courts.CourtSportCategory.Queries;
using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Features.Wallets.Queries;
using BeatSportsAPI.Application.Features.Wallets.Queries.GetById;
using BeatSportsAPI.Application.Features.Wallets.Queries.GetMerchantAndDestination;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Wallets;

public class WalletController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public WalletController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWallets([FromQuery] GetAllWalletCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpGet]
    [Route("accountId")]
    [CustomAuthorize(RoleEnums.Customer, RoleEnums.Owner)]
    public async Task<IActionResult> GetWalletById([FromQuery] GetWalletByIdCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpGet]
    [Route("merchant-destination-customer")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<MerchantNDestinationResponse> GetMerchantDestinationCustomer()
    {
        var response = await _mediator.Send(new GetMerchantDestinationCommand());

        return response;
    }
}