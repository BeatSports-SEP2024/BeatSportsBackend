using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services.VnPay.Config;
using Services.VnPay.Request;
using Services.VnPay.Response;

namespace BeatSportsAPI.Application.Features.Wallets.Commands.CreateWithdrawls;
public class CreateWithdrawTransactionCommand : AcountWithdraw, IRequest<VnpayWithdrawResponse>
{
    public string? vnp_Version { get; set; }
    public string? vnp_Command { get; set; }
    public string? vnp_TmnCode { get; set; }
    public string? vnp_Amount { get; set; }
    public string? vnp_OrderInfo { get; set; }
    public string? vnp_TxnRef { get; set; }
    public string? vnp_IpAddr { get; set; }
    public string? vnp_CreateDate { get; set; }
    public string? vnp_SecureHash { get; set; }
}

public class AcountWithdraw
{
    public string AccountId { get; set; }
}

public class CreateWithdrawTransactionCommandHandler : IRequestHandler<CreateWithdrawTransactionCommand, VnpayWithdrawResponse>
{
    private IBeatSportsDbContext _dbContext;
    private readonly ICurrentUserService currentUserService;
    private readonly IConfiguration _configuration;
    private readonly VnpayConfig vnpayConfig;
    /*private readonly MomoConfig momoConfig;
    private readonly ZaloPayConfig zaloPayConfig;*/

    public CreateWithdrawTransactionCommandHandler(IBeatSportsDbContext dbContext,
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
    public async Task<VnpayWithdrawResponse> Handle(CreateWithdrawTransactionCommand request, CancellationToken cancellationToken)
    {
        using (var client = new HttpClient())
        {
            var query = new List<string>
                {
                    $"vnp_Version={request.vnp_Version}",
                    $"vnp_Command={request.vnp_Command}",
                    $"vnp_TmnCode={request.vnp_TmnCode}",
                    $"vnp_Amount={request.vnp_Amount}",
                    $"vnp_OrderInfo={request.vnp_OrderInfo}",
                    $"vnp_TxnRef={request.vnp_TxnRef}",
                    $"vnp_IpAddr={request.vnp_IpAddr}",
                    $"vnp_CreateDate={request.vnp_CreateDate}",
                    $"vnp_SecureHash={request.vnp_SecureHash}"
                };

            var url = $"{vnpayConfig.PaymentUrl}?{string.Join("&", query)}";

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (content.StartsWith("<"))
            {
                throw new BadRequestException("Unexpected response format from VnPay: " + content);
            }

            var withdrawResponse = JsonConvert.DeserializeObject<VnpayWithdrawResponse>(content);

            if (withdrawResponse.vnp_ResponseCode == "00")
            {
                var accountExist = await _dbContext.Wallets.Where(a => a.AccountId == Guid.Parse(request.AccountId!)).SingleOrDefaultAsync();
                var transactionWaller = new Domain.Entities.Transaction
                {
                    WalletId = accountExist.Id,
                    TransactionMessage = withdrawResponse.vnp_Message,
                    TransactionPayload = content,
                    TransactionStatus = "0",
                    TransactionAmount = decimal.Parse(withdrawResponse.vnp_Amount!),
                    TransactionDate = DateTime.Parse(withdrawResponse.vnp_ResponseDate!),
                    TransactionType = request.vnp_Command,
                };
                _dbContext.Transactions.Add(transactionWaller);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var accountExist = await _dbContext.Wallets.Where(a => a.AccountId == Guid.Parse(request.AccountId!)).SingleOrDefaultAsync();
                var transactionWaller = new Domain.Entities.Transaction
                {
                    WalletId = accountExist.Id,
                    TransactionMessage = withdrawResponse.vnp_Message,
                    TransactionPayload = content,
                    TransactionStatus = "-1",
                    TransactionAmount = decimal.Parse(withdrawResponse.vnp_Amount!),
                    TransactionDate = DateTime.Parse(withdrawResponse.vnp_ResponseDate!),
                    TransactionType = request.vnp_Command,
                };
                _dbContext.Transactions.Add(transactionWaller);
                await _dbContext.SaveChangesAsync();
            }

            return withdrawResponse;
        }
    }
}
