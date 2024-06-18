using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class TimeChecking : BaseAuditableEntity
{
    [ForeignKey("CourtSubdivision")]
    public Guid CourtSubdivisionId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsLock { get; set; }
    public virtual CourtSubdivision CourtSubdivision { get; set; }
}