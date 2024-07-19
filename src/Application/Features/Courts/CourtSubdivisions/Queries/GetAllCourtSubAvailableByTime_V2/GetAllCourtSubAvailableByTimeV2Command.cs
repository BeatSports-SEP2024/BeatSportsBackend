using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubAvailableByTime_V1;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubAvailableByTime_V2;
public class GetAllCourtSubAvailableByTimeV2Command : IRequest<CourtSubCheckedResponseV2>
{
    public Guid CourtId { get; set; }
    public DateTime DateCheck { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class CourtSubCheckedResponseV2 
{
    public Guid CourtId { get; set; }
    public string CourtName { get; set; }
    public List<CourtSubCheckedResponseV3> CourtSubAvailables{ get; set; }
}

public class CourtSubCheckedResponseV3
{
    public Guid CourtSubId { get; set; }
    public string CourtSubName { get; set; }
    public string Status { get; set; }
}

