using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Exceptions;
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
        if (request.TransactionAmount < 0)
        {
            throw new BadRequestException("Số tiền yêu cầu rút phải lớn hơn 0");
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

        if (ownerWallet.Balance < request.TransactionAmount)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Yêu cầu rút tiền thất bại, số dư không đủ để thực hiện!"
            });
        }

        // Chặn rút tiền nếu số dư còn lại không đủ
        if (ownerWallet.Balance - request.TransactionAmount < 70000)
        {
            decimal withdrawableAmount = Math.Max(request.TransactionAmount - 70000, 0);
            string formattedWithdrawableAmount = withdrawableAmount.ToString("N0");

            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = $"Số dư tối thiểu trong ví không được dưới 70.000VND, số dư có thể rút {formattedWithdrawableAmount} VND"
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