using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Features.Transactions.Commands.ApproveMoneyForOwner;
using BeatSportsAPI.Application.Features.Transactions.Commands.ApproveWithdrawalRequestByOwner;
using BeatSportsAPI.Application.Features.Transactions.Commands.CreateWithdrawalRequestByOwner;
using BeatSportsAPI.Application.Features.Transactions.Commands.OwnerPayFeeOverview;
using BeatSportsAPI.Application.Features.Transactions.Commands.PayFeeMonthlyForOwner;
using BeatSportsAPI.Application.Features.Transactions.Commands.RejectMoneyForOwner;
using BeatSportsAPI.Application.Features.Transactions.Commands.RejectWithdrawalRequestByOwner;
using BeatSportsAPI.Application.Features.Transactions.Commands.TransferMoneyInApp;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionByCustomerV1;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionByOwner;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactions;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionsByCustomer;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllWithdrawalRequestByOwner;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetDetailWithdrawalRequestByOwner;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetDetailWithdrawWhenAccept;
using BeatSportsAPI.Application.Features.Wallets.Queries;
using BeatSportsAPI.Domain.Enums;
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
    //[CustomAuthorize(RoleEnums.Admin)]
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
    [Route("request-withdraw-money-in-app")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<IActionResult> WithdrawalRequestMoneyInApp([FromQuery] GetAllWithdrawalRequestByOwnerCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpGet]
    [Route("request-withdraw-money-in-app-detail")]
    //[CustomAuthorize(RoleEnums.Admin)]
    public async Task<IActionResult> WithdrawalRequestMoneyInAppDetail([FromQuery] GetDetailWithdrawalRequestByOwnerCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpGet]
    [Route("all-transaction-by-owner")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<IActionResult> GetAllTransactionByOwner([FromQuery] GetAllTransactionByOwner request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }

    [HttpGet]
    [Route("all-transaction-by-customer")]
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<IActionResult> GetAllTransactionByCustomer([FromQuery] GetAllTransactionByCustomerV1Query request)
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
    //[CustomAuthorize(RoleEnums.Admin, RoleEnums.Owner)]
    [CustomAuthorize(RoleEnums.Owner)]
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
    [CustomAuthorize(RoleEnums.Owner)]
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

    //[HttpPost]
    //[Route("approve-money-for-owner")]
    //public async Task<IActionResult> ApproveMoneyForOwner([FromBody] ApproveMoneyForOwnerCommand request)
    //{
    //    if (!ModelState.IsValid)
    //    {
    //        return BadRequest(ModelState);
    //    }

    //    var response = await _mediator.Send(request);

    //    if (response.Status == 400)
    //    {
    //        return BadRequest(response);
    //    }

    //    return Ok(response);
    //}

    [HttpPost]
    [Route("withdraw-money-in-app")]
    [CustomAuthorize(RoleEnums.Owner)]
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

    [HttpPost]
    [Route("approve-withdrawal-money-for-owner")]
    //[CustomAuthorize(RoleEnums.Admin)]
    public async Task<IActionResult> ApproveWithdrawalMoneyForOwner([FromBody] ApproveWithdrawalRequestByOwnerCommand request)
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
    [Route("reject-withdrawal-money-for-owner")]
    //[CustomAuthorize(RoleEnums.Admin)]
    public async Task<IActionResult> RejectWithdrawalMoneyForOwner([FromBody] RejectWithdrawalRequestByOwnerCommand request)
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
    [Route("pay-monthly-fee")]
    [CustomAuthorize(RoleEnums.Owner)]
    public async Task<IActionResult> PayMonthlyFee([FromBody] PayFeeMonthlyForOwnerCommand request)
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

    [HttpGet]
    [Route("monthly-fee-overview")]
    [CustomAuthorize(RoleEnums.Owner)]
    //[CustomAuthorize(RoleEnums.Owner, RoleEnums.Admin)]
    public async Task<IActionResult> MonthlyFeeOverview([FromQuery] OwnerPayFeeOverviewCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
    [HttpGet]
    [Route("transaction-withdraw-detail")]
    public async Task<IActionResult> GetTransactionWithdrawWhenAccept([FromQuery] GetDetailWithdrawWhenAcceptCommand request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var response = await _mediator.Send(request);

        return Ok(response);
    }
}