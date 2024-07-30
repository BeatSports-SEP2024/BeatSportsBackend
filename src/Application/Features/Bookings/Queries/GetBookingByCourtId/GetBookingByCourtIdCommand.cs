using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingByCourtId;
public class GetBookingByCourtIdCommand : IRequest<List<GetBookingByCourtIdResponse>>
{
    public Guid CourtId { get; set; }
}