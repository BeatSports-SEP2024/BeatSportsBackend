using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.CreateBooking;
public class CreateBookingCommand : IRequest<BeatSportsResponse>
{
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
    public Guid CampaignId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    //public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsRoomBooking { get; set; }
    public bool IsDeposit { get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    //public string BookingStatus { get; set; }
}
