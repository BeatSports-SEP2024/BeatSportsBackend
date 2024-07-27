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
public class CreateBookingCommand : IRequest<BookingSuccessResponse>
{
    public Guid BookingId { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    public decimal TotalAmount { get; set; }
}
