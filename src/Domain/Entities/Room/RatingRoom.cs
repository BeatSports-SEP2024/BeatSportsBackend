using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Entities.Room;
public class RatingRoom : BaseAuditableEntity
{
    public string? Description { get; set; }    
    public double? WinRatePercent { get; set; }    
    public double? LoseRatePercent { get; set; }

    public virtual RoomMatch? RoomMatch { get; set; }
}
