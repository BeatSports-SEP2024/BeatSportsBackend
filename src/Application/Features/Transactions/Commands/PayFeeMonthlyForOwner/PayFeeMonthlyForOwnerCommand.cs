using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using MediatR;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.PayFeeMonthlyForOwner;
public class PayFeeMonthlyForOwnerCommand : IRequest<BeatSportsResponseV2>
{
    public Guid? OwnerId { get; set; }
    [Range(1, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0.")]
    public decimal? FeeMonthlyForOwner { get; set; }
}