using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using BeatSportsAPI.Domain.Entities.Room;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using BeatSportsAPI.Application.Features.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.CreateRoomRequests;
public class CreateRoomRequestHandler : IRequestHandler<CreateRoomRequestCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IHubContext<RoomRequestHub> _hubContext;
    private readonly IEmailService _emailService;

    public CreateRoomRequestHandler(IBeatSportsDbContext beatSportsDbContext, IHubContext<RoomRequestHub> hubContext, IEmailService emailService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _hubContext = hubContext;
        _emailService = emailService;
    }

    public async Task<BeatSportsResponse> Handle(CreateRoomRequestCommand request, CancellationToken cancellationToken)
    {
        var roomMatch = _beatSportsDbContext.RoomMatches
            .Where(rm => rm.Id == request.RoomMatchId && !rm.IsDelete)
            .FirstOrDefault();
        var customer = _beatSportsDbContext.Customers
            .Include(a => a.Account)
            .Where(c => c.Id == request.CustomerId && !c.IsDelete)
            .FirstOrDefault();

        var roomMatchJoinedList = _beatSportsDbContext.RoomRequests
                                .Where(x => x.CustomerId == customer.Id && x.JoinStatus == RoomRequestEnums.Accepted)
                                .ToList();


        // Kiểm tra số lượng thành viên trong phòng
        var currentMemberCount = await _beatSportsDbContext.RoomMembers
            .CountAsync(rm => rm.RoomMatchId == roomMatch.Id);
        if (currentMemberCount >= roomMatch.MaximumMember)
        {
            throw new BadRequestException("Phòng này đã đủ thành viên, bạn hãy tham gia phòng khác.");
        }

        if (roomMatchJoinedList.Count > 0)
        {
            var flag = 0;

            foreach (var room in roomMatchJoinedList)
            {

                var roomMatchCheck = _beatSportsDbContext.RoomMatches
                                .Where(x => x.Id == room.RoomMatchId)
                                .FirstOrDefault();

                if (roomMatch.StartTimeRoom.Date == roomMatchCheck.StartTimeRoom.Date)
                {
                    if (roomMatch.StartTimeRoom <= roomMatchCheck.StartTimeRoom && roomMatch.EndTimeRoom >= roomMatchCheck.EndTimeRoom)
                    {
                        flag++;
                        break;
                    }
                    else if (((roomMatch.StartTimeRoom <= roomMatchCheck.StartTimeRoom) && (roomMatchCheck.StartTimeRoom < roomMatch.EndTimeRoom)))
                    {
                        flag++;
                        break;
                    }
                    else if (((roomMatch.StartTimeRoom < roomMatchCheck.EndTimeRoom) && (roomMatchCheck.EndTimeRoom <= roomMatch.EndTimeRoom)))
                    {
                        flag++;
                        break;
                    }
                }
            }

            if (flag > 0)
            {
                return await Task.FromResult(new BeatSportsResponse
                {
                    Message = "400"
                });
            }

        }

        if (roomMatch == null)
        {
            throw new NotFoundException($"Phòng đã bị xóa, không tìm thấy phòng này.");
        }

        if (customer == null)
        {
            throw new NotFoundException($"{request.CustomerId} is not existed");
        }

        if (roomMatch.IsPrivate == false)
        {
            var roomRequest = new RoomRequest
            {
                CustomerId = customer.Id,
                RoomMatchId = roomMatch.Id,
                JoinStatus = RoomRequestEnums.Accepted,
                DateRequest = DateTime.UtcNow,
            };
            _beatSportsDbContext.RoomRequests.Add(roomRequest);

            var roomMember = new RoomMember
            {
                CustomerId = customer.Id,
                RoomMatchId = roomMatch.Id,
                RoleInRoom = RoleInRoomEnums.Member,
            };
            _beatSportsDbContext.RoomMembers.Add(roomMember);

            await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

            await _hubContext.Clients.Group(roomMatch.Id.ToString()).SendAsync("UpdateRoom", "NewMember", customer.Id);

            return new BeatSportsResponse
            {
                Message = "Joined public room successfully."
            };
        }
        else
        {
            // Kiểm tra số tiền trong ví của customer đó, dựa theo cái roomMatch có lưu TotalCostEachMember so sánh với tiền trong ví rồi trừ
            var walletExistMoney = await _beatSportsDbContext.Wallets
                                    .Where(w => w.AccountId == customer.AccountId)
                                    .SingleOrDefaultAsync();
            if (walletExistMoney == null)
            {
                throw new NotFoundException("Ví tiền không tại, đã xảy ra lỗi.");
            }
            if (walletExistMoney.Balance < (decimal)(roomMatch.TotalCostEachMember ?? 0))
            {
                throw new BadRequestException("Số dư trong ví của bạn không đủ để tham gia phòng, Bạn cần nạp thêm vào ví.");
            }
            walletExistMoney.Balance -= (decimal)(roomMatch.TotalCostEachMember!);
            _beatSportsDbContext.Wallets.Update(walletExistMoney);

            // tạo transaction cho cái giao dịch tham gia phòng, sẽ gồm 3 transaction: Tham gia(), Hoàn trả(), Rời phòng()
            // tham gia
            var joinRoomTransaction = new Transaction
            {
                WalletId = walletExistMoney.Id,
                TransactionMessage = "Tham gia phòng thành công",
                // AdminCheckStatus không liên quan cho transaction này
                // hoàn thành(Approved) hết giờ, tiền trả vô ví của member, đang chờ xử lý(Pending) tham gia phòng thành công nhưng chưa hết giờ,
                // hay đã được hoàn trả(Cancel) thoát khỏi phòng trả tiền lại cho member thì update lại transaction
                TransactionStatus = "Pending", 
                TransactionAmount = (decimal)(roomMatch.TotalCostEachMember!),
                TransactionDate = DateTime.Now,
                TransactionType = "JoinRoom", // đây sẽ là chỗ lưu loại các giao dịch
                RoomMatchId = roomMatch.Id,
            };
            _beatSportsDbContext.Transactions.Add(joinRoomTransaction);

            var roomRequest = new RoomRequest
            {
                CustomerId = customer.Id,
                RoomMatchId = roomMatch.Id,
                JoinStatus = RoomRequestEnums.Pending,
                DateRequest = DateTime.UtcNow,
            };
            _beatSportsDbContext.RoomRequests.Add(roomRequest);
            await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

            // Notify all clients in the room's group
            await _hubContext.Clients.Group(roomMatch.Id.ToString()).SendAsync("UpdateRoom", "NewRequest", customer.Id);

            // Gửi email cho khách hàng
            await _emailService.SendEmailAsync(
                customer.Account.Email,
                "Yêu cầu tham gia phòng đang chờ duyệt",
                $@"
                    <html>
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
                            }}
                            .content p {{
                                margin: 10px 0;
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
                                Yêu cầu tham gia phòng đang chờ duyệt
                            </div>
                            <div class='content'>
                                <p>Chào {customer.Account.FirstName} {customer.Account.LastName},</p>
                                <p>Bạn đã gửi yêu cầu tham gia phòng với các thông tin sau:</p>
                                <p><strong>Tên phòng:</strong> {roomMatch.RoomName}</p>
                                <p><strong>Thể loại:</strong> {roomMatch.SportCategory.GetDescriptionFromEnum()}</p>
                                <p><strong>Thời gian bắt đầu:</strong> {roomMatch.StartTimeRoom}</p>
                                <p><strong>Thời gian kết thúc:</strong> {roomMatch.EndTimeRoom}</p>
                                <p>Vui lòng chờ quản trị viên duyệt yêu cầu của bạn.</p>
                            </div>
                            <div class='footer'>
                                <p>© 2024 BeatSports. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>"
            );

            return await Task.FromResult(new BeatSportsResponse
            {
                Message = "Send Request To Joining Successful, Waiting for Room Master approve"
            });
        }
    }
}