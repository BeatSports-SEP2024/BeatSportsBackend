using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.CourtEntity.TimePeriod;
public class TimePeriod : BaseAuditableEntity
{
    /// <summary>
    /// Thời gian tối thiểu cần để hủy sân
    /// Nếu minCancellationTime được đặt là 6, 
    /// người dùng cần phải hủy sân ít nhất 6 giờ trước giờ bắt đầu. 
    /// Nếu họ cố gắng hủy sân trong phạm vi 6 giờ trước giờ bắt đầu,
    /// việc hủy sẽ không được chấp nhận.
    /// </summary>
    public TimeSpan MinCancellationTime { get; set; }
    public string? Description { get; set; }
    /// <summary>
    /// Bắt đầu khung giờ
    /// </summary>
    public TimeSpan StartTime { get; set; }
    /// <summary>
    /// Kết thúc khung giờ
    /// </summary>
    public TimeSpan EndTime { get; set; }
    /// <summary>
    /// Tỉ giá so với giá gốc
    /// </summary>
    public decimal? PriceAdjustment { get; set; }
    /// <summary>
    /// Xác định xem nó cho ngày thường hay chuỗi ngày đặc biệt
    /// </summary>
    public bool IsNormalDay { get; set; }
    /// <summary>
    /// List day được gộp lại thành string để check 0: chủ nhật, 1: thứ 2, ...
    /// </summary>
    public string? ListDayByString { get; set; }
    /// <summary>
    /// Nếu kh phải ngày thường mới lưu những cái này
    /// </summary>
    public DateTime? StartDayApply { get; set; }
    /// <summary>
    /// Nếu kh phải ngày thường
    /// </summary>
    public DateTime? EndDayApply { get; set; }
    public IList<TimePeriodCourtSubdivision> TimePeriodCourtSubdivisions { get; set; }
}
