using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionByOwner;
public class GetAllTransactionByOwnerHandler : IRequestHandler<GetAllTransactionByOwner, List<TransactionResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetAllTransactionByOwnerHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<List<TransactionResponse>> Handle(GetAllTransactionByOwner request, CancellationToken cancellationToken)
    {
        var owner = _beatSportsDbContext.Owners.Where(x => x.Id == request.OwnerId).FirstOrDefault();
        var wallet = _beatSportsDbContext.Wallets.Where(x => x.AccountId == owner.AccountId).FirstOrDefault();
        var listTransaction = await _beatSportsDbContext.Transactions.Where(x =>
                                                                      // Trường hợp khi ví của owner là đích đến
                                                                      // Tức là nó "Giao dịch trong app
                                                                      (x.WalletTargetId == wallet.Id &&
                                                                      x.TransactionType.Trim() == "Giao dịch trong App" &&
                                                                      x.AdminCheckStatus == AdminCheckEnums.Accepted)
                                                                      ||
                                                                      (x.WalletId == wallet.Id &&
                                                                      x.TransactionType.Trim() == "Rút tiền")
                                                                      ||
                                                                      (x.WalletId == wallet.Id &&
                                                                      x.TransactionType.Trim() == "Payfee"))
            .OrderByDescending(x => x.LastModified).ThenByDescending(x => x.Created).ToListAsync();
        /*        var query = await _beatSportsDbContext.Transactions
                    .Include(t => t.Wallet)
                        .ThenInclude(w => w.Account)
                            .ThenInclude(a => a.Owner)
                    .Where(t => t.Wallet.Account.Owner.Id == request.OwnerId &&
                    ((t.TransactionType.Trim() == "Giao dịch trong App" && t.AdminCheckStatus == AdminCheckEnums.Accepted)
                    || t.TransactionType.Trim() == "Rút tiền"))
                    .OrderByDescending(x => x.LastModified).ToListAsync();
        */
        var response = listTransaction.Select(t => new TransactionResponse
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
        }).ToList();

        return response;
    }
}