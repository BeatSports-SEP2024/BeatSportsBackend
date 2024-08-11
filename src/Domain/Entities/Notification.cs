using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Entities;
public class Notification : BaseAuditableEntity
{
    [ForeignKey("Account")]
    public Guid AccountId { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    /// <summary>
    /// True/false là đã đọc hay chưa
    /// </summary>
    public bool IsRead { get; set; }
    /// <summary>
    /// Loại thông báo (ví dụ: Booking, Feedback, PayFee). trước tiên 3 loại này trước đi
    /// PayFee trả tiền app cho th admin hằng tháng
    /// </summary>
    public string? Type { get; set; }
    public string? BookingId { get; set; }
    public virtual Account Accounts { get; set; }    
}