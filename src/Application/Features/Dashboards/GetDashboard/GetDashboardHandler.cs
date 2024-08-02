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
                .Where(b => b.BookingStatus == "Approved" && !b.IsDelete && b.BookingDate.Year == request.Year)
                .SumAsync(b => b.TotalAmount, cancellationToken);

            // Lấy tổng số chủ sân
            var totalOwner = await _dbContext.Accounts
                .CountAsync(a => a.Role == "Owner" && !a.IsDelete && a.Created.Year == request.Year, cancellationToken);

            // Lấy tổng số khách hàng
            var totalCustomer = await _dbContext.Accounts
                .CountAsync(a => a.Role == "Customer" && !a.IsDelete && a.Created.Year == request.Year, cancellationToken);

            // Khởi tạo dữ liệu cho tất cả các tháng cho từng Dashboard
            var allMonthsCustomer = Enumerable.Range(1, 12).Select(month => new DashboardData
            {
                X = new DateTime(request.Year, month, 1),
                Y = 0
            }).ToList();

            var allMonthsOwner = Enumerable.Range(1, 12).Select(month => new DashboardData
            {
                X = new DateTime(request.Year, month, 1),
                Y = 0
            }).ToList();

            var allMonthsRevenue = Enumerable.Range(1, 12).Select(month => new DashboardData
            {
                X = new DateTime(request.Year, month, 1),
                Y = 0
            }).ToList();


            // Lấy dữ liệu đăng ký theo tháng từ cơ sở dữ liệu
            var registrationData = await _dbContext.Accounts
                .Where(a => a.Role == "Customer" && a.Created.Year == request.Year && !a.IsDelete)
                .GroupBy(a => new { a.Created.Year, a.Created.Month })
                .Select(g => new { g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Lấy dữ liệu doanh thu theo từng tháng 
            var revenueByMonth = await _dbContext.Bookings
                .Where(b => b.BookingStatus == "Approved" && !b.IsDelete && b.BookingDate.Year == request.Year)
                .GroupBy(b => new { Year = b.BookingDate.Year, Month = b.BookingDate.Month })
                .Select(g => new
                {
                    g.Key.Month,
                    TotalRevenue = g.Sum(b => b.TotalAmount) 
                })
                .ToListAsync(cancellationToken);

            // Lấy dữ liệu chủ sân đăng kí theo từng tháng
            var ownerRegisterByMonth = await _dbContext.Accounts
                .Where(a => a.Role == "Owner" && a.Created.Year == request.Year && !a.IsDelete)
                .GroupBy(a => new { a.Created.Year, a.Created.Month })
                .Select(g => new { g.Key.Month, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Cập nhật danh sách allMonths với dữ liệu từ cơ sở dữ liệu
            foreach (var data in registrationData)
            {
                var monthData = allMonthsCustomer.FirstOrDefault(m => m.X.Month == data.Month);
                if (monthData != null)
                {
                    monthData.Y = data.Count;
                }
            }

            // Cập nhật danh sách allMonthsOwner với dữ liệu đăng ký chủ sân
            foreach (var data in ownerRegisterByMonth)
            {
                var monthData = allMonthsOwner.FirstOrDefault(m => m.X.Month == data.Month);
                if (monthData != null)
                {
                    monthData.Y = data.Count;
                }
            }

            // Cập nhật danh sách allMonthsRevenue với dữ liệu doanh thu
            foreach (var data in revenueByMonth)
            {
                var monthData = allMonthsRevenue.FirstOrDefault(m => m.X.Month == data.Month);
                if (monthData != null)
                {
                    monthData.Y = data.TotalRevenue;
                }
            }

            var combinedResponse = new DashboardResponse
            {
                RevenueDashboard = new DashboardRevenue
                {
                    TotalBookingMoneyInApp = totalBookingMoneyInApp,
                    Dashboard = allMonthsRevenue
                },
                OwnerDashboard = new DashboardTotalOwner
                {
                    TotalOwner = totalOwner,
                    Dashboard = allMonthsOwner
                },
                CustomerDashboard = new DashboardTotalCustomer
                {
                    TotalCustomer = totalCustomer,
                    Dashboard = allMonthsCustomer
                }
            };

            return combinedResponse;
        }
    }
}