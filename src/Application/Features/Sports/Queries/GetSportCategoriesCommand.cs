using System.ComponentModel.DataAnnotations;
using AutoFilterer.Types;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Newtonsoft.Json;

namespace BeatSportsAPI.Application.Features.Sports.Queries;
public class GetSportCategoriesCommand : IRequest<PaginatedList<SportCategoriesResponse>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    [EnumDataType(typeof(SportCategoriesEnums))]
    public SportCategoriesEnums? SportCategoryName { get; set; }
}
