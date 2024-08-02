using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllWithdrawalRequestByOwner;
public class GetAllWithdrawalRequestByOwnerCommand : IRequest<PaginatedTransactionResponseV2>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public string? KeyWord { get; set; }
    public string? Filter { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
