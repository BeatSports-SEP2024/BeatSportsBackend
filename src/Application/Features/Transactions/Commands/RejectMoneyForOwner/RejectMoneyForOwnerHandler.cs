using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Transactions.Commands.ApproveMoneyForOwner;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.RejectMoneyForOwner;
public class RejectMoneyForOwnerHandler : IRequestHandler<RejectMoneyForOwnerCommand, BeatSportsResponseV2>
{
    private readonly IBeatSportsDbContext _dbContext;

    public RejectMoneyForOwnerHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<BeatSportsResponseV2> Handle(RejectMoneyForOwnerCommand request, CancellationToken cancellationToken)
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

        transaction.AdminCheckStatus = AdminCheckEnums.Cancel;
        transaction.ReasonOfRejected = request.ReasonOfRejected;

        _dbContext.Transactions.Update(transaction);
        _dbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponseV2
        {
            Status = 200,
            Message = "Reject hóa đơn thành công!"
        });
    }
}
