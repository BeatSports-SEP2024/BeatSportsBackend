using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Base;
using BeatSportsAPI.Application.Common.Contants;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.Wallets.Dtos;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services.Momo.Config;
using Services.Momo.Request;
using Services.VnPay.Config;

namespace BeatSportsAPI.Application.Features.Wallets.Commands.ProcessMomoPaymentReturnCommand;
public class ProcessMomoPaymentReturn : MomoPaymentResultRequest,
        IRequest<BaseResultWithData<(PaymentReturnDtos, string)>>
{
}

public class ProcessMomoPaymentReturnHandler : IRequestHandler<ProcessMomoPaymentReturn, BaseResultWithData<(PaymentReturnDtos, string)>>
{
    
    private readonly MomoConfig momoConfig;
    private IBeatSportsDbContext _dbContext;
    private readonly IMediator mediator;
    private IMapper _mapper;
    public ProcessMomoPaymentReturnHandler(IBeatSportsDbContext dbContext,
        IOptions<MomoConfig> momoConfigOptions,
        IMediator mediator,
        IMapper mapper)
    {
        this._dbContext = dbContext;
        this.momoConfig = momoConfigOptions.Value;
        this.mediator = mediator;
        _mapper = mapper;
    }

    public async Task<BaseResultWithData<(PaymentReturnDtos, string)>> Handle(ProcessMomoPaymentReturn request, CancellationToken cancellationToken)
    {
        string returnUrl = "";
        var result = new BaseResultWithData<(PaymentReturnDtos, string)>();
        try
        {
            var resultData = new PaymentReturnDtos();
            var isValidSignature = request.IsValidSignature(momoConfig.AccessKey, momoConfig.SecretKey);

            if (isValidSignature)
            {
                var payment = await _dbContext.Payments
                    .Where(p => p.Id == Guid.Parse(request.orderId)).SingleOrDefaultAsync();
                if(payment != null)
                {
                    var merchant = await _dbContext.Merchants
                        .Where(m => m.Id == payment.MerchantId)
                        .SingleOrDefaultAsync();

                    if (request.resultCode == 0)
                    {
                        resultData.PaymentStatus = "00";

                        resultData.PaymentId = payment.Id.ToString();

                        ///TODO: Update database
                        var transaction = new PaymentTransaction
                        {
                            TranMessage = MessageConstants.OK,
                            TranPayload = JsonConvert.SerializeObject(request),
                            TranStatus = "0",
                            TranAmount = request.amount,
                            TranDate = DateTime.Now,
                            PaymentId = Guid.Parse(request.orderId),
                            TranRefId = payment.PaymentRefId
                        };
                        _dbContext.PaymentTransactions.Add(transaction);
                        await _dbContext.SaveChangesAsync();
                        /// Confirm success
                        var transactionExist = await _dbContext.PaymentTransactions
                            .Where(t => t.Id == transaction.Id).SingleOrDefaultAsync();
                        if (transactionExist == null)
                        {
                            throw new BadRequestException("04, Input required data");
                        }
                        /// after success update transaction for wallets
                        var wallet = await _dbContext.Wallets.Where(w => w.AccountId == payment.AccountId).SingleOrDefaultAsync();
                        var transactionWallet = new Domain.Entities.Transaction
                        {
                            WalletId = wallet.Id,
                            TransactionMessage = transactionExist.TranMessage,
                            TransactionPayload = transactionExist.TranPayload,
                            TransactionStatus = transactionExist.TranStatus,
                            TransactionAmount = transactionExist.TranAmount,
                            TransactionDate = transactionExist.TranDate,
                            TransactionType = payment.PaymentType,
                            PaymentTransactionId = request.orderId
                        };
                        _dbContext.Transactions.Add(transactionWallet);
                        await _dbContext.SaveChangesAsync();

                        // After create transactionSuccess, update deposit for wallet
                        var transactionSuccess = await _dbContext.Transactions.Where(t => t.Id == transactionWallet.Id).SingleOrDefaultAsync();
                        if (transactionSuccess == null)
                        {
                            throw new BadRequestException("04, Input required data");
                        }

                        wallet.Balance += (decimal)request.amount!;
                        _dbContext.Wallets.Update(wallet);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        resultData.PaymentStatus = "10";
                        resultData.PaymentMessage = "Payment process failed";
                    }

                    returnUrl = merchant?.MerchantReturnUrl ?? string.Empty;

                    result.Success = true;
                    result.Message = MessageConstants.OK;
                    result.Data = (resultData, returnUrl);

                }
                else
                {
                    resultData.PaymentStatus = "11";
                    resultData.PaymentMessage = "Can't find payment at payment service";
                }
            }
            else
            {
                resultData.PaymentStatus = "99";
                resultData.PaymentMessage = "Invalid signature in response";
            }
        }
        catch(Exception ex)
        {
            result.Set(false, MessageConstants.Error);
            result.Errors.Add(new BaseError()
            {
                Code = MessageConstants.Exception,
                Message = ex.Message,
            });
        }
        return await Task.FromResult(result);
    }
}