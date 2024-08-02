using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactions;
using BeatSportsAPI.Domain.Entities;
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

        if (request.Filter.Equals("TransactionType"))
        {
            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionType.Contains(request.KeyWord) && t.AdminCheckStatus == AdminCheckEnums.Pending)
            .OrderByDescending(b => b.Created);
        }
        else if (request.Filter.Equals("From"))
        {
            var fromUser = _beatSportsDbContext.Wallets
                         .Include(x => x.Account)
                         .ToList();

            fromUser = fromUser.Where(x => RemoveDiacritics(x.Account.FirstName + " " + x.Account.LastName).ToLower().Contains(RemoveDiacritics(request.KeyWord).ToLower())).ToList();

            var a = new Queue<Transaction>();

            foreach (var user in fromUser)
            {
                var b = _beatSportsDbContext.Transactions
                       .Where(t => !t.IsDelete && user.Id == t.WalletId && t.AdminCheckStatus == AdminCheckEnums.Pending)
                       .FirstOrDefault();
                if (b != null)
                {
                    a.Enqueue(b);
                }

            }

            query = (IOrderedQueryable<Transaction>)a.AsQueryable();

        }
        else if (request.Filter.Equals("To"))
        {
            var toUser = _beatSportsDbContext.Wallets
                         .Include(x => x.Account)
                         .ToList();

            toUser = toUser.Where(x => RemoveDiacritics(x.Account.FirstName + " " + x.Account.LastName).ToLower().Contains(RemoveDiacritics(request.KeyWord).ToLower())).ToList();

            var a = new Queue<Transaction>();

            foreach (var user in toUser)
            {
                var b = _beatSportsDbContext.Transactions
                       .Where(t => !t.IsDelete && user.Id == t.WalletTargetId && t.AdminCheckStatus == AdminCheckEnums.Pending)
                       .FirstOrDefault();
                if (b != null)
                {
                    a.Enqueue(b);
                }

            }

            query = (IOrderedQueryable<Transaction>)a.AsQueryable();

        }

        if (request.StartTime.HasValue && request.EndTime.HasValue)
        {
            query = (IOrderedQueryable<Transaction>)query.Where(tp => tp.TransactionDate.Value.Date >= request.StartTime.Value.Date && tp.TransactionDate.Value.Date <= request.EndTime.Value.Date).AsQueryable();
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

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}