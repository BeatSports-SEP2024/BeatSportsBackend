using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Common.Response;
public class WalletResponse : IMapFrom<Wallet>
{
    public Guid WalletId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Balance { get; set; }
}