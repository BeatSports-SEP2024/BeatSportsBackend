using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Common.Response;
//Get All
public class SportCategoriesResponse : IMapFrom<SportCategory>
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; } = true;
}
