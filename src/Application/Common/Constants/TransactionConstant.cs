using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Constants;
public class TransactionConstant
{
    public static string TransactionSuccessMessage => "Đặt sân thành công";
    public static string TransactionFailedInsufficientBalance => "Số dư không đủ để thanh toán";
    public static string TransactionCancel => "Hủy đặt sân, nhận lại tiền đã thanh toán";

    public static string TransactionType => "Giao dịch trong ứng dụng";
}
