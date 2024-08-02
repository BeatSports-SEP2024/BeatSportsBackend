using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Response;
public class DashboardResponse
{
    public decimal? TotalAmountCustomer { get; set; }
    public decimal? TotalAmountOwnerWithdrawal { get; set; }
    public List<DashboardData> Dashboard { get; set; }
}

public class DashboardData
{
    public DateTime X { get; set; }
    public int Y1 { get; set; }
    public int Y2 { get; set; }
}