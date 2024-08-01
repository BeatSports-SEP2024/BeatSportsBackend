using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionByOwner;
public class GetAllTransactionByOwnerHandler : IRequestHandler<GetAllTransactionByOwner, PaginatedList<TransactionResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetAllTransactionByOwnerHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<PaginatedList<TransactionResponse>> Handle(GetAllTransactionByOwner request, CancellationToken cancellationToken)
    {
        var query = _beatSportsDbContext.Transactions
            .Include(t => t.Wallet)
                .ThenInclude(w => w.Account)
                    .ThenInclude(a => a.Owner)
            .Where(t => t.Wallet.Account.Owner.Id == request.OwnerId && 
            (t.TransactionType == "Giao dịch trong App" || t.TransactionType == "Rút tiền"));

        var response = await query.Select(t => new TransactionResponse
        {
            TransactionId = t.Id,
            WalletId = t.WalletId,
            WalletTargetId = t.WalletTargetId,
            TransactionMessage = t.TransactionMessage,
            TransactionPayload = t.TransactionPayload,
            TransactionStatus = t.TransactionStatus.ToString(),
            AdminCheckStatus = t.AdminCheckStatus.ToString(),
            TransactionAmount = t.TransactionAmount,
            TransactionDate = t.TransactionDate,
            TransactionType = t.TransactionType
        }).PaginatedListAsync(request.PageIndex, request.PageSize);
       
        return response;
    }
}