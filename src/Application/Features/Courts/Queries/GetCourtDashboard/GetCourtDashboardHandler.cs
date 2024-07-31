using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDashboard;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetCourtDashboard;
public class GetCourtDashboardHandler : IRequestHandler<GetCourtDashboardCommand, List<CourtDashboardResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetCourtDashboardHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public Task<List<CourtDashboardResponse>> Handle(GetCourtDashboardCommand request, CancellationToken cancellationToken)
    {
        var courtSubList = _beatSportsDbContext.CourtSubdivisions
                        .Where(x => x.CourtId == request.CourtId)
                        .Include(x => x.Bookings)
                        .Include(x => x.CourtSubdivisionSettings).ThenInclude(x => x.SportCategories)
                        .ToList();

        var year = request.Year;
        if (year == 0)
        {
            year = 2000;
        }

        if(request.SportCategory != null)
        {
            courtSubList = courtSubList.Where(x => x.CourtSubdivisionSettings.SportCategories.Name.Equals(request.SportCategory)).ToList();
        }

        var result = new List<CourtDashboardResponse>();

        for (int month = 1; month <= 12; month++)
        {
            var courtResponse = new CourtDashboardResponse();

            foreach (var courtSub in courtSubList)
            {
                var bookingsInGroup = courtSub.Bookings
                .Where(b => b.Created.Month == month && b.Created.Year == year)
                .ToList();

                courtResponse.Y += bookingsInGroup.Sum(x => x.TotalAmount);
            }

            courtResponse.X = DateTime.Parse($"{request.Year}/{month}/1");
            result.Add(courtResponse);
        }

        return Task.FromResult(result);
    }
}
