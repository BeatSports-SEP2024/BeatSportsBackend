using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetIncomeByBookingByCourtId;
/// <summary>
/// Orderby ngược lại dùm t
/// từ 31 -> 30 -> ... nói chung order by mới nhất về cũ nhất
/// Get all những booking đã trong trạng thái accept
/// </summary>
public class GetIncomeByBookingByCourtIdQuery : IRequest<List<IncomeByBookingResponse>>
{
    public Guid CourtId { get; set; }
    /// <summary>
    /// trong so sánh nhớ có >= hoặc <=, đừng dùng mỗi dấu > <
    /// </summary>
    public DateTime DayStart { get; set; }
    public DateTime DayEnd { get; set; }
}
public class IncomeByBookingResponse
{
    /// <summary>
    /// Count ngày
    /// Int i++ theo ngày bởi vì kh có cố định
    /// </summary>
    public int IdFlag { get; set; }
    /// <summary>
    /// Ngày check
    /// Format ra string yyyy-MM-dd hh:mm:ss
    /// </summary>
    public string? DateCheck { get; set; }
    /// <summary>
    /// Tổng tiền của ngày hôm đó đối với những booking có transaction đã được check bởi admin
    /// </summary>
    public decimal? TotalPriceOfDayWasCheckedByAdmin { get; set; }
    /// <summary>
    /// Tổng tiền của ngày hôm đó đối với những booking có transaction đang process, chưa được check bởi admin
    /// </summary>
    public decimal? TotalPriceOfDayProcessing { get; set; }
    public List<BookingOfCourtInDayV2>? ListBooked { get; set; }
}
public class BookingOfCourtInDayV2
{
    public Guid BookingId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    public Guid TransactionId { get; set; }
    /// <summary>
    /// Trả status ra cho t
    /// </summary>
    public string AdminCheckStatus { get; set; } = null!;
    public Guid CustomerId { get; set; }
    /// <summary>
    /// Full name của người chơi
    /// </summary>
    public string? FullNameOfCustomer { get; set; }
    public string? CourtSubdivisionName { get; set; }
    /// <summary>
    /// Ngày đặt
    /// Format ra string yyyy-MM-dd hh:mm:ss
    /// </summary>
    public string? DayTimeBooking { get; set; }
    /// <summary>
    /// Tổng giá tiền
    /// </summary>
    public decimal TotalPrice { get; set; }
}
