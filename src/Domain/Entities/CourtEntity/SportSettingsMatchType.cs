using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class SportSettingsMatchType : BaseAuditableEntity
{
    [ForeignKey("CourtSubdivisionSetting")]
    public Guid CourtSubdivisionSettingId { get; set; }

    public string? MatchTypeName { get; set; }
    public int? TotalMember { get; set; }

    public virtual CourtSubdivisionSetting? CourtSubdivisionSetting { get; set; }
}
