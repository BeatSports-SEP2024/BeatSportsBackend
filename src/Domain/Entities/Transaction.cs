using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities;
public class Transaction : BaseAuditableEntity
{
    [ForeignKey("Wallet")]
    public Guid WalletId { get; set; }
    public Guid WalletTargetId { get; set; }
    public string? TransactionMessage { get; set; }
    public string? TransactionPayload { get; set; }
    public string? TransactionStatus { get; set; }
    public AdminCheckEnums? AdminCheckStatus { get; set; }
    public decimal? TransactionAmount { get; set; }
    public DateTime? TransactionDate { get; set; }    
    public string? TransactionType { get; set; } 
    public string? ImageOfInvoice { get; set; }
    /// <summary>
    /// Không cần relationship, để 
    /// check coi call back được thì lấy 
    /// danh sách nạp tiền lên cho customer
    /// </summary>
    public string? PaymentTransactionId{ get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
    public virtual Booking Booking { get; set; }
}
