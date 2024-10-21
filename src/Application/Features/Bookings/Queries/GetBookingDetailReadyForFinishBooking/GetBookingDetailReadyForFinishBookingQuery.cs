using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailReadyForFinishBooking;
public class GetBookingDetailReadyForFinishBookingQuery : IRequest<BookingDetailReadyForFinishBookingResponse>
{
    public Guid CustomerId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    /// <summary>
    /// Format hh:mm:ss
    /// </summary>
    public TimeSpan StartTimeWantToPlay { get; set; }
    /// <summary>
    /// Format hh:mm:ss
    /// </summary>
    public TimeSpan EndTimeWantToPlay { get; set; }
    /// <summary>
    /// Format yyyy-MM-dd
    /// </summary>
    public DateTime DayWantToPlay { get; set; }
}

public class BookingDetailReadyForFinishBookingResponse
{
    /// <summary>
    /// tạo thành công r thì trả nó lên, để lúc xác nhận thì gửi xuống lại
    /// </summary>
    public Guid BookingId { get; set; }
    public Guid CourtId { get; set; }
    public string? CourtName { get; set; }
    public string? ImageCourtURL { get; set; }
    public List<CourtDetailInBookingDetailReadyForFinishBookingReponse>? ListCourtByTimePeriod { get; set; }
    public decimal TotalPrice { get; set; }

}
public class CourtDetailInBookingDetailReadyForFinishBookingReponse
{

    // Chõ này dùng string cho FE dễ check có tồn tại hay không
    public string? TimePeriodId { get; set; }
    public string? TimePeriodDescription { get; set; }
    public decimal PriceInTimePeriod { get; set; }
}
