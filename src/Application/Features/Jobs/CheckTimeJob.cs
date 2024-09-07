using System.Text.RegularExpressions;
using System.Transactions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Jobs;
public class CheckTimeJob
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IEmailService _emailService;

    public CheckTimeJob(IBeatSportsDbContext beatSportsDbContext, IEmailService emailService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _emailService = emailService;
    }
    public void CheckTimeOfCourt()
    {
        var bookingList = _beatSportsDbContext.Bookings.Where(x => x.BookingStatus == BookingEnums.Process.ToString()).ToList();
        foreach (var booking in bookingList)
        {

            var checkAfter2Minutes = booking.Created.AddMinutes(2);
            if (DateTime.Now > checkAfter2Minutes)
            {
                DateTime startTime = booking.PlayingDate.Date.Add(booking.StartTimePlaying);
                DateTime endTime = booking.PlayingDate.Date.Add(booking.EndTimePlaying);
                var timeChecking = _beatSportsDbContext.TimeChecking
                                   .Where(x => x.CourtSubdivisionId == booking.CourtSubdivisionId
                                   && x.StartTime == startTime && x.EndTime == endTime && x.DateBooking == booking.PlayingDate)
                                   .FirstOrDefault();


                if (timeChecking != null)
                {
                    timeChecking.IsDelete = true;
                    _beatSportsDbContext.TimeChecking.Remove(timeChecking);
                    _beatSportsDbContext.SaveChanges();

                    booking.IsDelete = true;
                    _beatSportsDbContext.Bookings.Remove(booking);
                    _beatSportsDbContext.SaveChanges();

                    Console.WriteLine($"Recurring job executed start for Booking ${booking.Id}");
                }
            }
        }
    }

    public void CheckTimeOfBooking()
    {
        // Lấy những booking nào đang có status là Approved ra 
        var bookingList = _beatSportsDbContext.Bookings
                        .Where(x => !x.IsDelete && x.BookingStatus == BookingEnums.Approved.ToString())
                        .ToList();

        foreach (var booking in bookingList)
        {
            // UnixTimestampMinCancellation là thời gian tối thiểu để hủy booking (này bao gồm cả ngày và giờ rồi)
            DateTime datetimeFromUnix = booking.UnixTimestampMinCancellation;
            // Ghép PlayingDate và StartTimePlaying lại
            DateTime playingStartDateTime = booking.PlayingDate.Date.Add(booking.StartTimePlaying);

            // So sánh thời gian hủy với thời gian hiện tại
            TimeSpan timeDifference = DateTime.Now - datetimeFromUnix;

            if (timeDifference >= TimeSpan.Zero)
            {
                // Thời gian hủy nhỏ hơn hoặc bằng thời gian hiện tại
                //throw new BadRequestException($"Không thể hủy đặt sân, thời gian tối thiểu để hủy lịch đặt sân đã trôi qua. Thời gian bị lệch: {timeDifference}");
                var transaction = _beatSportsDbContext.Transactions
                                .Where(x => x.Id == booking.TransactionId && x.AdminCheckStatus == AdminCheckEnums.Pending)
                                .FirstOrDefault();
                if (transaction != null)
                {
                    var ownerWallet = _beatSportsDbContext.Wallets
                                .Where(x => x.Id == transaction.WalletTargetId)
                                .FirstOrDefault();
                    // Không hiểu nổi 
                    transaction.AdminCheckStatus = AdminCheckEnums.Accepted;
                    // Hoàn thành transaction này để đánh dấu nó approve
                    transaction.TransactionStatus = TransactionEnum.Approved.ToString();
                    // Cộng vào ví owner
                    ownerWallet.Balance += (int)transaction.TransactionAmount;

                    _beatSportsDbContext.Transactions.Update(transaction);
                    _beatSportsDbContext.Wallets.Update(ownerWallet);
                    _beatSportsDbContext.SaveChanges();
                }

            }
        }
    }

    public void CheckBookingPlayDateIfFinish()
    {
        var bookingList = _beatSportsDbContext.Bookings
            .Include(cs => cs.CourtSubdivision)
            .ThenInclude(c => c.Court)
            .Where(x => !x.IsDelete && x.BookingStatus == BookingEnums.Approved.ToString())
            .ToList();

        foreach (var booking in bookingList)
        {
            var endDatePlaying = booking.PlayingDate.Add(booking.EndTimePlaying);
            if (endDatePlaying <= DateTime.Now)
            {
                booking.BookingStatus = BookingEnums.Finished.ToString();

                // Tạo thông báo feedback
                var customer = _beatSportsDbContext.Customers.Where(a => a.Id == booking.CustomerId).SingleOrDefault();
                var notification = _beatSportsDbContext.Notifications.Where(n => n.BookingId == booking.Id.ToString()).SingleOrDefault();
                if (notification != null)
                {
                    notification.Title = "Đánh giá sân";
                    notification.Message = $"hãy để lại phản hồi cho sân {booking.CourtSubdivision.Court.CourtName} mà bạn đã chơi.";
                    notification.IsRead = false;
                    notification.Type = "Feedback";

                    _beatSportsDbContext.Notifications.Update(notification);
                }
            }
        }
        _beatSportsDbContext.SaveChanges();
    }

    public void RemoveRoomWhenExpired()
    {
        // phòng sẽ giữ lại cho đến khi các thành viên xác nhận kết quả của trận đấu xong hết, thì sau 1 ngày thì cái phòng đó sẽ đóng vĩnh viễn

        // 1. Kiểm tra tất cả thành viên trong bảng roomMember của roomMatch đó đã cập nhật kết quả sau trận đấu chưa
        // (dựa theo thời gian kết thúc của bảng roomMatch)
        var expiredRooms = _beatSportsDbContext.RoomMatches
            .Where(x => !x.IsDelete && x.EndTimeRoom <= DateTime.Now).ToList();

        foreach (var room in expiredRooms)
        {
            var expiredRoomMemberNoResult = _beatSportsDbContext.RoomMembers
                .Where(rm => rm.RoomMatchId == room.Id
                            && rm.MatchingResultStatus == "NoResult")
                .ToList();
            if (expiredRoomMemberNoResult.Any())
            {
                foreach (var roomMember in expiredRoomMemberNoResult)
                {
                    // 2. Nếu chưa cập nhật kq trận đấu thì gửi mail và thông báo, cách 30p, 1 tiếng gửi, ... 1 lần cho cập nhật
                    var customer = _beatSportsDbContext.Customers.Where(c => c.Id == roomMember.CustomerId).SingleOrDefault();
                    var notification = new Notification
                    {
                        AccountId = customer.AccountId,
                        Title = "Cập nhật kết quả cho trận đấu",
                        Message = "bạn hay vào phần kết quả của trận đấu, cập nhật đội thắng giúp chủ phòng nhận lại tiền.",
                        IsRead = false,
                        Type = "ResultRoomMatch"
                    };
                    _beatSportsDbContext.Notifications.Add(notification);

                    var account = _beatSportsDbContext.Accounts.Where(a => a.Id == customer.AccountId).SingleOrDefault();
                    var roomMatch = _beatSportsDbContext.RoomMatches.Where(rm => rm.Id == room.Id).SingleOrDefault();
                    var bookingRoomMatch = _beatSportsDbContext.Bookings.Where(rm => rm.Id == roomMatch.BookingId).SingleOrDefault();
                    var court = _beatSportsDbContext.Courts.Where(rm => rm.Id == bookingRoomMatch.CourtSubdivisionId).SingleOrDefault();
                    if (account != null)
                    {
                        if (!string.IsNullOrEmpty(account.Email))
                        {
                            _emailService.SendEmailAsync(
                            account.Email,
                            "Trận đấu đã kết thúc - Cập nhật kết quả",
                            $@"<html>
                                <head>
                                    <style>
                                        body {{
                                            font-family: Montserrat, sans-serif;
                                            margin: 0;
                                            padding: 0;
                                            background-color: #f4f4f4;
                                        }}
                                        .container {{
                                            width: 100%;
                                            max-width: 600px;
                                            margin: 0 auto;
                                            background-color: #ffffff;
                                            padding: 20px;
                                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                        }}
                                        .header {{
                                            background-color: #007bff;
                                            color: #ffffff;
                                            padding: 10px 0;
                                            text-align: center;
                                            font-size: 24px;
                                        }}
                                        .content {{
                                            margin: 20px 0;
                                            line-height: 1.6;
                                            text-align: center;
                                        }}
                                        .footer {{
                                            margin: 20px 0;
                                            text-align: center;
                                            color: #777;
                                            font-size: 12px;
                                        }}
                                        .button {{
                                            background-color: #45B441;
                                            color: #ffffff;
                                            padding: 10px 20px;
                                            text-decoration: none;
                                            border-radius: 5px;
                                            display: inline-block;
                                            font-size: 16px;
                                            margin-top: 20px;
                                        }}
                                        .match-info {{
                                            text-align: left;
                                            margin-top: 20px;
                                            font-size: 14px;
                                        }}
                                        .match-info p {{
                                            margin: 5px 0;
                                        }}
                                        .match-info .title {{
                                            font-weight: bold;
                                            color: #111111;
                                        }}
                                    </style>
                                </head>
                                <body>
                                    <div class='container'>
                                        <div class='header'>
                                            Trận đấu đã kết thúc
                                        </div>
                                        <div class='content'>
                                            <p>Kính gửi {account.FirstName + " " + account.LastName},</p>
                                            <p>Trận đấu bạn tham gia đã kết thúc. Vui lòng cập nhật kết quả trận đấu để xác minh chi phí thanh toán cho chủ phòng.</p>
                                            <p>Việc xác nhận kết quả sẽ giúp đảm bảo tính minh bạch và công bằng trong hệ thống thanh toán.</p>
                                            <a href='beatsportsappuser://matching/matching-badminton-detail/{roomMatch.Id}' class='button'>Cập nhật kết quả</a>

                                            <div class='match-info'>
                                                <p class='title'>Thông tin trận đấu:</p>
                                                <p><strong>Thời gian thi đấu:</strong> {roomMatch.StartTimeRoom.ToString("HH:mm dd/MM/yyyy")}</p>
                                                <p><strong>Sân đấu:</strong> {court.CourtName}</p>
                                                <p><strong>Địa chỉ:</strong> {court.Address}</p>
                                            </div>
                                        </div>
                                        <div class='footer'>
                                            <p>© 2024 BeatSports. All rights reserved.</p>
                                        </div>
                                    </div>
                                </body>
                            </html>"
                            );
                        }
                    }

                }
            }
            else
            {
                // 3. Nếu cập nhật đầy đủ rồi thì cộng tiền đó vào ví cho chủ phòng, trả tiền thừa của thành viên đội thắng
                // đội thắng
                var roomMemberWin = _beatSportsDbContext.RoomMembers
                                                    .Where(rm => rm.RoomMatchId == room.Id
                                                                && rm.MatchingResultStatus == "Win")
                                                    .ToList();
                // đội thua
                var roomMemberLose = _beatSportsDbContext.RoomMembers
                                                    .Where(rm => rm.RoomMatchId == room.Id
                                                                && rm.MatchingResultStatus == "Lose")
                                                    .ToList();

                // chia tiền thắng
                var bookingRoomMatch = _beatSportsDbContext.Bookings.Where(rm => rm.Id == room.BookingId).SingleOrDefault();
                var ratingRoomExist = _beatSportsDbContext.RatingRooms.Where(rm => rm.Id == room.RatingRoomId).SingleOrDefault();
                var teamSize = (decimal)room.MaximumMember / 2;
                var teamCost = (bookingRoomMatch.TotalAmount * (decimal)(ratingRoomExist?.LoseRatePercent ?? 0));
                var costTeamWin = teamCost / teamSize;

                decimal totalAmountRefund = 0;
                decimal amountMemberReceiveRefund = 0;
                decimal totalAmountWin = 0;
                decimal totalAmountLose = 0;

                // lấy danh sách transaction thua
                foreach (var memberLose in roomMemberLose)
                {
                    var customer = _beatSportsDbContext.Customers.Where(c => c.Id == memberLose.CustomerId).SingleOrDefault();
                    var wallet = _beatSportsDbContext.Wallets.Where(a => a.AccountId == customer.AccountId).SingleOrDefault();

                    var transactionJoinRoomMatch = _beatSportsDbContext.Transactions
                                                   .Where(t => t.RoomMatchId == room.Id
                                                               && t.WalletId == wallet.Id
                                                               && t.TransactionType == "JoinRoom")
                                                   // [TransactionStatus] == Pending đang define để sử dụng, nếu Accepted thì ko thể outRoom
                                                   .SingleOrDefault();
                    // team thua trả đủ
                    if (transactionJoinRoomMatch != null)
                    {
                        totalAmountLose += (transactionJoinRoomMatch.TransactionAmount
                                                ?? throw new BadRequestException("Đã có lỗi, giao dịch tham gia phòng có số dư bằng 0."));
                    }
                }

                // lấy danh sách transaction thắng
                foreach (var memberWin in roomMemberWin)
                {
                    var customer = _beatSportsDbContext.Customers.Where(c => c.Id == memberWin.CustomerId).SingleOrDefault();
                    var wallet = _beatSportsDbContext.Wallets.Where(a => a.AccountId == customer.AccountId).SingleOrDefault();

                    var transactionJoinRoomMatch = _beatSportsDbContext.Transactions
                                                   .Where(t => t.RoomMatchId == room.Id
                                                               && t.WalletId == wallet.Id
                                                               && t.TransactionType == "JoinRoom")
                                                   // [TransactionStatus] == Pending đang define để sử dụng, nếu Accepted thì ko thể outRoom
                                                   .SingleOrDefault();
                    // team thắng dùng costTeamWin trả tiền. Lấy TransactionAmount - costTeamWin ra số dư trả lại cho thành viên
                    if (transactionJoinRoomMatch != null)
                    {
                        totalAmountRefund += ((transactionJoinRoomMatch.TransactionAmount
                            ?? throw new BadRequestException("Đã có lỗi, giao dịch tham gia phòng có số dư bằng 0.")) - costTeamWin);
                        totalAmountWin += costTeamWin;

                        wallet.Balance += ((transactionJoinRoomMatch.TransactionAmount
                            ?? throw new BadRequestException("Đã có lỗi, giao dịch tham gia phòng có số dư bằng 0.")) - costTeamWin);
                        _beatSportsDbContext.Wallets.Update(wallet);
                        // thanh toán tiền các thành viên tham gia phòng, tiền trả cho thành viên thắng
                        var refundMemberTransaction = new Domain.Entities.Transaction
                        {
                            WalletId = wallet.Id,
                            TransactionMessage = "Hoàn trả tiền cho thành viên đội thắng cuộc thành công.",
                            TransactionStatus = "Approved",
                            TransactionAmount = (totalAmountWin + totalAmountLose),
                            TransactionDate = DateTime.Now,
                            TransactionType = "RefundRoomMember",
                            RoomMatchId = room.Id,
                        };
                        _beatSportsDbContext.Transactions.Add(refundMemberTransaction);
                    }
                }

                // cộng tiền đội thắng và đội thua lại cho chủ phòng
                var roomMasterInRoomMember = _beatSportsDbContext.RoomMembers
                                                    .Where(rm => rm.RoomMatchId == room.Id
                                                                && rm.RoleInRoom == RoleInRoomEnums.Master)
                                                    .SingleOrDefault();
                var roomMasterCus = _beatSportsDbContext.Customers.Where(c => c.Id == roomMasterInRoomMember.CustomerId).SingleOrDefault();
                var walletRoomMaster = _beatSportsDbContext.Wallets.Where(a => a.AccountId == roomMasterCus.AccountId).SingleOrDefault();

                walletRoomMaster.Balance += (totalAmountWin + totalAmountLose);
                _beatSportsDbContext.Wallets.Update(walletRoomMaster);
                // thanh toán tiền các thành viên tham gia phòng, tiền trả cho chủ phòng
                var refundMasterTransaction = new Domain.Entities.Transaction
                {
                    WalletId = walletRoomMaster.Id,
                    TransactionMessage = "Tiền sân đã được các thành viên trong nhóm hoàn trả cho chủ phòng thành công.",
                    TransactionStatus = "Approved",
                    TransactionAmount = (totalAmountWin + totalAmountLose),
                    TransactionDate = DateTime.Now,
                    TransactionType = "RefundRoomMaster",
                    RoomMatchId = room.Id,
                };
                _beatSportsDbContext.Transactions.Add(refundMasterTransaction);
            }
        }

        // 4. Để phòng đó sau 1 ngày thì mới remove như bên dưới
        // Lấy thời gian hiện tại
        var currentDateTime = DateTime.Now;
        var expiredTransactionRoomMatch = _beatSportsDbContext.Transactions
                                .Where(x => !x.IsDelete
                                            && x.RoomMatchId.HasValue
                                            && x.TransactionType == "RefundRoomMaster")
                                .ToList();
        // Lọc các giao dịch đã quá hạn (tức là đã tồn tại hơn 1 ngày)
        var expiredTransactions = expiredTransactionRoomMatch
            .Where(t => ((TimeSpan)(currentDateTime - t.TransactionDate)).TotalDays > 1)
            .ToList();

        // Xóa các phòng đã hết hạn
        foreach (var transaction in expiredTransactions)
        {
            transaction.IsDelete = true; // vì xóa rồi, thì lần sau sẽ ko xóa nữa, dùng xóa mềm
            _beatSportsDbContext.Transactions.Update(transaction);

            // Nếu cần, cập nhật thời gian xóa hoặc lý do xóa
            // Lấy và xóa tất cả RoomMembers liên quan đến phòng hết hạn
            var roomMembers = _beatSportsDbContext.RoomMembers
                .Where(rm => rm.RoomMatchId == transaction.RoomMatchId).ToList();
            _beatSportsDbContext.RoomMembers.RemoveRange(roomMembers);

            // Lấy và xóa tất cả RoomJoiningRequests liên quan đến phòng hết hạn
            var roomJoiningRequests = _beatSportsDbContext.RoomRequests
                .Where(rj => rj.RoomMatchId == transaction.RoomMatchId).ToList();
            _beatSportsDbContext.RoomRequests.RemoveRange(roomJoiningRequests);

            // Xóa chính phòng đã hết hạn
            var roomMacth = _beatSportsDbContext.RoomMatches
                .Where(x => !x.IsDelete && x.Id == transaction.RoomMatchId).SingleOrDefault();
            _beatSportsDbContext.RoomMatches.Remove(roomMacth);
        }

        _beatSportsDbContext.SaveChanges();
    }

    public void NotificationForOwnerPayFee()
    {
        var today = DateTime.Now;
        if (today.Day == 1)
        {
            var owners = _beatSportsDbContext.Owners.ToList();
            foreach (var owner in owners)
            {
                var notification = new Notification
                {
                    AccountId = owner.AccountId,
                    Title = "Thanh toán phí dịch vụ",
                    Message = "đã đến ngày 10, vui lòng thanh toán phí dịch vụ quản lý sân.",
                    IsRead = false,
                    Type = "PayFee"
                };
                _beatSportsDbContext.Notifications.Add(notification);

                var account = _beatSportsDbContext.Accounts.Where(a => a.Id == owner.AccountId).SingleOrDefault();
                if (account != null)
                {
                    if (!string.IsNullOrEmpty(account.Email))
                    {
                        _emailService.SendEmailAsync(
                        account.Email,
                        "Thanh toán phí dịch vụ - BeatSports",
                        $@"<html>
                        <head>
                            <style>
                                body {{
                                    font-family: Montserrat, sans-serif;
                                    margin: 0;
                                    padding: 0;
                                    background-color: #f4f4f4;
                                }}
                                .container {{
                                    width: 100%;
                                    max-width: 600px;
                                    margin: 0 auto;
                                    background-color: #ffffff;
                                    padding: 20px;
                                    box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                                }}
                                .header {{
                                    background-color: #007bff;
                                    color: #ffffff;
                                    padding: 10px 0;
                                    text-align: center;
                                    font-size: 24px;
                                }}
                                .content {{
                                    margin: 20px 0;
                                    line-height: 1.6;
                                    text-align: center;
                                }}
                                .footer {{
                                    margin: 20px 0;
                                    text-align: center;
                                    color: #777;
                                    font-size: 12px;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    Thanh toán phí dịch vụ
                                </div>
                                <div class='content'>
                                    <p>Kính gửi {account.FirstName + " " + account.LastName},</p>
                                    <p>{notification.Message}</p>
                                    <p>Vui lòng đăng nhập vào hệ thống và hoàn tất thanh toán để tránh bất kỳ sự gián đoạn nào trong việc sử dụng dịch vụ.</p>
                                </div>
                                <div class='footer'>
                                    <p>© 2024 BeatSports. All rights reserved.</p>
                                </div>
                            </div>
                        </body>
                        </html>"
                        );
                    }
                }
            }
            _beatSportsDbContext.SaveChanges();
        }
    }
}