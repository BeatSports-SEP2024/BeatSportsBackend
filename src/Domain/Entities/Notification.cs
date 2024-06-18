using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Entities;
public class Notification : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }
    public string Message { get; set; }
    public bool Status { get; set; }
    public virtual Account Accounts { get; set; }    
}