using BeatSportsAPI.Application.Features.Transactions.Commands.ApproveMoneyForOwner;
using BeatSportsAPI.Application.Features.Transactions.Commands.CreateWithdrawalRequestByOwner;
using BeatSportsAPI.Application.Features.Transactions.Commands.RejectMoneyForOwner;
using BeatSportsAPI.Application.Features.Transactions.Commands.TransferMoneyInApp;
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
    public async Task<IActionResult> GetAllTransactionsById([FromQuery] GetAllTransactionByAccountCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpPost]
    [Route("transfer-money-in-app")]
    public async Task<IActionResult> TransferMoneyInApp([FromBody] TransferMoneyInAppCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _mediator.Send(request);

        if (response.Status == 400)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost]
    [Route("approve-money-for-owner")]
    public async Task<IActionResult> ApproveMoneyForOwner([FromBody] ApproveMoneyForOwnerCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _mediator.Send(request);

        if (response.Status == 400)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost]
    [Route("withdraw-money-in-app")]
    public async Task<IActionResult> WithdrawMoneyInApp([FromBody] CreateWithdrawalRequestByOwnerCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await _mediator.Send(request);

        if (response.Status == 400)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

}
