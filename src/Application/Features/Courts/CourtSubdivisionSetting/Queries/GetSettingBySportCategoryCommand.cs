using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisionSetting.Queries;
public class GetSettingBySportCategoryCommand : IRequest<List<CourtSubSettingResponse>>
{
    [EnumDataType(typeof(SportCategoriesEnums))]
    public SportCategoriesEnums SportCategoriesFilter { get; set; }
}