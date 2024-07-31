using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.ApproveWithdrawalRequestByOwner;
public class ApproveWithdrawalRequestByOwnerHandler : IRequestHandler<ApproveWithdrawalRequestByOwnerCommand, BeatSportsResponseV2>
{
    private readonly IBeatSportsDbContext _dbContext;

    public ApproveWithdrawalRequestByOwnerHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<BeatSportsResponseV2> Handle(ApproveWithdrawalRequestByOwnerCommand request, CancellationToken cancellationToken)
    {
        var transaction = _dbContext.Transactions
                        .Where(x => x.Id == request.TransactionId && x.AdminCheckStatus == AdminCheckEnums.Pending)
                        .FirstOrDefault();

        if (transaction == null)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Transaction không tồn tại!"
            });
        }

        var owner = _dbContext.Owners
                    .Where(x => x.Id == request.OwnerId)
                    .FirstOrDefault();

        if (owner == null)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Owner không tồn tại"
            });
        }

        var ownerWallet = _dbContext.Wallets
                            .Where(x => x.AccountId == owner.AccountId)
                            .FirstOrDefault();

        if (transaction.WalletId != ownerWallet.Id)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Duyệt đơn thất bại, ownerId không trùng khớp vói hóa đơn!"
            });
        }

        transaction.AdminCheckStatus = AdminCheckEnums.Accepted;

        _dbContext.Transactions.Update(transaction);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponseV2
        {
            Status = 200,
            Message = "Duyệt đơn rút tiền cho owner thành công!"
        });
    }
}
