using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFilterer.Attributes;
using AutoFilterer.Types;
using BeatSportsAPI.Domain.Enums;

namespace BeatSportsAPI.Application.Features.Accounts.Queries;
public class AccountFilter : FilterBase
{
    [EnumDataType(typeof(RoleEnums))]
    [CompareTo("Role", "equals")]
    public RoleEnums Role { get; set; }
    [CompareTo("UserName", "equals")]
    public string? Username { get; set; }
    //[CompareTo("Account.Wallet.Id", "equals")]
    //public Guid WalletId { get; set; }
    [CompareTo("PhoneNumber", "equals")]
    public string? PhoneNumber { get; set; }
    [CompareTo("Created", "greater than or equal")]
    public Range<DateTime>? CreatedRange { get; set; }
}