using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
public class GetAllCourtCommand : IRequest<PaginatedList<CourtResponse>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public CourtFilterer? Filterer { get; set; }
}
