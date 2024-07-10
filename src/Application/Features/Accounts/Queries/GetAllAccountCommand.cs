using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Accounts.Queries;
public class GetAllAccountCommand : IRequest<PaginatedList<AccountResponse>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    [EnumDataType(typeof(RoleEnums))]
    public RoleEnums Role { get; set; }
    public string? Username { get; set; }
    public Guid? WalletId { get; set; }
    [Phone]
    public string? PhoneNumber { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
