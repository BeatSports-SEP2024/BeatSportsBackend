﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.VnPay.Response;
public class VnpayWithdrawResponse
{
    public string? vnp_ResponseCode { get; set; }
    public string? vnp_Message { get; set; }
    public string? vnp_TxnRef { get; set; }
    public string? vnp_Amount { get; set; }
    public string? vnp_TransactionNo { get; set; }
    public string? vnp_ResponseDate { get; set; }
}
