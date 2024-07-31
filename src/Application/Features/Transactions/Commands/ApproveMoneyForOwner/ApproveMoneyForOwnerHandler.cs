using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Transactions.Commands.TransferMoneyInApp;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.ApproveMoneyForOwner;
public class ApproveMoneyForOwnerHandler : IRequestHandler<ApproveMoneyForOwnerCommand, BeatSportsResponseV2>
{
    private readonly IBeatSportsDbContext _dbContext;

    public ApproveMoneyForOwnerHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponseV2> Handle(ApproveMoneyForOwnerCommand request, CancellationToken cancellationToken)
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

        if(transaction.Created.Date.AddDays(7) > DateTime.Now.Date)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Chua den ngay chuyen tien!"
            });
        }

        var owner = _dbContext.Owners
                    .Where(x => x.Id == request.OwnerId)
                    .FirstOrDefault();

        var ownerWallet = _dbContext.Wallets
                            .Where(x => x.AccountId == owner.AccountId)
                            .FirstOrDefault();

        if(transaction.WalletTargetId != ownerWallet.Id)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Chuyển khoản thất bại, ownerId không trùng khớp vói hóa đơn!"
            });
        }

        transaction.AdminCheckStatus = AdminCheckEnums.Accepted;

        _dbContext.Transactions.Update(transaction);
        _dbContext.SaveChanges();

        ownerWallet.Balance += (int)transaction.TransactionAmount;

        _dbContext.Wallets.Update(ownerWallet);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponseV2
        {
            Status = 200,
            Message = "Chuyển tiền cho owner thành công!"
        });
    }
}
