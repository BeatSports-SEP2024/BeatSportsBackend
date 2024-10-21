using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.VnPay.Config;
using Services.VnPay.Request;
using Utils.Helpers;

namespace BeatSportsAPI.Application.Features.Wallets.Commands.CreateWithdrawls;
public class CreateWithdrawRequestCommand : IRequest<VnpayWithdrawRequest>
{
    public string AccountId { get; set; }
    public decimal Amount { get; set; }
    public string OrderInfo { get; set; }
}

public class CreateWithdrawRequestCommandHandler : IRequestHandler<CreateWithdrawRequestCommand, VnpayWithdrawRequest>
{
    private IBeatSportsDbContext _dbContext;
    private readonly ICurrentUserService currentUserService;
    private readonly IConfiguration _configuration;
    private readonly VnpayConfig vnpayConfig;
    /*private readonly MomoConfig momoConfig;
    private readonly ZaloPayConfig zaloPayConfig;*/

    public CreateWithdrawRequestCommandHandler(IBeatSportsDbContext dbContext,
        ICurrentUserService currentUserService,
        IConfiguration configuration,
        IOptions<VnpayConfig> vnpayConfigOptions
        )
    {
        _dbContext = dbContext;
        this.currentUserService = currentUserService;
        _configuration = configuration;
        this.vnpayConfig = vnpayConfigOptions.Value;
    }
    public async Task<VnpayWithdrawRequest> Handle(CreateWithdrawRequestCommand request, CancellationToken cancellationToken)
    {
        var txnRef = Guid.NewGuid().ToString();
        var createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        var ipAddr = currentUserService.IpAddress;

        var withdrawRequest = new VnpayWithdrawRequest
        {
            vnp_Version = vnpayConfig.Version,
            vnp_Command = "withdraw",
            vnp_TmnCode = vnpayConfig.TmnCode,
            vnp_Amount = ((int)(request.Amount * 100)).ToString(), // Amount in VND
            vnp_OrderInfo = request.OrderInfo,
            vnp_TxnRef = txnRef,
            vnp_IpAddr = ipAddr,
            vnp_CreateDate = createDate
        };

        var data = new StringBuilder();
        data.AppendFormat("{0}={1}", "vnp_Version", withdrawRequest.vnp_Version);
        data.AppendFormat("&{0}={1}", "vnp_Command", withdrawRequest.vnp_Command);
        data.AppendFormat("&{0}={1}", "vnp_TmnCode", withdrawRequest.vnp_TmnCode);
        data.AppendFormat("&{0}={1}", "vnp_Amount", withdrawRequest.vnp_Amount);
        data.AppendFormat("&{0}={1}", "vnp_OrderInfo", withdrawRequest.vnp_OrderInfo);
        data.AppendFormat("&{0}={1}", "vnp_TxnRef", withdrawRequest.vnp_TxnRef);
        data.AppendFormat("&{0}={1}", "vnp_IpAddr", withdrawRequest.vnp_IpAddr);
        data.AppendFormat("&{0}={1}", "vnp_CreateDate", withdrawRequest.vnp_CreateDate);

        withdrawRequest.vnp_SecureHash = HashHelper.HmacSHA512(vnpayConfig.HashSecret, data.ToString());

        return withdrawRequest;
    }
}
