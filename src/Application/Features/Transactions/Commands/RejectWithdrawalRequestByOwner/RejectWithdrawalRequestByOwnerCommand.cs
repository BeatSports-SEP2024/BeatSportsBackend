using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.RejectWithdrawalRequestByOwner;
public class RejectWithdrawalRequestByOwnerCommand : IRequest<BeatSportsResponseV2>
{
    public Guid OwnerId { get; set; }
    public Guid TransactionId { get; set; }
}
