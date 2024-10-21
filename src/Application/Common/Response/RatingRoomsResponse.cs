using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.Room;

namespace BeatSportsAPI.Application.Common.Response;
public class RatingRoomsResponse : IMapFrom<RatingRoom>
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public double? WinRatePercent { get; set; }
    public double? LoseRatePercent { get; set; }
}
