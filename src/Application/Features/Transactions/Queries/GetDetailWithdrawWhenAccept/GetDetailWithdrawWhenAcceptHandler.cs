using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetDetailWithdrawWhenAccept;
public class GetDetailWithdrawWhenAcceptHandler : IRequestHandler<GetDetailWithdrawWhenAcceptCommand, GetDetailWithdrawWhenAcceptResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetDetailWithdrawWhenAcceptHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<GetDetailWithdrawWhenAcceptResponse> Handle(GetDetailWithdrawWhenAcceptCommand request, CancellationToken cancellationToken)
    {
        var transaction = _beatSportsDbContext.Transactions
            .Include(c => c.Wallet)
                .ThenInclude(a => a.Account)
                    .ThenInclude(o => o.Owner)
            .Where(transaction => transaction.Id == request.TransactionId && transaction.TransactionType.Equals("Rút tiền"))
            .FirstOrDefault();

        if (transaction == null)
        {
            throw new BadRequestException("Khong tim thay transaction theo yeu cau");
        }
        var response = new GetDetailWithdrawWhenAcceptResponse()
        {
            TransactionId = transaction.Id,
            TransactionStatus = transaction.TransactionStatus,
            OwnerInfo = new UserInfo2
            {
                OwnerId = transaction.Wallet.Account.Owner.Id,
                Name = transaction.Wallet.Account.FirstName + " " + transaction.Wallet.Account.LastName,
                OwnerBankAccount = transaction.Wallet.Account.Owner.BankAccount,
                Role = transaction.Wallet.Account.Role,
                WalletId = transaction.Wallet.Id,
            },
            AdminCheckStatus = transaction.AdminCheckStatus.ToString(),
            ImageOfInvoice = transaction.ImageOfInvoice,
            TransactionDate = transaction.TransactionDate,
            TotalAmountReceived = transaction.TransactionAmount,
        };

        return response;
    }
}