using System.Net;
using BeatSportsAPI.Application.Common.Base;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Middlewares;
using BeatSportsAPI.Application.Features.Wallets.Commands.CreateDeposits;
using BeatSportsAPI.Application.Features.Wallets.Commands.CreateWithdrawls;
using BeatSportsAPI.Application.Features.Wallets.Commands.ProcessMomoPaymentReturnCommand;
using BeatSportsAPI.Application.Features.Wallets.Commands.ProcessVnpayPaymentReturnCommand;
using BeatSportsAPI.Application.Features.Wallets.Commands.ProcessZalopayPaymentReturnCommand;
using BeatSportsAPI.Application.Features.Wallets.Dtos;
using BeatSportsAPI.Domain.Enums;
using Duende.IdentityServer.Extensions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Services.Momo.Request;
using Services.VnPay.Config;
using Services.VnPay.Response;
using Services.ZaloPay.Request;
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
    [CustomAuthorize(RoleEnums.Customer)]
    public async Task<PaymentLinkDtos> Create([FromBody] PaymentByThirdWalletCommand request)
    {
        var response = await mediator.Send(request);
        return response;
    }

    #region kiểm tra thử cho zalopay với key, appid, của version2
    
    /// <summary>
    /// Test thu api zalo pay
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /*[HttpGet]
    [ProducesResponseType(typeof(BaseResultWithData<PaymentLinkDtos>), 200)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public Task<Dictionary<string, object>> GetHiHi()
    {
        string app_id = "2554";
        string key1 = "sdngKKJmqEMzvh5QQcdD2A9XBSKUNaYn";
        string create_order_url = "https://sb-openapi.zalopay.vn/v2/create";
        Random rnd = new Random();
        var embed_data = new
        {
            RedirectUrl = "https://localhost:5001/api/v1/payment/callback"
        };
        var items = new[] { new { } };
        var param = new Dictionary<string, string>();
        var app_trans_id = rnd.Next(1000000);  // Generate a random order's ID.
        var callback_url = "https://localhost:5001/api/v1/payment/callback";

        param.Add("app_id", app_id);
        param.Add("app_user", "user123");
        param.Add("app_time", Utils.Extensions.DateTimeExtensions.GetTimeStamp().ToString());
        param.Add("amount", "50000");
        param.Add("app_trans_id", DateTime.Now.ToString("yyMMdd") + "_" + app_trans_id); // mã giao dich có định dạng yyMMdd_xxxx
        param.Add("embed_data", JsonConvert.SerializeObject(embed_data));
        param.Add("item", JsonConvert.SerializeObject(items));
        param.Add("description", "Lazada - Thanh toán đơn hàng #" + app_trans_id);
        param.Add("bank_code", "zalopayapp");
        param.Add("callback_url", callback_url);

        var data = app_id + "|" + param["app_trans_id"] + "|" + param["app_user"] + "|" + param["amount"] + "|"
                + param["app_time"] + "|" + param["embed_data"] + "|" + param["item"];
        param.Add("mac", Utils.Helpers.HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key1, data));

        var result = Utils.Helpers.HttpHelper.PostFormAsync(create_order_url, param);


        return result;


        *//*{
              "return_code": 1,
              "return_message": "Giao dịch thành công",
              "sub_return_code": 1,
              "sub_return_message": "Giao dịch thành công",
              "zp_trans_token": "ACgt9tYIWCVr0g9KVr5fn7Cw",
              "order_url": "https://qcgateway.zalopay.vn/openinapp?order=eyJ6cHRyYW5zdG9rZW4iOiJBQ2d0OXRZSVdDVnIwZzlLVnI1Zm43Q3ciLCJhcHBpZCI6MjU1NH0=",
              "order_token": "ACgt9tYIWCVr0g9KVr5fn7Cw"
        }*//*
    }*/

    /*[HttpGet]
    [Route("callback1")]
    public IActionResult ZaloReturn([FromQuery] ZalopayPaymentResultRequest response)
    {
        string key2 = "trMrHtvjo6myautxDUiAcYsVtaeQ8nhf";
        //var result = new Dictionary<string, object>();
        try
        {
            string returnUrl = string.Empty;
            var returnModel = new PaymentReturnDtos();
            var result = new BaseResultWithData<(PaymentReturnDtos, string)>();
            var resultData = new PaymentReturnDtos();
            var isValidSignature = response.IsValidSignature(key2);
            if (isValidSignature)
            {
                resultData.PaymentStatus = "00";
            }

            var dataStr = "";
            //var reqMac = Convert.ToString(cbdata["mac"]);

            *//*var mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, key2, dataStr);

            Console.WriteLine("mac = {0}", mac);*//*

            // kiểm tra callback hợp lệ (đến từ ZaloPay server)
            *//*if (!reqMac.Equals(mac))
            {
                // callback không hợp lệ
                result["return_code"] = -1;
                result["return_message"] = "mac not equal";
            }
            else
            {
                // thanh toán thành công
                // merchant cập nhật trạng thái cho đơn hàng
                var dataJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(dataStr);
                Console.WriteLine("update order's status = success where app_trans_id = {0}", dataJson["app_trans_id"]);

                result["return_code"] = 1;
                result["return_message"] = "success";
            }*//*
        }
        catch (Exception ex)
        {
            *//*result["return_code"] = 0; // ZaloPay server sẽ callback lại (tối đa 3 lần)
            result["return_message"] = ex.Message;*//*
        }

        // thông báo kết quả cho ZaloPay server
        return Ok();
    }
    [HttpPost]
    [Route("callback")]
    public IActionResult CallbackZalo([FromBody] CallbackOrder callbackOrder)
    {
        string key2 = "trMrHtvjo6myautxDUiAcYsVtaeQ8nhf";
        var isValidSignature = callbackOrder.IsValidMac(key2);
        if (isValidSignature)
        {
            return StatusCode(400, "Bad Request");
        }

        // ViewData["result"] = result;
        return Ok();
    }*/
    #endregion


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

    [HttpGet]
    [Route("zalopay-return")]
    public async Task<IActionResult> ZaloReturn([FromQuery] ZalopayPaymentResultRequest response)
    {
        string returnUrl = string.Empty;
        var returnModel = new PaymentReturnDtos();
        var processResult = await mediator.Send(response.Adapt<ProcessZaloPaymentReturn>());

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
