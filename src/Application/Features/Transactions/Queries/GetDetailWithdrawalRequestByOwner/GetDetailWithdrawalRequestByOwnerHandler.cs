using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllWithdrawalRequestByOwner;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetDetailWithdrawalRequestByOwner;
public class GetDetailWithdrawalRequestByOwnerHandler : IRequestHandler<GetDetailWithdrawalRequestByOwnerCommand, TransactionResponseV4>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetDetailWithdrawalRequestByOwnerHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<TransactionResponseV4> Handle(GetDetailWithdrawalRequestByOwnerCommand request, CancellationToken cancellationToken)
    {
        var transactionRequest = _beatSportsDbContext.Transactions
                        .Where(x => x.Id == request.TransactionId)
                        .FirstOrDefault();

        var userWallet = _beatSportsDbContext.Wallets
                         .Where(x => x.Id == transactionRequest.WalletId)
                         .Include(x => x.Account)
                         .FirstOrDefault();

        var owner = _beatSportsDbContext.Owners
                    .Where(x => x.AccountId == userWallet.AccountId)
                    .Include(x => x.Account)
                    .FirstOrDefault();

        var transactionList = _beatSportsDbContext.Transactions
                            .Where(x => (x.WalletId == userWallet.Id || x.WalletTargetId == userWallet.Id) && x.AdminCheckStatus == AdminCheckEnums.Accepted)
                            .ToList();

        var totalAmountReceived = transactionList.Where(x => x.TransactionType.Equals("Nạp tiền") || (x.TransactionType.Equals("Giao dịch trong App") && x.AdminCheckStatus == AdminCheckEnums.Accepted)).Sum(x => x.TransactionAmount);
        var totalAmountWithdrawn = transactionList.Where(x => x.TransactionType.Equals("Rút tiền") && x.AdminCheckStatus == AdminCheckEnums.Accepted).Sum(x => x.TransactionAmount);
        var totalAmountAvailableForWithdrawal = totalAmountReceived - totalAmountWithdrawn;

        var result = new List<TransactionResponseV2>();

        foreach (var transaction in transactionList)
        {
            var fromUser = _beatSportsDbContext.Wallets
                         .Where(x => x.Id == transaction.WalletId)
                         .Include(x => x.Account)
                         .FirstOrDefault();


            var toUser = _beatSportsDbContext.Wallets
                         .Where(x => x.Id == transaction.WalletTargetId)
                         .Include(x => x.Account)
                         .FirstOrDefault();

            var toUserResponse = new UserInfo();

            if (toUser != null)
            {
                toUserResponse.Name = toUser.Account.FirstName + " " + toUser.Account.LastName;
                toUserResponse.WalletId = transaction.WalletTargetId;
                toUserResponse.Role = toUser.Account.Role;
            }

            var res = new TransactionResponseV2()
            {
                TransactionId = transaction.Id,
                From = new UserInfo()
                {
                    Name = fromUser.Account.FirstName + " " + fromUser.Account.LastName,
                    WalletId = transaction.WalletId,
                    Role = fromUser.Account.Role
                },
                To = toUserResponse,
                TransactionAmount = transaction.TransactionAmount,
                TransactionStatus = transaction.TransactionStatus,
                AdminCheckStatus = transaction.AdminCheckStatus.ToString(),
                TransactionDate = transaction.TransactionDate,
                TransactionType = transaction.TransactionType
            };

            result.Add(res);
        }
        var response = new TransactionResponseV4()
        {
            TransactionId = transactionRequest.Id,
            UserInfo = new UserInfo2()
            {
                OwnerId = owner.Id,
                Name = userWallet.Account.FirstName + " " + userWallet.Account.LastName,
                WalletId = transactionRequest.WalletId,
                Role = userWallet.Account.Role,
                OwnerBankAccount = owner.BankAccount,
            },
            OwnerBankNumber = owner.BankAccount,
            TransactionDate = transactionRequest.TransactionDate,
            // Lấy admin check
            TransactionStatus = transactionRequest.AdminCheckStatus.ToString(),
            TransactionList = result,
            TotalAmountReceived = totalAmountReceived,
            TotalAmountWithdrawn = totalAmountWithdrawn,
            TotalAmountAvailableForWithdrawal = totalAmountAvailableForWithdrawal,
            TotalAmountWithdrawalRequestByOwner = transactionRequest.TransactionAmount
        };

        return Task.FromResult(response);
    }
}
