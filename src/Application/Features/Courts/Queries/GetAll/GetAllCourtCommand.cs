using System.ComponentModel.DataAnnotations;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
public class GetAllCourtCommand : IRequest<PaginatedList<CourtResponse>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    [EnumDataType(typeof(SportCategoriesEnums))]
    [StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
    public SportCategoriesEnums SportCategoriesEnums { get; set; }
    //public CourtFilterer? Filterer { get; set; }
}
