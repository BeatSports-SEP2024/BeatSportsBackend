using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Dashboards.GetDashboard;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Dashboards
{
    public class GetDashboardHandler : IRequestHandler<GetDashboardCommand, DashboardResponse>
    {
        private readonly IBeatSportsDbContext _dbContext;

        public GetDashboardHandler(IBeatSportsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardResponse> Handle(GetDashboardCommand request, CancellationToken cancellationToken)
        {
            // Lấy tổng tiền đặt sân đã duyệt
            var totalBookingMoneyInApp = await _dbContext.Bookings
                .Where(b => b.BookingStatus == "Approved")
                .SumAsync(b => b.TotalAmount, cancellationToken);

            // Lấy tổng số chủ sân
            var totalOwner = await _dbContext.Accounts
                .CountAsync(a => a.Role == "Owner", cancellationToken);

            // Lấy tổng số khách hàng
            var totalCustomer = await _dbContext.Accounts
                .CountAsync(a => a.Role == "Customer", cancellationToken);

            // Lấy số liệu đăng ký theo tháng
            var registrationData = await _dbContext.Accounts
                .Where(a => a.Role == "Customer")
                .Select(a => new { a.Created.Year, a.Created.Month })
                .ToListAsync(cancellationToken); 

            var groupedData = registrationData
                .GroupBy(a => new { a.Year, a.Month })
                .Select(g => new DashboardData
                {
                    X = new DateTime(g.Key.Year, g.Key.Month, 1),
                    Y = g.Count()
                })
                .OrderBy(d => d.X)
                .ToList();  

            // Tạo các response object
            var combinedResponse = new DashboardResponse
            {
                RevenueDashboard = new DashboardRevenue
                {
                    TotalBookingMoneyInApp = totalBookingMoneyInApp,
                    Dashboard = groupedData
                },
                OwnerDashboard = new DashboardTotalOwner
                {
                    TotalOwner = totalOwner,
                    Dashboard = groupedData
                },
                CustomerDashboard = new DashboardTotalCustomer
                {
                    TotalCustomer = totalCustomer,
                    Dashboard = groupedData
                }
            };

            return combinedResponse;
        }
    }
}