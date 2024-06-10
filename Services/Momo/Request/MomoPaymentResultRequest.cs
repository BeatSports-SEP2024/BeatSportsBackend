using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Utils.Helpers;

namespace Services.Momo.Request;
public class MomoPaymentResultRequest
{
    [JsonProperty("partnerCode")]
    public string partnerCode { get; set; } = string.Empty;

    [JsonProperty("orderId")]
    public string orderId { get; set; } = string.Empty;

    [JsonProperty("requestId")]
    public string requestId { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public long amount { get; set; }

    [JsonProperty("orderInfo")]
    public string orderInfo { get; set; } = string.Empty;

    [JsonProperty("orderType")]
    public string orderType { get; set; } = string.Empty;

    [JsonProperty("transId")]
    public string transId { get; set; } = string.Empty;

    [JsonProperty("message")]
    public string message { get; set; } = string.Empty;

    [JsonProperty("resultCode")]
    public int resultCode { get; set; }

    [JsonProperty("payType")]
    public string payType { get; set; } = string.Empty;

    [JsonProperty("responseTime")]
    public long responseTime { get; set; }

    [JsonProperty("extraData")]
    public string extraData { get; set; } = string.Empty;

    [JsonProperty("signature")]
    public string signature { get; set; } = string.Empty;

    public bool IsValidSignature(string accessKey, string secretKey)
    {
        var rawHash = $"accessKey={accessKey}" +
                      $"&amount={this.amount}" +
                      $"&extraData={this.extraData}" +
                      $"&message={this.message}" +
                      $"&orderId={this.orderId}" +
                      $"&orderInfo={this.orderInfo}" +
                      $"&orderType={this.orderType}" +
                      $"&partnerCode={this.partnerCode}" +
                      $"&payType={this.payType}" +
                      $"&requestId={this.requestId}" +
                      $"&responseTime={this.responseTime}" +
                      $"&resultCode={this.resultCode}" +
                      $"&transId={this.transId}";
        var checkSignature = HashHelper.HmacSHA256(rawHash, secretKey);
        return this.signature.Equals(checkSignature);
    }
}
