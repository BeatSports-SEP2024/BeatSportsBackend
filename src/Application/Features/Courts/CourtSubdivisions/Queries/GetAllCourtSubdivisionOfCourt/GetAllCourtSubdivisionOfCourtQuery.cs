using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionOfCourt;
public class GetAllCourtSubdivisionOfCourtQuery : IRequest<PaginatedList<CourtSubdivisionResponse>>
{
    public Guid CourtId { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}
