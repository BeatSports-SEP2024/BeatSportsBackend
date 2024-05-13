namespace BeatSportsAPI.Domain.Entities;
public class Account : BaseAuditableEntity
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}
