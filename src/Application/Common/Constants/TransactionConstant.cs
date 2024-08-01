using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Constants;
public class TransactionConstant
{
    public static string TransactionSuccessMessage => "Đặt sân thành công";
    public static string TransactionForTransferSuccess => "Chuyển khoản thành công";
    public static string TransactionFailedInsufficientBalance => "Số dư không đủ để thanh toán";
    public static string TransactionCancel => "Hủy đặt sân, nhận lại tiền đã thanh toán";
    /// <summary>
    /// transactionType có 3 trạng thái: Rút tiền, nạp tiền, Giao dịch trong App
    /// </summary>
    public static string TransactionTypeInApp => "Giao dịch trong App";
    public static string TransactionTypeWithDraw => "Rút tiền";
}
