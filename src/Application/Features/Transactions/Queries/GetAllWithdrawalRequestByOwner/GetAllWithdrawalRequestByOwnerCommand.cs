using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllWithdrawalRequestByOwner;
public class GetAllWithdrawalRequestByOwnerCommand : IRequest<List<TransactionResponseV3>>
{
    public string? KeyWord { get; set; }
    public string? Filter { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
