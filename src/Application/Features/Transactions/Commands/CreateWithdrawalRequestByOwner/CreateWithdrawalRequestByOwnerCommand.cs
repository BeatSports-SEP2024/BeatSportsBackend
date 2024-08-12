using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.CreateWithdrawalRequestByOwner;
public class CreateWithdrawalRequestByOwnerCommand : IRequest<BeatSportsResponseV2>
{
    public Guid OwnerId { get; set; }
    public decimal TransactionAmount { get; set; }
}