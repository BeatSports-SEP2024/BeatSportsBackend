using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities;
public class User : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get;set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string? ProfilePictureURL { get; set; }
    public string? Bio { get; set; }
    public string PhoneNumber { get; set; } = null!;
    public string Role { get; set; } = null!;

    public virtual Account Account { get; set; }
}
