using BeatSportsAPI.Domain.Entities.PaymentEntity;

namespace BeatSportsAPI.Domain.Entities;
public class Account : BaseAuditableEntity
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string? ProfilePictureURL { get; set; }
    public string? Bio { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Role { get; set; } = null!;

    public virtual Wallet? Wallet { get; set; }
    public virtual Payment? Payment { get; set; }
}
