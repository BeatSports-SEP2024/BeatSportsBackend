﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Common.Response;
public class TransactionResponse : IMapFrom<Transaction>
{
    public Guid TransactionId { get; set; }
    public Guid WalletId { get; set; }
    public Guid WalletTargetId { get; set; }
    public string? TransactionMessage { get; set; }
    public string? TransactionPayload { get; set; }
    public string? TransactionStatus { get; set; }
    public string AdminCheckStatus { get; set; }
    public decimal? TransactionAmount { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? TransactionType { get; set; }
}

public class TransactionResponseV2 
{
    public Guid TransactionId { get; set; }
    public UserInfo From { get; set; }  
    public UserInfo To { get; set; }
    public decimal? TransactionAmount { get; set; }
    public string? TransactionStatus { get; set; }
    public string AdminCheckStatus { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? TransactionType { get; set; }
}

public class UserInfo
{
    public Guid WalletId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
}