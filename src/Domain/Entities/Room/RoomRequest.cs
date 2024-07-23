using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Entities.Room;
public class RoomRequest : BaseAuditableEntity
{
    [ForeignKey("Customer")]
    public Guid CustomerId { get; set; }
    [ForeignKey("RoomMatch")]
    public Guid RoomMatchId { get; set; }
    public RoomRequestEnums JoinStatus { get; set; }
    public DateTime DateRequest { get; set; }
    public DateTime DateApprove { get; set; }
    public virtual Customer Customer { get; set; }
    public virtual RoomMatch RoomMatch { get; set; }
}
