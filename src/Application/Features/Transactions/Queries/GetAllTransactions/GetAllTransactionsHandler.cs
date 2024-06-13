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
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactions;
public class GetAllTransactionsHandler : IRequestHandler<GetAllTransactionsCommand, PaginatedList<TransactionResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllTransactionsHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public Task<PaginatedList<TransactionResponse>> Handle(GetAllTransactionsCommand request, CancellationToken cancellationToken)
    {
        IQueryable<Transaction> query = _beatSportsDbContext.Transactions
            .Where(t => !t.IsDelete);

        var list = query.Select(q => new TransactionResponse
        {
            WalletId = q.WalletId,
            TransactionMessage = q.TransactionMessage,
            TransactionStatus = q.TransactionStatus,
            TransactionDate = q.TransactionDate,
            TransactionAmount = q.TransactionAmount,
            TransactionPayload = q.TransactionPayload,
            TransactionType = q.TransactionType,
        })
            .PaginatedListAsync(request.PageIndex, request.PageSize);
        return list;
    }
}