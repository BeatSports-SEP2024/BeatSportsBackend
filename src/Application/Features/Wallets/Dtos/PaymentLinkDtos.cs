using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Features.Wallets.Dtos;
public class PaymentLinkDtos
{
    public string PaymentId { get; set; } = string.Empty;
    public string PaymentUrl { get; set; } = string.Empty;
}
