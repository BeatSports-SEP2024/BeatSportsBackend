using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingFinishForInvoice;
/// <summary>
/// Get hết những th booking của court id này theo ngày bắt đầu, ngày kết thúc mà FE gửi xuống
/// </summary>
public class GetBookingFinishForInvoiceQuery : IRequest<List<BookingFinishForInvoiceResponse>>
{
    public Guid CourtId { get; set; }
    /// <summary>
    /// trong so sánh nhớ có >= hoặc <=, đừng dùng mỗi dấu > <
    /// </summary>
    public DateTime DayStart { get; set; }
    public DateTime DayEnd { get; set; }
}
public class BookingFinishForInvoiceResponse
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
    public decimal? TotalPriceOfDay { get; set; }
    public List<BookingOfCourtInDay>? ListBooked { get; set; }
}
public class BookingOfCourtInDay
{
    public Guid BookingId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
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
