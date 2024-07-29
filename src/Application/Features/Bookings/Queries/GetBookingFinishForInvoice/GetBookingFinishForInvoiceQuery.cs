namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingFinishForInvoice;
public class GetBookingFinishForInvoiceQuery
{
    public Guid CourtId { get; set; }
    public DateTime DayStart { get; set; }
    public DateTime DayEnd { get; set; }
}
public class BookingFinishForInvoiceResponse
{
    /// <summary>
    /// Count ngày
    /// </summary>
    public int IdFlag { get; set; }
    /// <summary>
    /// Ngày check
    /// </summary>
    public string DateCheck { get; set; }
}
public class BookingOfCourtInDay
{
    public Guid BookingId { get; set; }
    /// <summary>
    /// Ngày đặt
    /// </summary>
    public string DayTimeBooking { get; set; }
    /// <summary>
    /// Tổng giá tiền
    /// </summary>
    public decimal TotalPrice { get; set; }
    /// <summary>
    /// Full name của người chơi
    /// </summary>
    public string FullNameOfUser { get; set; }
}
