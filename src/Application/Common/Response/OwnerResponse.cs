﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Response;
public class OwnerResponse : IMapFrom<Owner>
{
    public Guid AccountId { get; set; }
    public Guid OwnerId { get; set; }
    public string UserName { get; set; } 
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string? ProfilePictureURL { get; set; }
    public string? Bio { get; set; }
    public string PhoneNumber { get; set; } 
    public string Address { get; set; }
}