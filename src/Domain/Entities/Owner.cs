﻿using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities;
public class Owner
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }

    public virtual Account Account { get; set; }
}
