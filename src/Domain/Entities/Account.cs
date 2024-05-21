using BeatSportsAPI.Domain.Entities.PaymentEntity;

namespace BeatSportsAPI.Domain.Entities;
public class Account : BaseAuditableEntity
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    /// <summary>
    /// this is google Id for login with google
    /// </summary>
    public string? GoogleId { get; set; }

    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? ProfilePictureURL { get; set; }
    public string? Bio { get; set; }
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = null!;

    public virtual Wallet? Wallet { get; set; }
    public virtual Customer? Customer { get; set; }
    public virtual Owner? Owner { get; set; }

    public virtual IList<Payment> Payment { get; set; }
}
