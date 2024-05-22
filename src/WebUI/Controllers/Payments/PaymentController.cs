using System.Net;
using BeatSportsAPI.Application.Common.Base;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.Wallets.Commands.CreateDeposits;
using BeatSportsAPI.Application.Features.Wallets.Commands.CreateWithdrawls;
using BeatSportsAPI.Application.Features.Wallets.Commands.ProcessMomoPaymentReturnCommand;
using BeatSportsAPI.Application.Features.Wallets.Commands.ProcessVnpayPaymentReturnCommand;
using BeatSportsAPI.Application.Features.Wallets.Dtos;
using Duende.IdentityServer.Extensions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.Momo.Request;
using Services.VnPay.Config;
using Services.VnPay.Response;
using Utils.Extensions;

namespace WebAPI.Controllers.Payments;
[Route("api/v1/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IMediator mediator;
    private readonly VnpayConfig vnpayConfig;
    private IBeatSportsDbContext _dbContext;

    public PaymentController(IMediator mediator,
            IOptions<VnpayConfig> vnpayConfigOptions,
            IBeatSportsDbContext dbContext
        )
    {
        this.mediator = mediator;
        this.vnpayConfig = vnpayConfigOptions.Value;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Create payment to get link
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(typeof(BaseResultWithData<PaymentLinkDtos>), 200)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<PaymentLinkDtos> Create([FromBody] PaymentByThirdWalletCommand request)
    {
        var response = await mediator.Send(request);
        return response;
    }

    [HttpGet]
    [Route("vnpay-return")]
    public async Task<IActionResult> VnpayReturn([FromQuery] VnpayPayResponse response)
    {
        string returnUrl = string.Empty;
        var returnModel = new PaymentReturnDtos();
        var processRequest = response.Adapt<ProcessVnpayPaymentReturn>();
        var processResult = await mediator.Send(processRequest);

        if (!processResult.Item2.IsNullOrEmpty())
        {
            returnModel = processResult.Item1 as PaymentReturnDtos;
            returnUrl = processResult.Item2 as string;
        }

        if (returnUrl.EndsWith("/"))
            returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
        return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] CreateWithdrawRequestCommand request)
    {
        var wallet = await _dbContext.Wallets
                .FirstOrDefaultAsync(w => w.AccountId == Guid.Parse(request.AccountId));

        if (wallet == null)
        {
            return NotFound("Wallet not found.");
        }

        if (wallet.Balance < request.Amount)
        {
            return BadRequest("Insufficient balance.");
        }

        var withdrawRequest = mediator.Send(request);
        var requestWithdrawResponse = new CreateWithdrawTransactionCommand
        {
            AccountId = request.AccountId,
            vnp_Version = withdrawRequest.Result.vnp_Version,
            vnp_Command = withdrawRequest.Result.vnp_Command,
            vnp_TmnCode = withdrawRequest.Result.vnp_TmnCode,
            vnp_Amount = withdrawRequest.Result.vnp_Amount,
            vnp_OrderInfo = withdrawRequest.Result.vnp_OrderInfo,
            vnp_TxnRef = withdrawRequest.Result.vnp_TxnRef,
            vnp_IpAddr = withdrawRequest.Result.vnp_IpAddr,
            vnp_CreateDate = withdrawRequest.Result.vnp_CreateDate,
            vnp_SecureHash = withdrawRequest.Result.vnp_SecureHash,
        };
        var withdrawResponse = mediator.Send(requestWithdrawResponse);

        if (withdrawResponse.Result.vnp_ResponseCode == "00")
        {
            
            wallet.Balance -= request.Amount;
            _dbContext.Wallets.Update(wallet);
            await _dbContext.SaveChangesAsync();

            return Ok(withdrawResponse);
        }
        else
        {
            return BadRequest(withdrawResponse.Result.vnp_Message);
        }
    }

    [HttpGet]
    [Route("momo-return")]
    public async Task<IActionResult> MomoReturn([FromQuery] MomoPaymentResultRequest response)
    {
        string returnUrl = string.Empty;
        var returnModel = new PaymentReturnDtos();
        var processResult = await mediator.Send(response.Adapt<ProcessMomoPaymentReturn>());

        if (processResult.Success)
        {
            returnModel = processResult.Data.Item1 as PaymentReturnDtos;
            returnUrl = processResult.Data.Item2 as string;
        }

        if (returnUrl.EndsWith("/"))
            returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
        return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");
    }
}
