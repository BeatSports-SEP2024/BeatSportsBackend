using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Entities;
public class RefreshToken: BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }
    public string AccessToken { get; set; }
    public string Token { get; set; } 
    public DateTime TokenCreated { get; set; }
    public DateTime TokenExpires { get; set; }
    public virtual Account Account { get; set; }
}
