using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactions;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionsByCustomer;
using BeatSportsAPI.Application.Features.Wallets.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.Transactions;

public class TransactionController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTransactions([FromQuery] GetAllTransactionsCommand request)
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
    [HttpGet]
    public async Task<IActionResult> GetAllTransactionsById([FromQuery] GetAllTransactionByAccountCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}
