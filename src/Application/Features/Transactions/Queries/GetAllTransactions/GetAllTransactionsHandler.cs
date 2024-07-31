using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        var query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete)
            .OrderByDescending(b => b.Created);

        if (request.Filter.Equals("TransactionStatus"))
        {
            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionStatus.Contains(request.KeyWord))
            .OrderByDescending(b => b.Created);

        }else if (request.Filter.Equals("AdminCheckStatus"))
        {
            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.AdminCheckStatus.ToString().Contains(request.KeyWord))
            .OrderByDescending(b => b.Created);
        }
        else if (request.Filter.Equals("TransactionType"))
        {
            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && t.TransactionType.Contains(request.KeyWord))
            .OrderByDescending(b => b.Created);
        }else if (request.Filter.Equals("From"))
        {
            var fromUser = _beatSportsDbContext.Wallets
                         .Include(x => x.Account)
                         .ToList();

            fromUser = fromUser.Where(x => (x.Account.FirstName + " " + x.Account.LastName).Contains(request.KeyWord)).ToList();

            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && fromUser.Any(x => x.Id == t.WalletId))
            .OrderByDescending(b => b.Created);

        }else if (request.Filter.Equals("To"))
        {
            var fromUser = _beatSportsDbContext.Wallets
                         .Include(x => x.Account)
                         .ToList();

            fromUser = fromUser.Where(x => (x.Account.FirstName + " " + x.Account.LastName).Contains(request.KeyWord)).ToList();

            query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete && fromUser.Any(x => x.Id == t.WalletTargetId))
            .OrderByDescending(b => b.Created);
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

            var response = new TransactionResponseV2()
            {
                TransactionId = transaction.Id,
                From = new UserInfo()
                {
                    Name = fromUser.Account.FirstName + " " + fromUser.Account.LastName,
                    WalletId = transaction.WalletId,
                    Role = fromUser.Account.Role
                },
                To = new UserInfo()
                {
                    Name = toUser.Account.FirstName + " " + toUser.Account.LastName,
                    WalletId = transaction.WalletTargetId,
                    Role = toUser.Account.Role
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