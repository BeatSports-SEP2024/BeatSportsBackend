using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubAvailableByTime_V1;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubAvailableByTime_V2;
public class GetAllCourtSubAvailableByTimeV2Handler : IRequestHandler<GetAllCourtSubAvailableByTimeV2Command, CourtSubCheckedResponseV2>
{
    private IBeatSportsDbContext _dbContext;

    public GetAllCourtSubAvailableByTimeV2Handler(IBeatSportsDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public Task<CourtSubCheckedResponseV2> Handle(GetAllCourtSubAvailableByTimeV2Command request, CancellationToken cancellationToken)
    {
        var courtSubList = _dbContext.CourtSubdivisions
                         .Where(x => x.CourtId == request.CourtId)
                         .Include(x => x.Court)
                         .ToList();

        var startTime = request.DateCheck.Add(request.StartTime);
        var endTime = request.DateCheck.Add(request.EndTime);

        var tmp = courtSubList.ToList();

        foreach (var courseSub in courtSubList)
        {
            var flag = 0;

            var timeCheckList = _dbContext.TimeChecking
                            .Where(x => x.CourtSubdivisionId == courseSub.Id).ToList();

            if(timeCheckList.Count > 0)
            {
                foreach (var timeCheck in timeCheckList)
                {
                    if (startTime <= timeCheck.StartTime && endTime >= timeCheck.EndTime)
                    {
                        flag++;
                        break;
                    }
                    else if (((startTime <= timeCheck.StartTime) && (timeCheck.StartTime <= endTime)))
                    {
                        flag++;
                        break;
                    }
                    else if (((startTime <= timeCheck.EndTime) && (timeCheck.EndTime <= endTime)))
                    {
                        flag++;
                        break;
                    }
                }
            }
            
            if(flag > 0)
            {
                tmp.Remove(courseSub);
            }
        }

        var courtSubResponse = tmp.Select(x => new CourtSubCheckedResponseV3
        {
            CourtSubId = x.Id,
            CourtSubName = x.CourtSubdivisionName,
            Status = "Available"
        });

        var result = new CourtSubCheckedResponseV2
        {
            CourtId = request.CourtId,
            CourtName = courtSubList.ElementAt(0).Court.CourtName,
            CourtSubAvailables = courtSubResponse.ToList(),
        };

        return Task.FromResult(result);
    }
}
