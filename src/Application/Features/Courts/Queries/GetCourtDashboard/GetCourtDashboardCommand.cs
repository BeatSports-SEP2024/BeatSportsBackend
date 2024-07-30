using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDashboard;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetCourtDashboard;
public class GetCourtDashboardCommand : IRequest<List<CourtDashboardResponse>>
{
    public Guid CourtId { get; set; }
    public int Year { get; set; }
    public string SportCategory { get; set; }
}
