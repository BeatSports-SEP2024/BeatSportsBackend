﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Common;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Models.Authentication;
public class RegisterCustomerModelRequest : IRequest<BeatSportsResponse>
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    [EnumDataType(typeof(GenderEnums))]
    public GenderEnums Gender { get; set; }
    public string? ProfilePictureURL { get; set; }
    public string? Bio { get; set; }
    public string PhoneNumber { get; set; } = null!;
    [EnumDataType(typeof(RoleEnums))]
    public RoleEnums Role { get; set; } 
    public CustomerModels Customer { get; set; }

    public class CustomerModels
    {
        public int RewardPoints { get; set; }
        public string Address { get; set; } = null!;
    }
}