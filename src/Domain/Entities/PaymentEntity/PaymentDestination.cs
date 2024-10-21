namespace BeatSportsAPI.Domain.Entities.PaymentEntity;

/// <summary>
/// Destination of payment / List of Payment method
/// </summary>
public class PaymentDestination : BaseAuditableEntity
{
    public string? DesName { get; set; } = string.Empty;
    public string? DesShortName { get; set; } = string.Empty;
    public string? DesParentId { get; set; } = string.Empty;
    public string? DesLogo { get; set; } = string.Empty;
    /// <summary>
    /// Sort Index
    /// </summary>
    public int SortIndex { get; set; }
    public bool IsActive { get; set; }

    public virtual IList<Payment> Payment { get; set; } = null!;
}
