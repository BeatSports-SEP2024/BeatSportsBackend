using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.TimePeriod.Command.CreateTimePeriod;
public class CreateTimePeriodCommand : IRequest<BeatSportsResponse>
{
    public string Description { get; set; }
    public TimeSpan MinCancellationTime { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    [Required]
    public Guid CourtId { get; set; }
    /// <summary>
    /// Xác định nó áp dụng cho setting nào -> setting sẽ áp dụng cho nguyên list sân nhỏ của nó ứng với sân lớn và setting
    /// </summary>
    public List<Guid> ListCourtSettingId { get; set; } = null!;
    /// <summary>
    /// Xác định xem là ngày thường hay ngày đặc biệt
    /// </summary>
    public bool IsNormalDay { get; set; }
    /// <summary>
    /// Ngày đặc biệt thì có start và end
    /// </summary>
    public DateTime? DayStartTimePeriod { get; set; }
    /// <summary>
    /// Ngày đặc biệt thì có start và end
    /// </summary>
    public DateTime? DayEndTimePeriod { get; set; } 
    /// <summary>
    /// Ngày thường thì xác định bằng những thứ trong tuần: Thứ 2, Thứ 3,...
    /// </summary>
    public List<string>? ListDayInWeek { get; set; }
    /// <summary>
    /// Giờ nó sẽ là giá cộng thêm hoặc giảm bớt tùy ý
    /// Price Adjustment : Điêu chỉnh giá cả tăng hay giảm
    /// </summary>
    public decimal PriceAdjustment { get; set; }
}