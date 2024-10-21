using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.TransferMoneyInApp;
public class TransferMoneyInAppCommand : IRequest<BeatSportsResponseV2>
{
    public Guid CustomerId { get; set; }
    public Guid OwnerId { get; set; }
    public decimal? TransactionAmount { get; set; }
}
