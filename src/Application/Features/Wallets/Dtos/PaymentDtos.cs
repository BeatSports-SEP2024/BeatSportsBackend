using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Domain.Entities.PaymentEntity;

namespace BeatSportsAPI.Application.Features.Wallets.Dtos;
public class PaymentDtos : IMapFrom<Payment>
{
    public string Id { get; set; } = string.Empty;
    public string PaymentContent { get; set; } = string.Empty;
    public string PaymentCurrency { get; set; } = string.Empty;
    public string PaymentRefId {  get; set; } = string.Empty;   
    public decimal? RequiredAmount {  get; set; }
    public DateTime? PaymentDate { get; set; } = DateTime.Now;
    public DateTime? ExpireDate { get; set; }
    public string PaymentLanguage {  get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal? PaidAmount { get; set; }

    // relationship
    public string MerchantId { get; set; } = string.Empty;
    public string PaymentDestinationId { get; set; } = string.Empty;
    public string PaymentMethodId { get; set; } = string.Empty;
    public string PaymentAccountId { get; set; } = string.Empty;

}
