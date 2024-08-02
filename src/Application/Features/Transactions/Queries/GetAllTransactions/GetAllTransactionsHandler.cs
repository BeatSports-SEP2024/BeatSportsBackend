using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactions;
public class GetAllTransactionsHandler : IRequestHandler<GetAllTransactionsCommand, List<TransactionResponseV2>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllTransactionsHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<List<TransactionResponseV2>> Handle(GetAllTransactionsCommand request, CancellationToken cancellationToken)
    {
        if(request.KeyWord == null)
        {
            request.KeyWord = "";
        }

        if (request.Filter == null)
        {
            request.Filter = "";
        }

        var query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete)
            .OrderByDescending(b => b.Created);

        if (request.Filter.Equals("TransactionType"))
        {
            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionType.Contains(request.KeyWord))
            .OrderByDescending(b => b.Created);
        }else if (request.Filter.Equals("From"))
        {
            var fromUser = _beatSportsDbContext.Wallets
                         .Include(x => x.Account)
                         .ToList();

            fromUser = fromUser.Where(x => RemoveDiacritics(x.Account.FirstName + " " + x.Account.LastName).ToLower().Contains(RemoveDiacritics(request.KeyWord).ToLower())).ToList();

            var a = new Queue<Transaction>();

            foreach (var user in fromUser)
            {
                var b = _beatSportsDbContext.Transactions
                       .Where(t => !t.IsDelete && user.Id == t.WalletId)
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

            foreach(var user in toUser)
            {
                 var b = _beatSportsDbContext.Transactions
                        .Where(t => !t.IsDelete && user.Id == t.WalletTargetId)
                        .FirstOrDefault();
                if(b != null)
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
        var result = new List<TransactionResponseV2>();

        foreach(var transaction in query)
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

            if(toUser != null)
            {
                toUserResponse.Name = toUser.Account.FirstName + " " + toUser.Account.LastName;
                toUserResponse.WalletId = transaction.WalletTargetId;
                toUserResponse.Role = toUser.Account.Role;
            }

            var response = new TransactionResponseV2()
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