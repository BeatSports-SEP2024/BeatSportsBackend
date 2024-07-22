using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Common.Response;
public class BookingResponse : IMapFrom<Booking>
{   
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    //public Guid RoomMatchId { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsRoomBooking { get; set; }
    public bool IsDeposit { get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string BookingStatus { get; set; }
}

public class BookingByCustomerId : IMapFrom<Booking>
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    //public Guid RoomMatchId { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    public string? CourtSubName { get; set; }
    public string? CourtName { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsRoomBooking { get; set; }
    public bool IsDeposit { get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string BookingStatus { get; set; }
}
