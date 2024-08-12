using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Response;

#region Main Dashboard
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
#endregion
#region SubDashboard
public class PaidOwner
{
    public string? OwnerAccount { get; set; }
    public Guid OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public decimal? TotalFeePaid { get; set; }
    public DateTime? TransactionDate { get; set; }
}

public class UnpaidOwner
{
    public string? OwnerAccount { get; set; }
    public Guid OwnerId { get; set; }
    public string? OwnerName { get; set; }
    public decimal? TotalFeeNeedToPaid { get; set; }
    public DateTime? TransactionDate { get; set; }
}
public class OwnerPayFeeOverviewResponse
{
    public int TotalActiveOwners { get; set; }
    /// <summary>
    /// Tổng tiền mà owner đã đóng trong tháng đó
    /// </summary>
    public decimal TotalPaidThisMonth { get; set; }
    /// <summary>
    /// Tổng tiền đã thu từ trước tới giờ
    /// </summary>
    public decimal TotalPaidOverall { get; set; }
    /// <summary>
    /// Tổng số tiền mà các owner chưa thanh toán
    /// </summary>
    public decimal TotalNotPaidFee { get; set; }
    public List<PaidOwner>? PaidOwnerList { get; set; }
    public List<UnpaidOwner>? UnpaidOwnerList { get; set; }
}
#endregion