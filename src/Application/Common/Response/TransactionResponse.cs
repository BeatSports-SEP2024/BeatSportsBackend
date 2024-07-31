using System;
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

public class TransactionResponseV3 
{
    public Guid TransactionId { get; set; }
    public UserInfo2 UserInfo { get; set; }  
    public decimal? TransactionAmount { get; set; }
    public string? TransactionStatus { get; set; }
    public string AdminCheckStatus { get; set; }
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
public class UserInfo2
{
    public Guid OwnerId { get; set; }
    public Guid WalletId { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
}

public class TransactionResponseV4
{
    public Guid TransactionId { get; set; }
    public UserInfo2 UserInfo { get; set; }
    public List<TransactionResponseV2> TransactionList { get; set; }
    public decimal? TotalAmountReceived { get; set; }
    public decimal? TotalAmountWithdrawn { get; set; }
    public decimal? TotalAmountAvailableForWithdrawal { get; set; }
    public decimal? ToTalAmountWithdrawalRequestByOwner { get; set; }
}