using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.TransactionThridparty.Queries.GetTransactionByCusId;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BeatSportsAPI.Application.Features.TransactionThridparty.Queries.GetAllTransactionForAdmin;
public class GetTransactionsForAdminCommand : IRequest<TransactionThirdpartyForAdminResponse>
{
}

public class GetTransactionsForAdminCommandHandler : IRequestHandler<GetTransactionsForAdminCommand, TransactionThirdpartyForAdminResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetTransactionsForAdminCommandHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public async Task<TransactionThirdpartyForAdminResponse> Handle(GetTransactionsForAdminCommand request, CancellationToken cancellationToken)
    {
        var listTransaction = new List<TransactionThirdpartyResponse>();

        var paymentExist = await _beatSportsDbContext.Payments.ToListAsync();
        foreach (var payment in paymentExist)
        {
            // kiểm tra xem transation của wallet đã update chưa, nếu có update rồi thì tức là call back đã gọi thành công
            // nếu mà chưa có tức là call back ko được, phải thực hiện hoàn tiền từ buosc này, nên chưa handle, chỉ check thôi
            var transactionWallet = await _beatSportsDbContext.Transactions.Where(t => t.PaymentTransactionId == payment.Id.ToString()).FirstOrDefaultAsync();
            // đã call back thành công nè
            if (transactionWallet != null)
            {
                var transactionCusIdExist = await _beatSportsDbContext.PaymentTransactions.Where(pt => pt.PaymentId == payment.Id).FirstOrDefaultAsync();

                // Lấy các thông tin của user ra 
                var customerInfo = await _beatSportsDbContext.Transactions
                    .Include(t => t.Wallet)
                        .ThenInclude(w => w.Account)
                            .ThenInclude(a => a.Customer)
                    .Where(t => t.PaymentTransactionId == payment.Id.ToString())
                    .FirstOrDefaultAsync();

                //var transactionPayload = JsonConvert.DeserializeObject<TransactionPayload>(transactionCusIdExist.TranPayload!);
                //JObject transactionPayload = JObject.Parse(transactionCusIdExist!.TranPayload!);
                if (transactionCusIdExist != null)
                {
                    var transactionPayload = JsonConvert.DeserializeObject<ExpandoObject>(transactionCusIdExist!.TranPayload!)!;

                    var data = new TransactionThirdpartyResponse
                    {
                        TransactionId = transactionCusIdExist.Id,
                        TransactionMessage = transactionCusIdExist!.TranMessage,
                        TransactionPayload = transactionPayload,
                        TransactionStatus = transactionCusIdExist.TranStatus,
                        TransactionAmount = transactionCusIdExist.TranAmount,
                        TransactionDate = transactionCusIdExist.TranDate,
                        PaymentId = transactionCusIdExist.PaymentId,
                        TransactionType = payment.PaymentType,
                        CallbackStatus = "Success",
                        CustomerId = customerInfo.Wallet.Account.Customer.Id,
                        CustomerName = customerInfo.Wallet.Account.FirstName +" "+ customerInfo.Wallet.Account.LastName,
                        CustomerUsername = customerInfo.Wallet.Account.UserName,
                    };
                    listTransaction.Add(data);
                }
            }

            // call back thất bại => tức là paymentTransaction tồn tại nhưng transaction của ví ko tồn tại
            else
            {
                var transactionCusIdExist = await _beatSportsDbContext.PaymentTransactions.Where(pt => pt.PaymentId == payment.Id).FirstOrDefaultAsync();
                //var transactionPayload = JsonConvert.DeserializeObject<TransactionPayload>(transactionCusIdExist.TranPayload!);
                //JObject transactionPayload = JObject.Parse(transactionCusIdExist!.TranPayload!);
                if (transactionCusIdExist != null)
                {
                    var transactionPayload = JsonConvert.DeserializeObject<ExpandoObject>(transactionCusIdExist!.TranPayload!)!;

                    var data = new TransactionThirdpartyResponse
                    {
                        TransactionId = transactionCusIdExist.Id,
                        TransactionMessage = transactionCusIdExist!.TranMessage,
                        TransactionPayload = transactionPayload,
                        TransactionStatus = transactionCusIdExist.TranStatus,
                        TransactionAmount = transactionCusIdExist.TranAmount,
                        TransactionDate = transactionCusIdExist.TranDate,
                        PaymentId = transactionCusIdExist.PaymentId,
                        TransactionType = payment.PaymentType,
                        CallbackStatus = "Failed"
                    };
                    listTransaction.Add(data);
                }
            }
        }

        // lọc ra những anh nào CallbackStatus = "Success" và transactionCusIdExist.TranStatus= '0'
        var successfulTransactions = listTransaction
            .Where(t => t.CallbackStatus == "Success" && t.TransactionStatus == "0")
            .ToList();
        // từ chối ko nạp tiền
        var failedTransactions = listTransaction
           .Where(t => t.CallbackStatus == "Success" && t.TransactionStatus == "-1")
           .ToList();

        // bị 1ỗi gì đó, ko apply tiền vào ví chánh thức của customer nữa
        var failedCallBackTransactions = listTransaction
          .Where(t => t.CallbackStatus == "Failed")
          .ToList();

        decimal totalCustomerDeposit = _beatSportsDbContext.Transactions
            .Where(t => t.TransactionType == "Nạp tiền")
            .Sum(t => t.TransactionAmount ?? 0);

        // Kiểm tra xem tiền đó owner rút và đã được duyệt rồi
        decimal totalOwnerWithdrawal = _beatSportsDbContext.Transactions
            .Where(t => t.TransactionType == "Rút tiền" && (int)t.AdminCheckStatus == 1)
            .Sum(t => t.TransactionAmount ?? 0);

        var withdrawHistory = await _beatSportsDbContext.Transactions
            .Where(t => t.TransactionType == "Rút tiền" && (int)t.AdminCheckStatus == 1)
            .Select(t => new WithdrawHistory
            {
                OwnerAccount = t.Wallet.Account.UserName,
                OwnerId = t.Wallet.Account.Owner.Id,
                OwnerName = t.Wallet.Account.Owner.Account.FirstName + " " + t.Wallet.Account.Owner.Account.LastName,
                TotalOwnerWithdraw = t.TransactionAmount ?? 0,
                TransactionDate = t.TransactionDate ?? DateTime.Now
            }).ToListAsync(cancellationToken);


        // Chỗ này get tổng tiền customer nạp vào 
        decimal totalAdminMoney = successfulTransactions.Sum(t => t.TransactionAmount ?? 0);

        // Chỗ này check số dư khả dĩ mà owner có thể rút
        decimal totalMoneyCanWithdraw = totalAdminMoney - totalOwnerWithdrawal;

        var totalCountRecord = listTransaction.Count;

        return new TransactionThirdpartyForAdminResponse
        {
            ListTransactionThirdpartyResponse = listTransaction.OrderByDescending(p => p.TransactionDate).ToList(),
            TotalCount = totalCountRecord,
            TotalSuccess = successfulTransactions.Count,
            TotalFailed = failedTransactions.Count,
            TotalAdminMoney = totalAdminMoney,
            TotalOwnerWithdraw = totalOwnerWithdrawal,
            TotalMoneyCanWithdraw = totalMoneyCanWithdraw,
            TotalMoneyCustomerDeposit = totalCustomerDeposit,
            WithdrawHistoryResponse = withdrawHistory,
        };
    }
}