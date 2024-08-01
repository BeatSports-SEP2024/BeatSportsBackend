﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactions;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllWithdrawalRequestByOwner;
public class GetAllWithdrawalRequestByOwnerHandler : IRequestHandler<GetAllWithdrawalRequestByOwnerCommand, List<TransactionResponseV3>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetAllWithdrawalRequestByOwnerHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }
    public Task<List<TransactionResponseV3>> Handle(GetAllWithdrawalRequestByOwnerCommand request, CancellationToken cancellationToken)
    {
        if (request.KeyWord == null)
        {
            request.KeyWord = "";
        }

        if (request.Filter == null)
        {
            request.Filter = "";
        }

        var query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionType.Equals("Rút tiền") && t.AdminCheckStatus == AdminCheckEnums.Pending)
            .OrderByDescending(b => b.Created);

        var a = query.Count();

        if (request.Filter.Equals("TransactionStatus"))
        {
            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionType.Equals("Rút tiền") && t.AdminCheckStatus == AdminCheckEnums.Pending && t.TransactionStatus.Contains(request.KeyWord))
            .OrderByDescending(b => b.Created);

        }
        else if (request.Filter.Equals("AdminCheckStatus"))
        {
            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionType.Equals("Rút tiền") && t.AdminCheckStatus == AdminCheckEnums.Pending && t.AdminCheckStatus.ToString().Contains(request.KeyWord))
            .OrderByDescending(b => b.Created);
        }
        else if (request.Filter.Equals("TransactionType"))
        {
            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionType.Equals("Rút tiền") && t.AdminCheckStatus == AdminCheckEnums.Pending && t.TransactionType.Contains(request.KeyWord))
            .OrderByDescending(b => b.Created);
        }
        else if (request.Filter.Equals("From"))
        {
            var fromUser = _beatSportsDbContext.Wallets
                         .Include(x => x.Account)
                         .ToList();

            fromUser = fromUser.Where(x => (x.Account.FirstName + " " + x.Account.LastName).Contains(request.KeyWord)).ToList();

            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionType.Equals("Rút tiền") && t.AdminCheckStatus == AdminCheckEnums.Pending && fromUser.Any(x => x.Id == t.WalletId))
            .OrderByDescending(b => b.Created);

        }
        else if (request.Filter.Equals("To"))
        {
            var fromUser = _beatSportsDbContext.Wallets
                         .Include(x => x.Account)
                         .ToList();

            fromUser = fromUser.Where(x => (x.Account.FirstName + " " + x.Account.LastName).Contains(request.KeyWord)).ToList();

            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionType.Equals("Rút tiền") && t.AdminCheckStatus == AdminCheckEnums.Pending && fromUser.Any(x => x.Id == t.WalletTargetId))
            .OrderByDescending(b => b.Created);
        }


        var result = new List<TransactionResponseV3>();

        foreach (var transaction in query)
        {
            var userWallet = _beatSportsDbContext.Wallets
                         .Where(x => x.Id == transaction.WalletId)
                         .Include(x => x.Account)
                         .FirstOrDefault();

            var owner = _beatSportsDbContext.Owners
                        .Where(x => x.AccountId == userWallet.AccountId)
                        .Include(x => x.Account)
                        .FirstOrDefault();

            var response = new TransactionResponseV3()
            {
                TransactionId = transaction.Id,
                UserInfo = new UserInfo2()
                {
                    OwnerId = owner.Id,
                    Name = userWallet.Account.FirstName + " " + userWallet.Account.LastName,
                    WalletId = transaction.WalletId,
                    Role = userWallet.Account.Role
                },
                TransactionAmount = transaction.TransactionAmount,
                TransactionStatus = transaction.TransactionStatus,
                AdminCheckStatus = transaction.AdminCheckStatus.ToString(),
                TransactionDate = transaction.TransactionDate,
                TransactionType = transaction.TransactionType
            };

            result.Add(response);
        }

        return Task.FromResult(result);
    }
}