using System.ComponentModel.DataAnnotations.Schema;

namespace BeatSportsAPI.Domain.Entities;
public class Transaction : BaseAuditableEntity
{
    [ForeignKey("Wallet")]
    public Guid WalletId { get; set; }
    public Guid WalletTargetId { get; set; }
    public Guid? RoomMatchId { get; set; }
    public string? TransactionMessage { get; set; }
    public string? TransactionPayload { get; set; }
    /// <summary>
    /// TransactionStatus: Trạng thái này theo dõi quá trình xử lý của giao dịch, 
    /// chẳng hạn như việc trừ tiền khỏi ví của khách hàng đã hoàn tất hay chưa, 
    /// và nó phản ánh trạng thái hiện tại của giao dịch như đã hoàn thành, đang chờ xử lý, thất bại, hay đã được hoàn trả.
    /// </summary>
    public string? TransactionStatus { get; set; }
    /// <summary>
    /// AdminCheckStatus: Trạng thái này liên quan đến quan điểm của chủ sân (owner), 
    /// Trạng thái này cực kỳ quan trọng để phân biệt liệu giao dịch có nằm trong khoảng thời gian mà khách hàng có thể hủy hay không, 
    /// giúp quản lý các tình huống cần giữ giao dịch tạm thời trước khi hoàn tất cho tài khoản của chủ sân, 
    /// đảm bảo việc xử lý các hủy bỏ tiềm năng một cách thích hợp.
    /// </summary>
    public AdminCheckEnums? AdminCheckStatus { get; set; }
    public decimal? TransactionAmount { get; set; }
    public DateTime? TransactionDate { get; set; }
    /// <summary>
    /// Đang có tổng cộng là 
    /// Giao dịch trong App, Payfee, Nạp tiền, Rút tiền, Đóng tiền, JoinRoom, 
    /// OutRoom(Chủ phòng out, thành viên out đều là ...), RefundRoomMaster, RefundRoomMember (Sau khi cập nhật kết quả, trả tiền cho thành viên team win)
    /// </summary>
    public string? TransactionType { get; set; } 
    public string? ImageOfInvoice { get; set; }
    public string? ReasonOfRejected { get; set; }
    /// <summary>
    /// Không cần relationship, để 
    /// check coi call back được thì lấy 
    /// danh sách nạp tiền lên cho customer
    /// </summary>
    public string? PaymentTransactionId{ get; set; }

    public virtual Wallet Wallet { get; set; } = null!;
    public virtual Booking Booking { get; set; }
}
