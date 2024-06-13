using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionsByCustomer;
public class GetAllTransactionByAccountHandler : IRequestHandler<GetAllTransactionByAccountCommand, PaginatedList<TransactionResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetAllTransactionByAccountHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<PaginatedList<TransactionResponse>> Handle(GetAllTransactionByAccountCommand request, CancellationToken cancellationToken)
    {
        IQueryable<Transaction> query = _beatSportsDbContext.Transactions
        .Include(t => t.Wallet)
            .ThenInclude(w => w.Account)
                .Where(t => t.Wallet.Account.Id == request.AccountId);

        // Kiểm tra nếu không tìm thấy transactions       
        if (query == null)
        {
            throw new NotFoundException($"{request.AccountId} may not have any transactions");
        }

        // Chuyển đổi kết quả query sang TransactionResponse
        var transactionResponses = query.Select(c => new TransactionResponse
        {
            AccountId = c.Wallet.Account.Id,
            WalletId = c.Wallet.Id,
            TransactionMessage = c.TransactionMessage,
            TransactionPayload = c.TransactionPayload,
            TransactionStatus = c.TransactionStatus,
            TransactionAmount = c.TransactionAmount,
            TransactionDate = c.TransactionDate,
            TransactionType = c.TransactionType,
        }).PaginatedListAsync(request.PageIndex, request.PageSize);

        return transactionResponses;
    }
}