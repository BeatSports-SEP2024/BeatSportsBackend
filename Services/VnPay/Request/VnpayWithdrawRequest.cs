using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.VnPay.Request;
public class VnpayWithdrawRequest
{
    public string? vnp_Version { get; set; }
    public string? vnp_Command { get; set; }
    public string? vnp_TmnCode { get; set; }
    public string? vnp_Amount { get; set; }
    public string? vnp_OrderInfo { get; set; }
    public string? vnp_TxnRef { get; set; }
    public string? vnp_IpAddr { get; set; }
    public string? vnp_CreateDate { get; set; }
    public string? vnp_SecureHash { get; set; }
}
