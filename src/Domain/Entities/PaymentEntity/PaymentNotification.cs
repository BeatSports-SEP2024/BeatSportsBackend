using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.PaymentEntity;

/// <summary>
/// Notification of payment 
/// </summary>
public class PaymentNotification : BaseAuditableEntity
{
    public string PaymentRefId { get; set; } = string.Empty;
    public DateTime? NotiDate { get; set; }
    public string? NotiContent { get; set; } = string.Empty;
    public decimal NotiAmount { get; set; }
    public string? NotiMessage { get; set; } = string.Empty;
    public string? NotiSignature { get; set; } = string.Empty;
    public string? NotiNotiStatus { get; set; } = string.Empty;
    public DateTime? NotiResDate { get; set; }
    public string? NotiResMessage { get; set; } = string.Empty;
    public string? NotiResHttpCode { get; set; } = string.Empty;

    [ForeignKey("Payment")]
    public Guid? PaymentId { get; set; } 
    [ForeignKey("Merchant")]
    public Guid MerchantId { get; set; } 

    public virtual Merchant Merchant { get; set; } = null!;
    public virtual Payment Payment { get; set; } = null!;
}
