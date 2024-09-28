using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Hubs;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatchesUpdateResult;
public class RoomMatchesUpdateResultCommand : IRequest<BeatSportsResponse>
{
    public Guid RoomMatchId { get; set; }
    public Guid CustomerId { get; set; }
    public string? Team { get; set; }
}

public class RoomMatchesUpdateResultCommandHandler : IRequestHandler<RoomMatchesUpdateResultCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IHubContext<RoomRequestHub> _hubContext;
    private readonly IEmailService _emailService;

    public RoomMatchesUpdateResultCommandHandler(IBeatSportsDbContext dbContext, IHubContext<RoomRequestHub> hubContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _emailService = emailService;
    }

    public async Task<BeatSportsResponse> Handle(RoomMatchesUpdateResultCommand request, CancellationToken cancellationToken)
    {
        var roomMemberExist = await _dbContext.RoomMembers
                    .Where(rm => rm.CustomerId == request.CustomerId
                                && rm.RoomMatchId == request.RoomMatchId)
                    .SingleOrDefaultAsync();
        if (roomMemberExist == null)
        {
            throw new NotFoundException("Đã có lỗi, thành viên hoặc phòng đấu không tồn tại.");
        }
        if (string.IsNullOrEmpty(request.Team))
        {
            throw new BadRequestException("Vui lòng chọn đội chiến thắng.");
        }
        roomMemberExist.MatchingResultStatus = (request.Team == "A" ? "VotedTeamA" : "VotedTeamB");
        _dbContext.RoomMembers.Update(roomMemberExist);
        await _dbContext.SaveChangesAsync();
        // Check coi car phong da update vote het chua, 
        // neu roi thi cap nhat ket qua tien o day
        var expiredRoomMemberNoResult = _dbContext.RoomMembers
                .Where(rm => rm.RoomMatchId == request.RoomMatchId
                            && rm.MatchingResultStatus == "NoResult")
                .ToList();
        if (!expiredRoomMemberNoResult.Any())
        {
            var room = _dbContext.RoomMatches.Where(x => x.Id == request.RoomMatchId && !x.IsDelete).SingleOrDefault();
            // 3. Nếu cập nhật đầy đủ rồi thì cộng tiền đó vào ví cho chủ phòng, trả tiền thừa của thành viên đội thắng
            // tính tổng phiếu vote cho team A
            var voteTeamA = _dbContext.RoomMembers
                                                .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                            && rm.MatchingResultStatus == "VotedTeamA")
                                                .ToList();
            // tính tổng phiếu vote cho team B
            var voteTeamB = _dbContext.RoomMembers
                                                .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                            && rm.MatchingResultStatus == "VotedTeamB")
                                                .ToList();
            // kiểm tra nếu vote == nhau thì reset gửi mail, thông báo cho các thành viên vote lại
            if (voteTeamA.Count == voteTeamB.Count)
            {
                if (room.VoteCount > 2)
                {
                    return new BeatSportsResponse
                    {
                        Message = "Phòng này đã bỏ phiếu đủ, vui lòng không bỏ phiếu lại",
                    };
                }
                else if (room.VoteCount < 2)
                {
                    room.VoteCount++;
                    var memberInRoom = _dbContext.RoomMembers.Where(rm => rm.RoomMatchId == request.RoomMatchId).ToList();
                    foreach (var member in memberInRoom)
                    {
                        member.MatchingResultStatus = "NoResult";
                        _dbContext.RoomMembers.Update(member);
                        var customer = _dbContext.Customers.Where(c => c.Id == member.CustomerId).SingleOrDefault();
                        var notification = new Notification
                        {
                            AccountId = customer.AccountId,
                            Title = "Cập nhật lại kết quả cho trận đấu",
                            Message = "các thành viên trong phòng đã bỏ phiếu bằng nhau, yêu cầu các thành viên cập nhật lại cho đúng với kết quả.",
                            IsRead = false,
                            Type = "ResultRoomMatch",
                            RoomMatchId = request.RoomMatchId.ToString()
                        };
                        _dbContext.Notifications.Add(notification);

                        var account = _dbContext.Accounts.Where(a => a.Id == customer.AccountId).SingleOrDefault();
                        var bookingRoomMatch = _dbContext.Bookings.Where(rm => rm.Id == room.BookingId).SingleOrDefault();
                        if (bookingRoomMatch == null)
                        {
                            throw new NotFoundException("Đã có lỗi, đơn hàng không tồn tại trong phòng đấu.");
                        }
                        var courtSub = _dbContext.CourtSubdivisions.Where(rm => rm.Id == bookingRoomMatch.CourtSubdivisionId).SingleOrDefault();
                        if (courtSub == null)
                        {
                            throw new NotFoundException("Đã có lỗi, không tìm thấy sân nhỏ của đơn hàng.");
                        }
                        var court = _dbContext.Courts.Where(rm => rm.Id == courtSub.CourtId).SingleOrDefault();
                        if (court == null)
                        {
                            throw new NotFoundException("Đã có lỗi, không tìm thấy sân lớn.");
                        }
                        if (account != null)
                        {
                            if (!string.IsNullOrEmpty(account.Email))
                            {
                                await _emailService.SendEmailAsync(
                                account.Email,
                                "Cập nhật lại kết quả trận đấu - Kết quả phiếu bầu hòa",
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
                                                Cập nhật lại kết quả trận đấu
                                            </div>
                                            <div class='content'>
                                                <p>Kính gửi {account.FirstName + " " + account.LastName},</p>
                                                <p>Do kết quả phiếu bầu hiện tại giữa các đội đang hòa, chúng tôi cần bạn cập nhật lại kết quả trận đấu để xác định chính xác đội chiến thắng.</p>
                                                <p>Việc này sẽ đảm bảo tính công bằng và minh bạch trong hệ thống thanh toán.</p>
                                                <a href='beatsportsappuser://matching/matching-badminton-detail/{request.RoomMatchId}' class='button'>Cập nhật lại kết quả</a>

                                                <div class='match-info'>
                                                    <p class='title'>Thông tin trận đấu:</p>
                                                    <p><strong>Thời gian thi đấu:</strong> {room.StartTimeRoom.ToString("HH:mm dd/MM/yyyy")}</p>
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
                else if (room.VoteCount == 2)
                {
                    room.VoteCount++;
                    var roomMemberA = _dbContext.RoomMembers
                                               .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                           && rm.Team == "A")
                                               .ToList();

                    var roomMemberB = _dbContext.RoomMembers
                                                        .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                                    && rm.Team == "B")
                                                        .ToList();
                    // chia tiền thắng
                    var bookingRoomMatch = _dbContext.Bookings.Where(rm => rm.Id == room.BookingId).SingleOrDefault();
                    var ratingRoomExist = _dbContext.RatingRooms.Where(rm => rm.Id == room.RatingRoomId).SingleOrDefault();
                    var teamSize = (decimal)room.MaximumMember / 2;
                    var teamCost = (bookingRoomMatch.TotalAmount * (decimal)0.5);
                    var costTeamWin = teamCost / teamSize;

                    decimal totalAmountRefund = 0;
                    decimal amountMemberReceiveRefund = 0;
                    decimal totalAmountWin = 0;
                    decimal totalAmountLose = 0;

                    // lấy danh sách transaction A
                    foreach (var memberWin in roomMemberA)
                    {
                        var customer = _dbContext.Customers.Where(c => c.Id == memberWin.CustomerId).SingleOrDefault();
                        var wallet = _dbContext.Wallets.Where(a => a.AccountId == customer.AccountId).SingleOrDefault();

                        var transactionJoinRoomMatch = _dbContext.Transactions
                                                       .Where(t => t.RoomMatchId == request.RoomMatchId
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
                            _dbContext.Wallets.Update(wallet);
                            // thanh toán tiền các thành viên tham gia phòng, tiền trả cho thành viên thắng
                            var refundMemberTransaction = new Domain.Entities.Transaction
                            {
                                WalletId = wallet.Id,
                                TransactionMessage = "Hoàn trả tiền cho thành viên đội thắng cuộc thành công.",
                                TransactionStatus = "Approved",
                                AdminCheckStatus = AdminCheckEnums.Accepted,
                                TransactionAmount = ((transactionJoinRoomMatch.TransactionAmount
                                                ?? throw new BadRequestException("Đã có lỗi, giao dịch tham gia phòng có số dư bằng 0.")) - costTeamWin),
                                TransactionDate = DateTime.Now,
                                TransactionType = "RefundRoomMember",
                                RoomMatchId = request.RoomMatchId,
                            };
                            _dbContext.Transactions.Add(refundMemberTransaction);
                        }
                    }

                    // lấy danh sách transaction B
                    foreach (var memberWin in roomMemberB)
                    {
                        var customer = _dbContext.Customers.Where(c => c.Id == memberWin.CustomerId).SingleOrDefault();
                        var wallet = _dbContext.Wallets.Where(a => a.AccountId == customer.AccountId).SingleOrDefault();

                        var transactionJoinRoomMatch = _dbContext.Transactions
                                                       .Where(t => t.RoomMatchId == request.RoomMatchId
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
                            _dbContext.Wallets.Update(wallet);
                            // thanh toán tiền các thành viên tham gia phòng, tiền trả cho thành viên thắng
                            var refundMemberTransaction = new Domain.Entities.Transaction
                            {
                                WalletId = wallet.Id,
                                TransactionMessage = "Hoàn trả tiền cho thành viên đội thắng cuộc thành công.",
                                TransactionStatus = "Approved",
                                AdminCheckStatus = AdminCheckEnums.Accepted,
                                TransactionAmount = ((transactionJoinRoomMatch.TransactionAmount
                                                ?? throw new BadRequestException("Đã có lỗi, giao dịch tham gia phòng có số dư bằng 0.")) - costTeamWin),
                                TransactionDate = DateTime.Now,
                                TransactionType = "RefundRoomMember",
                                RoomMatchId = request.RoomMatchId,
                            };
                            _dbContext.Transactions.Add(refundMemberTransaction);
                        }
                    }

                    // cộng tiền đội thắng và đội thua lại cho chủ phòng
                    var roomMasterInRoomMember = _dbContext.RoomMembers
                                                        .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                                    && rm.RoleInRoom == RoleInRoomEnums.Master)
                                                        .SingleOrDefault();
                    var roomMasterCus = _dbContext.Customers.Where(c => c.Id == roomMasterInRoomMember.CustomerId).SingleOrDefault();
                    var walletRoomMaster = _dbContext.Wallets.Where(a => a.AccountId == roomMasterCus.AccountId).SingleOrDefault();

                    walletRoomMaster.Balance += (totalAmountWin + totalAmountLose);
                    _dbContext.Wallets.Update(walletRoomMaster);
                    // thanh toán tiền các thành viên tham gia phòng, tiền trả cho chủ phòng
                    var refundMasterTransaction = new Domain.Entities.Transaction
                    {
                        WalletId = walletRoomMaster.Id,
                        TransactionMessage = "Tiền sân đã được các thành viên trong nhóm hoàn trả cho chủ phòng thành công.",
                        AdminCheckStatus = AdminCheckEnums.Accepted,
                        TransactionStatus = "Approved",
                        TransactionAmount = (totalAmountWin + totalAmountLose),
                        TransactionDate = DateTime.Now,
                        TransactionType = "RefundRoomMaster",
                        RoomMatchId = request.RoomMatchId,
                    };
                    _dbContext.Transactions.Add(refundMasterTransaction);
                }
            }

            // chênh nhau hoặc tất cả về 1 vote, thì xem như team đó thắng
            // Team B thắng
            else if (voteTeamA.Count < voteTeamB.Count)
            {
                var roomMemberLose = _dbContext.RoomMembers
                                               .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                           && rm.Team == "A")
                                               .ToList();

                var roomMemberWin = _dbContext.RoomMembers
                                                    .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                                && rm.Team == "B")
                                                    .ToList();
                // chia tiền thắng
                var bookingRoomMatch = _dbContext.Bookings.Where(rm => rm.Id == room.BookingId).SingleOrDefault();
                var ratingRoomExist = _dbContext.RatingRooms.Where(rm => rm.Id == room.RatingRoomId).SingleOrDefault();
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
                    var customer = _dbContext.Customers.Where(c => c.Id == memberLose.CustomerId).SingleOrDefault();
                    var wallet = _dbContext.Wallets.Where(a => a.AccountId == customer.AccountId).SingleOrDefault();

                    var transactionJoinRoomMatch = _dbContext.Transactions
                                                   .Where(t => t.RoomMatchId == request.RoomMatchId
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
                    var customer = _dbContext.Customers.Where(c => c.Id == memberWin.CustomerId).SingleOrDefault();
                    var wallet = _dbContext.Wallets.Where(a => a.AccountId == customer.AccountId).SingleOrDefault();

                    var transactionJoinRoomMatch = _dbContext.Transactions
                                                   .Where(t => t.RoomMatchId == request.RoomMatchId
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
                        _dbContext.Wallets.Update(wallet);
                        // thanh toán tiền các thành viên tham gia phòng, tiền trả cho thành viên thắng
                        var refundMemberTransaction = new Domain.Entities.Transaction
                        {
                            WalletId = wallet.Id,
                            TransactionMessage = "Hoàn trả tiền cho thành viên đội thắng cuộc thành công.",
                            TransactionStatus = "Approved",
                            AdminCheckStatus = AdminCheckEnums.Accepted,
                            TransactionAmount = ((transactionJoinRoomMatch.TransactionAmount
                                            ?? throw new BadRequestException("Đã có lỗi, giao dịch tham gia phòng có số dư bằng 0.")) - costTeamWin),
                            TransactionDate = DateTime.Now,
                            TransactionType = "RefundRoomMember",
                            RoomMatchId = request.RoomMatchId,
                        };
                        _dbContext.Transactions.Add(refundMemberTransaction);
                    }
                }

                // cộng tiền đội thắng và đội thua lại cho chủ phòng
                var roomMasterInRoomMember = _dbContext.RoomMembers
                                                    .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                                && rm.RoleInRoom == RoleInRoomEnums.Master)
                                                    .SingleOrDefault();
                var roomMasterCus = _dbContext.Customers.Where(c => c.Id == roomMasterInRoomMember.CustomerId).SingleOrDefault();
                var walletRoomMaster = _dbContext.Wallets.Where(a => a.AccountId == roomMasterCus.AccountId).SingleOrDefault();

                walletRoomMaster.Balance += (totalAmountWin + totalAmountLose);
                _dbContext.Wallets.Update(walletRoomMaster);
                // thanh toán tiền các thành viên tham gia phòng, tiền trả cho chủ phòng
                var refundMasterTransaction = new Domain.Entities.Transaction
                {
                    WalletId = walletRoomMaster.Id,
                    TransactionMessage = "Tiền sân đã được các thành viên trong nhóm hoàn trả cho chủ phòng thành công.",
                    AdminCheckStatus = AdminCheckEnums.Accepted,
                    TransactionStatus = "Approved",
                    TransactionAmount = (totalAmountWin + totalAmountLose),
                    TransactionDate = DateTime.Now,
                    TransactionType = "RefundRoomMaster",
                    RoomMatchId = request.RoomMatchId,
                };
                _dbContext.Transactions.Add(refundMasterTransaction);
            }
            // Team A thắng
            else if (voteTeamA.Count > voteTeamB.Count)
            {
                var roomMemberWin = _dbContext.RoomMembers
                                               .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                           && rm.Team == "A")
                                               .ToList();

                var roomMemberLose = _dbContext.RoomMembers
                                                    .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                                && rm.Team == "B")
                                                    .ToList();
                // chia tiền thắng
                var bookingRoomMatch = _dbContext.Bookings.Where(rm => rm.Id == room.BookingId).SingleOrDefault();
                var ratingRoomExist = _dbContext.RatingRooms.Where(rm => rm.Id == room.RatingRoomId).SingleOrDefault();
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
                    var customer = _dbContext.Customers.Where(c => c.Id == memberLose.CustomerId).SingleOrDefault();
                    var wallet = _dbContext.Wallets.Where(a => a.AccountId == customer.AccountId).SingleOrDefault();

                    var transactionJoinRoomMatch = _dbContext.Transactions
                                                   .Where(t => t.RoomMatchId == request.RoomMatchId
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
                    var customer = _dbContext.Customers.Where(c => c.Id == memberWin.CustomerId).SingleOrDefault();
                    var wallet = _dbContext.Wallets.Where(a => a.AccountId == customer.AccountId).SingleOrDefault();

                    var transactionJoinRoomMatch = _dbContext.Transactions
                                                   .Where(t => t.RoomMatchId == request.RoomMatchId
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
                        _dbContext.Wallets.Update(wallet);
                        // thanh toán tiền các thành viên tham gia phòng, tiền trả cho thành viên thắng
                        var refundMemberTransaction = new Domain.Entities.Transaction
                        {
                            WalletId = wallet.Id,
                            TransactionMessage = "Hoàn trả tiền cho thành viên đội thắng cuộc thành công.",
                            TransactionStatus = "Approved",
                            AdminCheckStatus = AdminCheckEnums.Accepted,
                            TransactionAmount = ((transactionJoinRoomMatch.TransactionAmount
                                            ?? throw new BadRequestException("Đã có lỗi, giao dịch tham gia phòng có số dư bằng 0.")) - costTeamWin),
                            TransactionDate = DateTime.Now,
                            TransactionType = "RefundRoomMember",
                            RoomMatchId = request.RoomMatchId,
                        };
                        _dbContext.Transactions.Add(refundMemberTransaction);
                    }
                }

                // cộng tiền đội thắng và đội thua lại cho chủ phòng
                var roomMasterInRoomMember = _dbContext.RoomMembers
                                                    .Where(rm => rm.RoomMatchId == request.RoomMatchId
                                                                && rm.RoleInRoom == RoleInRoomEnums.Master)
                                                    .SingleOrDefault();
                var roomMasterCus = _dbContext.Customers.Where(c => c.Id == roomMasterInRoomMember.CustomerId).SingleOrDefault();
                var walletRoomMaster = _dbContext.Wallets.Where(a => a.AccountId == roomMasterCus.AccountId).SingleOrDefault();

                walletRoomMaster.Balance += (totalAmountWin + totalAmountLose);
                _dbContext.Wallets.Update(walletRoomMaster);
                // thanh toán tiền các thành viên tham gia phòng, tiền trả cho chủ phòng
                var refundMasterTransaction = new Domain.Entities.Transaction
                {
                    WalletId = walletRoomMaster.Id,
                    TransactionMessage = "Tiền sân đã được các thành viên trong nhóm hoàn trả cho chủ phòng thành công.",
                    TransactionStatus = "Approved",
                    AdminCheckStatus = AdminCheckEnums.Accepted,
                    TransactionAmount = (totalAmountWin + totalAmountLose),
                    TransactionDate = DateTime.Now,
                    TransactionType = "RefundRoomMaster",
                    RoomMatchId = request.RoomMatchId,
                };
                _dbContext.Transactions.Add(refundMasterTransaction);
            }
        }
        await _dbContext.SaveChangesAsync();
        return new BeatSportsResponse
        {
            Message = "Vote recorded successfully.",
        };
    }
}