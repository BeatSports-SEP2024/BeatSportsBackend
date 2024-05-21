using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities;
public class Transaction : BaseAuditableEntity
{
    [ForeignKey("Wallet")]
    public Guid WalletId { get; set; }  
    public string? TransactionMessage { get; set; }
    public string? TransactionPayload { get; set; }
    public string? TransactionStatus { get; set; }
    public decimal? TransactionAmount { get; set; }
    public DateTime? TransactionDate { get; set; }    
    public string? TransactionType { get; set; } 

    public virtual Wallet Wallet { get; set; } = null!;
}
