using System.ComponentModel.DataAnnotations;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
public class GetAllCourtCommand : IRequest<PaginatedList<CourtResponseV2>>
{
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
}
