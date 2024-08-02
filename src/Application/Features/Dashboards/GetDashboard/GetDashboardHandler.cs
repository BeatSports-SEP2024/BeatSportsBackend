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

            // Khởi tạo dữ liệu cho tất cả các tháng
            var allMonths = Enumerable.Range(1, 12).Select(month => new DashboardData
            {
                X = new DateTime(request.Year, month, 1),
                Y = 0  
            }).ToList();

            // Lấy dữ liệu đăng ký theo tháng từ cơ sở dữ liệu
            var registrationData = await _dbContext.Accounts
                .Where(a => a.Role == "Customer" && a.Created.Year == request.Year)
                .GroupBy(a => new { a.Created.Year, a.Created.Month })
                .Select(g => new { g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Cập nhật danh sách allMonths với dữ liệu từ cơ sở dữ liệu
            foreach (var data in registrationData)
            {
                var monthData = allMonths.FirstOrDefault(m => m.X.Month == data.Month);
                if (monthData != null)
                {
                    monthData.Y = data.Count;
                }
            }

            // Tạo các response object
            var combinedResponse = new DashboardResponse
            {
                RevenueDashboard = new DashboardRevenue
                {
                    TotalBookingMoneyInApp = totalBookingMoneyInApp,
                    Dashboard = allMonths
                },
                OwnerDashboard = new DashboardTotalOwner
                {
                    TotalOwner = totalOwner,
                    Dashboard = allMonths
                },
                CustomerDashboard = new DashboardTotalCustomer
                {
                    TotalCustomer = totalCustomer,
                    Dashboard = allMonths
                }
            };

            return combinedResponse;
        }
    }
}