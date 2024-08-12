using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.PayFeeMonthlyForOwner;
public class PayFeeMonthlyForOwnerHandler : IRequestHandler<PayFeeMonthlyForOwnerCommand, BeatSportsResponseV2>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public PayFeeMonthlyForOwnerHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<BeatSportsResponseV2> Handle(PayFeeMonthlyForOwnerCommand request, CancellationToken cancellationToken)
    {
        if (request.FeeMonthlyForOwner < 70000 || request.FeeMonthlyForOwner % 70000 != 0)
        {
            throw new BadRequestException("Số tiền phí không đúng với thực tế");
        }

        var owner = _beatSportsDbContext.Owners
                    .Where(x => x.Id == request.OwnerId)
                    .FirstOrDefault();

        if (owner == null)
        {
            return await Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Owner không tồn tại"
            });
        }

        var ownerWallet = _beatSportsDbContext.Wallets
                            .Where(x => x.AccountId == owner.AccountId)
                            .FirstOrDefault();

        if (ownerWallet.Balance < request.FeeMonthlyForOwner)
        {
            return await Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Vui lòng duy trì số dư tối thiểu 70000VND"
            });
        }

        // Kiểm tra tháng này owner đó đã thu chưa, thu rồi thì không thu nữa
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        var hasPaidForCurrentMonth = _beatSportsDbContext.Transactions
        .Any(x => x.WalletId == ownerWallet.Id
                  && x.TransactionType == TransactionEnum.Payfee.ToString()
                  && x.TransactionDate.Value.Month == currentMonth
                  && x.TransactionDate.Value.Year == currentYear);

        if (hasPaidForCurrentMonth)
        {
            return new BeatSportsResponseV2
            {
                Status = 400,
                Message = $"Phí dịch vụ đã được thu cho tháng {currentMonth}"
            };
        }

        var transaction = new Transaction()
        {
            WalletId = ownerWallet.Id,
            TransactionMessage = "Đã thanh toán phí cho tháng này",
            TransactionStatus = TransactionEnum.Approved.ToString(),
            AdminCheckStatus = AdminCheckEnums.Accepted,
            TransactionAmount = request.FeeMonthlyForOwner,
            TransactionDate = DateTime.Now,
            TransactionType = TransactionEnum.Payfee.ToString(),
        };

        _beatSportsDbContext.Transactions.Add(transaction);

        ownerWallet.Balance -= (int)request.FeeMonthlyForOwner;

        _beatSportsDbContext.Wallets.Update(ownerWallet);
        _beatSportsDbContext.SaveChanges();

        return await Task.FromResult(new BeatSportsResponseV2
        {
            Status = 200,
            Message = "Đã thu phí cho tháng hiện tại thành công"
        });
    }
}