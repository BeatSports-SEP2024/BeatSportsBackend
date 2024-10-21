using System.ComponentModel.DataAnnotations;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Queries.GetAllCourtSubdivisionOfCourt;
public class GetAllCourtSubdivisionOfCourtQuery : IRequest<PaginatedList<CourtSubdivisionResponseV3>>
{
    public Guid CourtId { get; set; }
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
}
