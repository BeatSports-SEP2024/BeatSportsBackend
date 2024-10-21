using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Commands.DeleteBooking;
public class DeleteBookingCommand : IRequest<BeatSportsResponse>
{
    public Guid BookingId { get; set; }
}