using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Common.Response;
public class TransactionResponse : IMapFrom<Transaction>
{
    public Guid AccountId { get; set; }
    public Guid WalletId { get; set; }
    public string? TransactionMessage { get; set; }
    public string? TransactionPayload { get; set; }
    public string? TransactionStatus { get; set; }
    public decimal? TransactionAmount { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? TransactionType { get; set; }
}
