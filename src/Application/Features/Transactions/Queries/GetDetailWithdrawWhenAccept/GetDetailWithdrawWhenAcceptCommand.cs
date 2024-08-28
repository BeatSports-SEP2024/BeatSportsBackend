using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetDetailWithdrawWhenAccept;
public class GetDetailWithdrawWhenAcceptCommand : IRequest<GetDetailWithdrawWhenAcceptResponse>
{
    public Guid TransactionId { get; set; }
}