using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.OwnerPayFeeOverview;
public class OwnerPayFeeOverviewHandler : IRequestHandler<OwnerPayFeeOverviewCommand, OwnerPayFeeOverviewResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public OwnerPayFeeOverviewHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<OwnerPayFeeOverviewResponse> Handle(OwnerPayFeeOverviewCommand request, CancellationToken cancellationToken)
    {
        string monthAndYear = request.MonthYear;

        // Split string to get month and year
        var parts = monthAndYear.Split('/');
        int month = int.Parse(parts[0]);
        int year = int.Parse(parts[1]);

        // Lấy tất cả các giao dịch trong tháng/năm đã chỉ định và không bị xóa
        var transactions = await _beatSportsDbContext.Transactions
            .Include(c => c.Wallet)
            .ThenInclude(c => c.Account)
            .ThenInclude(c => c.Owner)
            .Where(transaction => !transaction.IsDelete
                                  && transaction.TransactionDate.HasValue
                                  && transaction.TransactionDate.Value.Month == month
                                  && transaction.TransactionDate.Value.Year == year)
            .ToListAsync();

        var activeOwners = await _beatSportsDbContext.Owners
            .Include(o => o.Account)
                    .Where(o => !o.Account.IsDelete
                    && o.Created.Month <= month
                    && o.Created.Year == year
                    )
            .ToListAsync();

        var paidOwners = new List<PaidOwner>();
        var unpaidOwners = new List<UnpaidOwner>();

        // Phân loại các chủ sở hữu thành đã thanh toán và chưa thanh toán
        foreach (var owner in activeOwners)
        {
            var ownerTransactions = transactions
                .Where(transaction =>
                       transaction.Wallet?.Account?.Owner?.Id == owner.Id
                       && transaction.TransactionType == TransactionEnum.Payfee.ToString())
                .ToList();

            if (ownerTransactions == null || !ownerTransactions.Any())
            {
                // Chưa thanh toán
                unpaidOwners.Add(new UnpaidOwner
                {
                    OwnerAccount = owner.Account.UserName,
                    OwnerId = owner.Id,
                    OwnerName = $"{owner.Account.FirstName} {owner.Account.LastName}",
                    TotalFeeNeedToPaid = 70000,
                    TransactionDate = new DateTime(1, month, year)
                });
            }

            if (ownerTransactions.Any())
            {
                // Đã thanh toán
                var totalFeePaid = ownerTransactions.Sum(t => t.TransactionAmount);
                var lastTransactionDate = ownerTransactions.Max(t => t.TransactionDate);

                paidOwners.Add(new PaidOwner
                {
                    OwnerAccount = owner.Account.UserName, 
                    OwnerId = owner.Id,
                    OwnerName = $"{owner.Account.FirstName} {owner.Account.LastName}",
                    TotalFeePaid = 70000,
                    TransactionDate = lastTransactionDate
                });
            }
            else
            {
                // Chưa thanh toán
/*                unpaidOwners.Add(new UnpaidOwner
                {
                    OwnerAccount = owner.Account.UserName,
                    OwnerId = owner.Id,
                    OwnerName = $"{owner.Account.FirstName} {owner.Account.LastName}",
                    TotalFeeNeedToPaid = 70000, 
                    TransactionDate = null
                });*/
            }
        }

        // Tính tổng tiền đã thanh toán trong tháng hiện tại
        var totalPaidThisMonth = transactions
            .Where(transaction => transaction.TransactionType == TransactionEnum.Payfee.ToString()
                                  && transaction.TransactionDate.Value.Month == month
                                  && transaction.TransactionDate.Value.Year == year
                                  && !transaction.IsDelete)
            .Sum(transaction => transaction.TransactionAmount);

        // Tính tổng tiền admin đã nhận từ trước đến giờ
        var totalPaidOverall = await _beatSportsDbContext.Transactions
            .Where(transaction => !transaction.IsDelete && transaction.TransactionType == TransactionEnum.Payfee.ToString())
            .SumAsync(transaction => transaction.TransactionAmount);

        var totalNotPaidFee = unpaidOwners.Count * 70000;

        // Trả về kết quả
        return new OwnerPayFeeOverviewResponse
        {
            TotalActiveOwners = activeOwners.Count,
            TotalPaidThisMonth = (decimal)totalPaidThisMonth,
            TotalPaidOverall = (decimal)totalPaidOverall,
            TotalNotPaidFee = totalNotPaidFee,
            PaidOwnerList = paidOwners,
            UnpaidOwnerList = unpaidOwners
        };
    }
}