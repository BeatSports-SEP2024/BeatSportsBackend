using System;
using System.Collections.Generic;
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
public class GetAllTransactionsHandler : IRequestHandler<GetAllTransactionsCommand, PaginatedTransactionResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllTransactionsHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedTransactionResponse> Handle(GetAllTransactionsCommand request, CancellationToken cancellationToken)
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
            .Where(t => !t.IsDelete)
            .OrderByDescending(b => b.Created);

        if (request.Filter.Equals("TransactionStatus"))
        {
            query = query
                .Where(t => t.TransactionStatus.Contains(request.KeyWord))
                .OrderByDescending(t => t.Created);
        }
        else if (request.Filter.Equals("AdminCheckStatus"))
        {
            query = query
                .Where(t => t.AdminCheckStatus.ToString().Contains(request.KeyWord))
                .OrderByDescending(t => t.Created);
        }
        else if (request.Filter.Equals("TransactionType"))
        {
            query = query
                .Where(t => t.TransactionType.Contains(request.KeyWord))
                .OrderByDescending(t => t.Created);
        }
        else if (request.Filter.Equals("From"))
        {
            var fromUserIds = _beatSportsDbContext.Wallets
                             .Include(x => x.Account)
                             .Where(x => (x.Account.FirstName + " " + x.Account.LastName).Contains(request.KeyWord))
                             .Select(x => x.Id)
                             .ToList();

            query = query
                .Where(t => fromUserIds.Contains(t.WalletId))
                .OrderByDescending(t => t.Created);
        }
        else if (request.Filter.Equals("To"))
        {
            var toUserIds = _beatSportsDbContext.Wallets
                             .Include(x => x.Account)
                             .Where(x => (x.Account.FirstName + " " + x.Account.LastName).Contains(request.KeyWord))
                             .Select(x => x.Id)
                             .ToList();

            query = query
                .Where(t => toUserIds.Contains(t.WalletTargetId))
                .OrderByDescending(t => t.Created);
        }

        // Implementing pagination
        var totalCount = await query.CountAsync(cancellationToken);
        var pagedResult = await query
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var result = new List<TransactionResponseV2>();

        foreach (var transaction in pagedResult)
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

        var paginatedResponse = new PaginatedTransactionResponse
        {
            PageNumber = request.PageIndex,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            TotalCount = totalCount,
            Items = result
        };

        return paginatedResponse;
    }
}