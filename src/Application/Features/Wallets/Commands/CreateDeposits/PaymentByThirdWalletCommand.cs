using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.Wallets.Dtos;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Services.Momo.Config;
using Services.Momo.Request;
using Services.VnPay.Config;
using Services.VnPay.Request;

namespace BeatSportsAPI.Application.Features.Wallets.Commands.CreateDeposits;
public class PaymentByThirdWalletCommand : IRequest<PaymentLinkDtos>
{
    public string PaymentContent { get; set; } = string.Empty;
    public string PaymentCurrency { get; set; } = string.Empty;
    public string PaymentRefId { get; set; } = string.Empty;
    public long? RequiredAmount { get; set; }
    //public DateTime? PaymentDate { get; set; } = DateTime.Now;
    //public DateTime? ExpireDate { get; set; } = DateTime.Now.AddMinutes(15);
    public string? PaymentLanguage { get; set; } = string.Empty;

    public string? MerchantId { get; set; } = string.Empty;
    public string? PaymentDestinationId { get; set; } = string.Empty;

    //public CreatePaymentSignature CreatePaymentSignature { get; set; }
    public TransactionWallet Transaction { get; set; } = null!;
}

public class CreatePaymentSignature
{
    public string? SignValue { get; set; } = string.Empty;
    public string? SignAlgo { get; set; } = string.Empty;
}
public class TransactionWallet
{
    //public double? Price { get; set; }
    public string? AccountId { get; set; }
    public string? PaymentMethodId { get; set; }

    // Type (Deposit or Withdrawls)
    public string TransactionType { get; set; } = null!;
}

public class PaymentByThirdWalletCommandHandler : IRequestHandler<PaymentByThirdWalletCommand, PaymentLinkDtos>
{
    private IBeatSportsDbContext _dbContext;
    private readonly ICurrentUserService currentUserService;
    private readonly IConfiguration _configuration;
    private readonly VnpayConfig vnpayConfig;
    private readonly MomoConfig momoConfig;

    public PaymentByThirdWalletCommandHandler(IBeatSportsDbContext dbContext,
        ICurrentUserService currentUserService,
        IConfiguration configuration,
        IOptions<VnpayConfig> vnpayConfigOptions,
        IOptions<MomoConfig> momoConfigOptions
        )
    {
        _dbContext = dbContext;
        this.currentUserService = currentUserService;
        _configuration = configuration;
        this.vnpayConfig = vnpayConfigOptions.Value;
        this.momoConfig = momoConfigOptions.Value;
    }
    public async Task<PaymentLinkDtos> Handle(PaymentByThirdWalletCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = new Payment
            {
                PaymentContent = request.PaymentContent,
                PaymentCurrency = request.PaymentCurrency,
                PaymentRefId = request.PaymentRefId,
                RequiredAmount = request.RequiredAmount,
                PaymentDate = DateTime.Now,
                ExpireDate = DateTime.Now.AddMinutes(15),
                /*PaymentStatus = "0",*/
                PaymentLanguage = request.PaymentLanguage,
                PaymentType = request.Transaction.TransactionType,
                MerchantId = Guid.Parse(request.MerchantId!),
                PaymentDestinationId = Guid.Parse(request.PaymentDestinationId!),

                AccountId = Guid.Parse(request.Transaction.AccountId!),
                PaymentMethodId = Guid.Parse(request.Transaction.PaymentMethodId!)
            };
            _dbContext.Payments.Add(payment);

            /*var signature = new PaymentSignature
            {
                SignValue = request.CreatePaymentSignature.SignValue,
                SignAlgo = request.CreatePaymentSignature.SignAlgo,
                SignOwn = request.MerchantId,
                SignDate = DateTime.Now,
                PaymentId = payment.Id,
                IsValid = true,
            };
            _dbContext.PaymentSignature.Add(signature);*/

            await _dbContext.SaveChangesAsync();

            // check đích thanh toán
            var paymentUrl = string.Empty;
            var destinationExist = await _dbContext.PaymentsDestinations
                .Where(d => d.Id == Guid.Parse(request.PaymentDestinationId!))
                .Select(d => d.DesShortName)
                .SingleOrDefaultAsync();
            switch (destinationExist)
            {
                case "VNPAY":
                    var vnpayPayRequest = new VnpayPayRequest(
                        vnpayConfig.Version,
                        vnpayConfig.TmnCode,
                        DateTime.Now,
                        currentUserService.IpAddress ?? string.Empty,
                        request.RequiredAmount ?? 0,
                        request.PaymentCurrency ?? string.Empty,
                        request.Transaction.TransactionType, // (deposit or withdrawls)
                        request.PaymentContent ?? string.Empty,
                        vnpayConfig.ReturnUrl,
                        payment.Id.ToString() ?? string.Empty);
                    paymentUrl = vnpayPayRequest.GetLink(vnpayConfig.PaymentUrl, vnpayConfig.HashSecret);
                    break;

                case "MOMO":
                    var momoOneTimePayRequest = new MomoPaymentRequest(momoConfig.PartnerCode,
                        payment.Id.ToString() ?? string.Empty, request.RequiredAmount ?? 0, payment.Id.ToString() ?? string.Empty,
                        request.PaymentContent ?? string.Empty, momoConfig.ReturnUrl, momoConfig.IpnUrl, "captureWallet",
                        string.Empty);
                    momoOneTimePayRequest.MakeSignature(momoConfig.AccessKey, momoConfig.SecretKey);
                    (bool createMomoLinkResult, string? createMessage) = momoOneTimePayRequest.GetLink(momoConfig.PaymentUrl);
                    if (createMomoLinkResult)
                    {
                        paymentUrl = createMessage;
                    }
                    else
                    {
                        throw new BadRequestException(createMessage);
                    }
                    break;
                default:
                    break;
            }

            return new PaymentLinkDtos
            {
                PaymentId = payment.Id.ToString(),
                PaymentUrl = paymentUrl,
            };
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }
    }
}
