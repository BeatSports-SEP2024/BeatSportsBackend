using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.ApproveMoneyForOwner;
public class ApproveMoneyForOwnerCommand : IRequest<BeatSportsResponseV2>
{
    public Guid OwnerId { get; set; }
    public Guid TransactionId { get; set; }
}
