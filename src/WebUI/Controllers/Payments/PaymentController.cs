using System.Net;
using BeatSportsAPI.Application.Common.Base;
using BeatSportsAPI.Application.Features.Wallets.Commands.CreateDeposits;
using BeatSportsAPI.Application.Features.Wallets.Commands.ProcessVnpayPaymentReturnCommand;
using BeatSportsAPI.Application.Features.Wallets.Dtos;
using Duende.IdentityServer.Extensions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

    public PaymentController(IMediator mediator,
            IOptions<VnpayConfig> vnpayConfigOptions)
    {
        this.mediator = mediator;
        this.vnpayConfig = vnpayConfigOptions.Value;
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
        /*var response = new PaymentLinkDtos();*/
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
}
