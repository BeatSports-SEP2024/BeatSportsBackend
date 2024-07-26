using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BeatSportsAPI.Application.Common.Mappings;
using System.Threading.Tasks;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace BeatSportsAPI.Application.Common.Response;
public class TransactionThirdpartyResponse
{
    public Guid TransactionId { get; set; }
    public string? TransactionMessage { get; set; }
    public ExpandoObject? TransactionPayload { get; set; }
    public string? TransactionStatus { get; set; }
    public decimal? TransactionAmount { get; set; }
    public DateTime? TransactionDate { get; set; }
    public string? TransactionType { get; set; }
    public Guid PaymentId { get; set; }
}

public class ResponseData
{
    public long Vnp_Amount { get; set; }
    public string Vnp_BankCode { get; set; }
    public string Vnp_BankTranNo { get; set; }
    public string Vnp_CardType { get; set; }
    public string Vnp_OrderInfo { get; set; }
    public string Vnp_PayDate { get; set; }
    public string Vnp_ResponseCode { get; set; }
    public string Vnp_TmnCode { get; set; }
    public string Vnp_TransactionNo { get; set; }
    public string Vnp_TransactionStatus { get; set; }
    public string Vnp_TxnRef { get; set; }
}

public class TransactionPayload
{
    public ResponseData ResponseData { get; set; }
    public string Vnp_TmnCode { get; set; }
    public string Vnp_BankCode { get; set; }
    public string Vnp_BankTranNo { get; set; }
    public string Vnp_CardType { get; set; }
    public string Vnp_OrderType { get; set; }
    public string Vnp_OrderInfo { get; set; }
    public string Vnp_TransactionNo { get; set; }
    public string Vnp_TransactionStatus { get; set; }
    public string Vnp_TxnRef { get; set; }
    public string Vnp_SecureHashType { get; set; }
    public string Vnp_SecureHash { get; set; }
    public long Vnp_Amount { get; set; }
    public string Vnp_ResponseCode { get; set; }
    public string Vnp_PayDate { get; set; }
}
