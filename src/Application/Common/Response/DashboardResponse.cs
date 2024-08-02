using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Response;

public class DashboardDataInside
{
    public List<DashboardData>? Dashboard { get; set; }
}

public class DashboardData
{
    // Xét theo từng tháng
    public DateTime X { get; set; }
    // Số lượng customer đăng kí trong tháng đó hoặc số tiền hoặc số owner đăng kí
    public object? Y { get; set; }
}

public class DashboardRevenue : DashboardDataInside
{
    public decimal? TotalBookingMoneyInApp { get; set; }   
}

public class DashboardTotalOwner : DashboardDataInside
{
    public int TotalOwner { get; set; }
}

public class DashboardTotalCustomer : DashboardDataInside
{
    public int TotalCustomer { get; set; }
}

public class DashboardResponse
{
    public DashboardRevenue RevenueDashboard { get; set; }
    public DashboardTotalOwner OwnerDashboard { get; set; }
    public DashboardTotalCustomer CustomerDashboard { get; set; }
}