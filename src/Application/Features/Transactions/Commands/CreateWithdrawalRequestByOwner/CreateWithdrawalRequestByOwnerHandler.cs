using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Transactions.Commands.TransferMoneyInApp;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.CreateWithdrawalRequestByOwner;
public class CreateWithdrawalRequestByOwnerHandler : IRequestHandler<CreateWithdrawalRequestByOwnerCommand, BeatSportsResponseV2>
{
    private readonly IBeatSportsDbContext _dbContext;

    public CreateWithdrawalRequestByOwnerHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponseV2> Handle(CreateWithdrawalRequestByOwnerCommand request, CancellationToken cancellationToken)
    {
       
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

        if (ownerWallet.Balance < request.TransactionAmount)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Yêu cầu rút tiền thất bại, số dư không đủ để thực hiện!"
            });
        }

        // Chặn ở mức 70000VND để tự động thu theo tháng
        if (ownerWallet.Balance < 70000)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Số dư tối thiểu trong ví không được dưới 70000VND"
            });
        }

        var transaction = new Transaction()
        {
            WalletId = ownerWallet.Id,
            TransactionMessage = "Rút tiền thành công",
            TransactionStatus = TransactionEnum.Pending.ToString(),
            AdminCheckStatus = AdminCheckEnums.Pending,
            TransactionAmount = request.TransactionAmount,
            TransactionDate = DateTime.Now,
            TransactionType = "Rút tiền",
        };

        _dbContext.Transactions.Add(transaction);

        ownerWallet.Balance -= (int)request.TransactionAmount;

        _dbContext.Wallets.Update(ownerWallet);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponseV2
        {
            Status = 200,
            Message = "Gửi yêu cầu rút tiền thành công!"
        });
    }
}
