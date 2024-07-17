using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Entities.CourtEntity;
public class CourtSubdivisionSetting : BaseAuditableEntity
{
    [ForeignKey("CourtSubdivision")]
    public Guid CourtSubdivisionId { get; set; }
    [ForeignKey("SportCategory")]
    public Guid SportCategory { get; set; }
    //Mo ta ve loai hinh va chat lieu san thi dau
    public string? CourtType { get; set; }
    public virtual SportCategory SportCategories { get; set; }
    public virtual CourtSubdivision CourtSubdivisions { get; set; }
}