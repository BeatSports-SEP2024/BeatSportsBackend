﻿using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities.Room;
public class RoomMember : BaseAuditableEntity
{
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
    public string? RoleInRoom { get; set; }

    public virtual Customer Customer { get; set; } = null!;
    public virtual RoomMatch RoomMatch { get; set; } = null!;
}
