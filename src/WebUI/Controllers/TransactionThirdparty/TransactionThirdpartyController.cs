using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.TransactionThridparty.Queries.GetAllTransactionForAdmin;
using BeatSportsAPI.Application.Features.TransactionThridparty.Queries.GetTransactionByCusId;
using BeatSportsAPI.Application.Features.Wallets.Queries.GetById;
using BeatSportsAPI.Application.Features.Wallets.Queries.GetMerchantAndDestination;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.TransactionThirdparty;
[ApiController]
public class TransactionThirdpartyController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public TransactionThirdpartyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route("customerId")]
    public async Task<IActionResult> GetTransactionThridpartyById([FromQuery] GetTransactionByCusIdCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpGet]
    public async Task<TransactionThirdpartyForAdminResponse> GetTransactionsThridpartyByAdmin()
    {
        var response = await _mediator.Send(new GetTransactionsForAdminCommand());
        return response;
    }
}
