using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionAndTimeByCourtIdAndDate;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubAvailableByTime_V1;
public class GetAllCourtSubAvailableByTimeV1Handler : IRequestHandler<GetAllCourtSubAvailableByTimeV1Command, CourtSubCheckedResponse>
{
    private IBeatSportsDbContext _dbContext;

    public GetAllCourtSubAvailableByTimeV1Handler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<CourtSubCheckedResponse> Handle(GetAllCourtSubAvailableByTimeV1Command request, CancellationToken cancellationToken)
    {
        // Check CourtSub
        var courtSub = _dbContext.CourtSubdivisions
            .Where(x => x.Id == request.CourtSubId && !x.IsDelete).SingleOrDefault();
        if (courtSub == null)
        {
            throw new BadRequestException($"CourtSub with CourtSub ID: {request.CourtSubId} does not exist");
        }

        var timeCkeckedList = _dbContext.TimeChecking
                            .Where(x => x.CourtSubdivisionId == request.CourtSubId && x.DateBooking.Date == request.DateCheck.Date && x.IsLock == true)
                            .ToList();

        var timeResponse = timeCkeckedList.Select(x => new TimeCheckingResponse
        {
            TimeCheckingId = x.Id,
            StartTime = x.StartTime.TimeOfDay.ToString(@"hh\:mm"),
            EndTime = x.EndTime.TimeOfDay.ToString(@"hh\:mm"),
        }).ToList();

        var response = new CourtSubCheckedResponse
        {
            CourtSubId = request.CourtSubId,
            CourtSubName = courtSub.CourtSubdivisionName,
            TimeBookeds = timeResponse,
        };

        return Task.FromResult(response);
    }
}
