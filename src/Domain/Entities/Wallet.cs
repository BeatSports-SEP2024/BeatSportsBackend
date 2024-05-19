using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities;
public class Wallet : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }    
    public decimal Balance { get; set; }    

    public virtual Account Account { get; set; } = null!;
}
