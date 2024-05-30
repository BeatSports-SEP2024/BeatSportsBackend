using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
public class GetAllCourtWithDetailCommand : IRequest<PaginatedList<CourtWithDetailResponse>>
{
    [Required]
    public int PageIndex { get; set; }
    [Required]
    public int PageSize { get; set; }
    [EnumDataType(typeof(SportCategoriesEnums))]
    [StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
    public SportCategoriesEnums SportCategoriesEnums { get; set; }
    //public CourtFilterer? Filterer { get; set; }
}
