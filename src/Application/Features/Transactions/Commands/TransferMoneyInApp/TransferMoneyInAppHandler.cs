using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Courts.Commands.CreateCourt;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.TransferMoneyInApp;
public class TransferMoneyInAppHandler : IRequestHandler<TransferMoneyInAppCommand, BeatSportsResponseV2>
{
    private readonly IBeatSportsDbContext _dbContext;

    public TransferMoneyInAppHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponseV2> Handle(TransferMoneyInAppCommand request, CancellationToken cancellationToken)
    {
        var customer = _dbContext.Customers
                    .Where(x => x.Id == request.CustomerId)
                    .FirstOrDefault();
                        
        var customerWallet = _dbContext.Wallets
                            .Where(x => x.AccountId == customer.AccountId)
                            .FirstOrDefault();

        var owner = _dbContext.Owners
                    .Where(x => x.Id == request.OwnerId)
                    .FirstOrDefault();

        var ownerWallet = _dbContext.Wallets
                            .Where(x => x.AccountId == owner.AccountId)
                            .FirstOrDefault();

        if(customerWallet.Balance < request.TransactionAmount)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Chuyển khoản thất bại, số dư không đủ để thực hiện!"
            });
        }

        var transaction = new Domain.Entities.Transaction()
        {
            WalletId = customerWallet.Id,
            WalletTargetId = ownerWallet.Id,
            TransactionMessage = "Chuyển khoản thành công",
            TransactionStatus = "0",
            AdminCheckStatus = AdminCheckEnums.Pending,
            TransactionAmount = request.TransactionAmount,
            TransactionDate = DateTime.Now,
            TransactionType = "Giao dịch trong App",
        };

        _dbContext.Transactions.Add(transaction);
        _dbContext.SaveChanges();

        customerWallet.Balance -= (int)request.TransactionAmount;

        _dbContext.Wallets.Update(customerWallet);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponseV2
        {
            Status = 200,
            Message = "Thanh toán thành công!"
        });
    }
}
