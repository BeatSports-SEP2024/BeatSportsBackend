using System.ComponentModel.DataAnnotations;
using AutoFilterer.Attributes;
using AutoFilterer.Enums;
using AutoFilterer.Types;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetAll;
public class CourtFilterer : FilterBase
{
    [EnumDataType(typeof(SportCategoriesEnums))]
    [StringFilterOptions(StringFilterOption.Contains, StringComparison.InvariantCultureIgnoreCase)]
    public SportCategoriesEnums SportCategoriesEnums { get; set; }
}
