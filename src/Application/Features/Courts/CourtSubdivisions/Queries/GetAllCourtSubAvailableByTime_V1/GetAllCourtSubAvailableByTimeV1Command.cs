using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetCourtSubdivisionAndTimeByCourtIdAndDate;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubAvailableByTime_V1;
public class GetAllCourtSubAvailableByTimeV1Command : IRequest<CourtSubCheckedResponse>
{
    public Guid CourtSubId { get; set; }
    public DateTime DateCheck { get; set; }
}

public class CourtSubCheckedResponse 
{
    public Guid CourtSubId { get; set; }
    public string CourtSubName { get; set; }
    public List<TimeCheckingResponse> TimeBookeds { get; set; }
}

public class TimeCheckingResponse
{
    public Guid TimeCheckingId { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}
