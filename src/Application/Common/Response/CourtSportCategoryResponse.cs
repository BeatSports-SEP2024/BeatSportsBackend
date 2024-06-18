using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Application.Common.Response;
public class CourtSportCategoryResponse : IMapFrom<CourtSportCategory>
{
    public Guid SportCategoryId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
}