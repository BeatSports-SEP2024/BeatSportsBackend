using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Sports.Commands;
public class CreateSportCategoriesCommand : IRequest<BeatSportsResponse>
{
    [EnumDataType(typeof(SportCategoriesEnums))]
    public SportCategoriesEnums Name { get; set; } 
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; } = true;
}
