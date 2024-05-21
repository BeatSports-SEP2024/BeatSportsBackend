using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.PaymentEntity;
public class Payment : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }
    [ForeignKey("PaymentMethod")]
    public Guid PaymentMethodId { get; set; }
    [ForeignKey("PaymentDestination")]
    public Guid PaymentDestinationId { get; set; }
    [ForeignKey("Merchant")]
    public Guid MerchantId { get; set; }
    public string PaymentContent { get; set; } = string.Empty;
    public string PaymentCurrency { get; set; } = string.Empty;
    /// <summary>
    /// Reference from order/booking to merchant
    /// </summary>
    public string? PaymentRefId { get; set; }
    public decimal? RequiredAmount { get; set; }
    public DateTime? PaymentDate { get; set; } = DateTime.Now;
    public DateTime? ExpireDate { get; set; }
    public string? PaymentLanguage { get; set; } = string.Empty;
    public decimal? PaidAmount { get; set; }
    public string? PaymentStatus { get; set; } = string.Empty;
    public string? PaymentLastMessage { get; set; } = string.Empty;

    //Relationship
    public virtual Account Account { get; set; } = null!;
    public virtual Merchant Merchant { get; set; } = null!;
    public virtual PaymentDestination PaymentDestination { get; set; } = null!;
    public virtual PaymentMethod PaymentMethod { get; set; }
    public virtual IList<PaymentSignature> PaymentSignature { get; set; }
    public virtual IList<PaymentTransaction> PaymentTransaction { get; set; }
}
