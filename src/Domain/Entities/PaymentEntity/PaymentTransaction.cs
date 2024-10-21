using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.PaymentEntity;

/// <summary>
/// Payment History
/// </summary>
public class PaymentTransaction : BaseAuditableEntity
{
    public string? TranMessage { get; set; } = string.Empty;
    public string? TranPayload { get; set; } = string.Empty;
    public string? TranStatus { get; set; } = string.Empty;
    public decimal? TranAmount { get; set; }
    public DateTime? TranDate { get; set; }
    public string? TranRefId { get; set; } = string.Empty;

    [ForeignKey("Payment")]
    public Guid PaymentId { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}
