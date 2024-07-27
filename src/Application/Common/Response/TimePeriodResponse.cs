using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.CourtEntity.TimePeriod;

namespace BeatSportsAPI.Application.Common.Response;

//For Get All Response
public class TimePeriodResponse : IMapFrom<TimePeriod>
{
    public Guid TimePeriodId { get; set; }
    public Guid CourtSubdivisionSettingId { get; set; }
    public string? CourtSubdivisionSettingType { get; set; }
    public string? MinCancellationTime { get; set; }
    public string Description { get; set; }
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
    public bool IsNormalDay { get; set; }
    public string? DayStartApply { get; set; }
    public string? DayEndApply { get; set; }
    public string? StringListDayInWeekApply { get; set; }
    public string? StringListDayInWeekApplyDescription { get; set; }
    public decimal PriceAdjustment { get; set; }
}
public class TimePeriodWithCourtInformationResponse : TimePeriodResponse
{
    public Guid CourtId { get; set; }
    public string? CourtName { get; set; }
    public string? Address { get; set; }
}

