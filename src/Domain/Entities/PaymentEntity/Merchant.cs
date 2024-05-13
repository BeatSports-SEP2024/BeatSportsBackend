namespace BeatSportsAPI.Domain.Entities.PaymentEntity;

/// <summary>
/// Choose method for paying
/// </summary>
public class Merchant : BaseAuditableEntity
{
    public string? MerchantName { get; set; } = string.Empty;
    public string? MerchantWebLink { get; set; } = string.Empty;
    public string? MerchantIpnUrl { get; set; } = string.Empty;
    public string? MerchantReturnUrl { get; set; } = string.Empty;
    public string? SecretKey { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public virtual IList<Payment> Payment { get; set; } = null!;
    public virtual IList<PaymentNotification> PaymentNotification { get; set; } = null!;
}
