using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.Wallets.Dtos;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Services.VnPay.Config;
using Services.VnPay.Response;

namespace BeatSportsAPI.Application.Features.Wallets.Commands.ProcessVnpayPaymentReturnCommand;
public class ProcessVnpayPaymentReturn : VnpayPayResponse,
    IRequest<(PaymentReturnDtos, string)>
{
}

public class ProcessVnpayPaymentReturnHandler : IRequestHandler<ProcessVnpayPaymentReturn, (PaymentReturnDtos, string)>
{
    private readonly VnpayConfig vnpayConfig;
    private IBeatSportsDbContext _dbContext;
    private readonly IMediator mediator;
    private IMapper _mapper;
    public ProcessVnpayPaymentReturnHandler(IBeatSportsDbContext dbContext,
        IOptions<VnpayConfig> vnpayConfigOptions,
        IMediator mediator,
        IMapper mapper)
    {
        this._dbContext = dbContext;
        this.vnpayConfig = vnpayConfigOptions.Value;
        this.mediator = mediator;
        _mapper = mapper;
    }

    public async Task<(PaymentReturnDtos, string)> Handle(ProcessVnpayPaymentReturn request, CancellationToken cancellationToken)
    {
        string returnUrl = string.Empty;
        string message = "";
        string status = "";
        var result = (new PaymentReturnDtos(), "");
        try
        {
            var resultData = new PaymentReturnDtos();
            var isValidSignature = request.IsValidSignature(vnpayConfig.HashSecret);

            if (isValidSignature)
            {
                var payment = await _dbContext.Payments
                    .Where(p => p.Id == Guid.Parse(request.vnp_TxnRef))
                    .SingleOrDefaultAsync();

                if (payment != null)
                {
                    var merchant = await _dbContext.Merchants
                        .Where(m => m.Id == payment.MerchantId)
                        .SingleOrDefaultAsync();

                    // update transaction
                    if (payment.RequiredAmount == (request.vnp_Amount / 100))
                    {
                        if (payment.PaymentStatus != "0")
                        {
                            if (request.vnp_ResponseCode == "00" &&
                               request.vnp_TransactionStatus == "00")
                            {
                                status = "0";
                                message = "Tran success";


                                /// Update database
                                var transaction = new PaymentTransaction
                                {
                                    TranMessage = message,
                                    TranPayload = JsonConvert.SerializeObject(request),
                                    TranStatus = status,
                                    TranAmount = request.vnp_Amount / 100,
                                    TranDate = DateTime.Now,
                                    PaymentId = Guid.Parse(request.vnp_TxnRef),
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
                                var transactionWallet = new Transaction
                                {
                                    WalletId = wallet.Id,
                                    TransactionMessage = transactionExist.TranMessage,
                                    TransactionPayload = transactionExist.TranPayload,
                                    TransactionStatus = transactionExist.TranStatus,
                                    TransactionAmount = transactionExist.TranAmount,
                                    TransactionDate = transactionExist.TranDate,
                                    TransactionType = payment.PaymentType,
                                    PaymentTransactionId = request.vnp_TxnRef,
                                };
                                _dbContext.Transactions.Add(transactionWallet);
                                await _dbContext.SaveChangesAsync();

                                // After create transactionSuccess, update deposit for wallet
                                var transactionSuccess = await _dbContext.Transactions.Where(t => t.Id == transactionWallet.Id).SingleOrDefaultAsync();
                                if (transactionSuccess == null)
                                {
                                    throw new BadRequestException("04, Input required data");
                                }

                                wallet.Balance += (decimal)(request.vnp_Amount / 100)!;
                                _dbContext.Wallets.Update(wallet);
                                await _dbContext.SaveChangesAsync();
                            }
                            // mã lỗi : Giao dịch thất bại do người dùng đã từ chối xác nhận thanh toán.
                            else
                            {
                                status = "-1";
                                message = "Tran error";

                                /// Update database
                                var transaction = new PaymentTransaction
                                {
                                    TranMessage = message,
                                    TranPayload = JsonConvert.SerializeObject(request),
                                    TranStatus = status,
                                    TranAmount = request.vnp_Amount / 100,
                                    TranDate = DateTime.Now,
                                    PaymentId = Guid.Parse(request.vnp_TxnRef),
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
                                var transactionWallet = new Transaction
                                {
                                    WalletId = wallet.Id,
                                    TransactionMessage = transactionExist.TranMessage,
                                    TransactionPayload = transactionExist.TranPayload,
                                    TransactionStatus = transactionExist.TranStatus,
                                    TransactionAmount = transactionExist.TranAmount,
                                    TransactionDate = transactionExist.TranDate,
                                    TransactionType = payment.PaymentType,
                                    PaymentTransactionId = request.vnp_TxnRef
                                };
                                _dbContext.Transactions.Add(transactionWallet);
                                await _dbContext.SaveChangesAsync();

                                // After create transactionSuccess, update deposit for wallet
                                // beacuse status error to not update wallet 
                            }
                        }
                        else
                        {
                            throw new BadRequestException("04, Invalid amount");
                        }
                    }
                    returnUrl = (merchant?.MerchantReturnUrl + $"?payment={status}") ?? string.Empty;
                }
                else
                {
                    resultData.PaymentStatus = "11";
                    resultData.PaymentMessage = "Can't find payment at payment service";
                }

                if (request.vnp_ResponseCode == "00")
                {
                    resultData.PaymentStatus = "00";
                    resultData.PaymentId = payment.Id.ToString();
                    ///TODO: Make signature
                    resultData.Signature = Guid.NewGuid().ToString();
                }
                else
                {
                    resultData.PaymentStatus = "10";
                    resultData.PaymentMessage = "Payment process failed";
                }

                result = (resultData, returnUrl);
            }
            else
            {
                resultData.PaymentStatus = "99";
                resultData.PaymentMessage = "Invalid signature in response";

            }
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ex.Message);
        }

        return result;
    }
}
