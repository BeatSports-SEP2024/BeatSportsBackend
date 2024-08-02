using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Dashboards.GetDashboard;
public class GetDashboardHandler : IRequestHandler<GetDashboardCommand, List<DashboardResponse>>
{
    private readonly IBeatSportsDbContext _dbContext;

    public GetDashboardHandler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<DashboardResponse>> Handle(GetDashboardCommand request, CancellationToken cancellationToken)
    {
        var response = new DashboardResponse();

        // Tổng số tiền Customer nạp
        response.TotalAmountCustomer = await _dbContext.Transactions
            .Where(t => t.TransactionType == "Nạp tiền")
            .SumAsync(t => t.TransactionAmount, cancellationToken);

        // Tổng số tiền Owner rút
        response.TotalAmountOwnerWithdrawal = await _dbContext.Transactions
            .Where(t => t.TransactionType == "Rút tiền")
            .SumAsync(t => t.TransactionAmount, cancellationToken);

        var accounts = await _dbContext.Accounts.ToListAsync(cancellationToken);

        // Thực hiện nhóm và tính toán dữ liệu trên client
        response.Dashboard = accounts
            .GroupBy(u => new
            {
                Year = u.Created.Year,
                Month = u.Created.Month
            })
            .Select(g => new DashboardData
            {
                X = new DateTime(g.Key.Year, g.Key.Month, 1),
                Y1 = g.Count(u => u.Role == "Customer"),
                Y2 = g.Count(u => u.Role == "Owner")
            })
            .OrderBy(d => d.X)
            .ToList();

        var listResponse = new List<DashboardResponse> { response };

        return listResponse;
    }
}