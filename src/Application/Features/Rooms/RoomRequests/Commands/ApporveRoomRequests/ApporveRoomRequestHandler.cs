using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Hubs;
using BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.UpdateRoomRequests;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.ApporveRoomRequests;
public class ApporveRoomRequestHandler : IRequestHandler<ApporveRoomRequestCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IHubContext<RoomRequestHub> _hubContext;
    private readonly IEmailService _emailService;

    public ApporveRoomRequestHandler(IBeatSportsDbContext beatSportsDbContext, IHubContext<RoomRequestHub> hubContext, IEmailService emailService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _hubContext = hubContext;
        _emailService = emailService;
    }

    public async Task<BeatSportsResponse> Handle(ApporveRoomRequestCommand request, CancellationToken cancellationToken)
    {
        var roomRequest = _beatSportsDbContext.RoomRequests
            .Include(c => c.Customer)
                .ThenInclude(a => a.Account)
            .Where(rq => rq.Id == request.RoomRequestId && rq.CustomerId == request.CustomerId && !rq.IsDelete)
            .FirstOrDefault();

        if (roomRequest == null)
        {
            throw new NotFoundException($"{request.RoomRequestId} is not existed");
        }

        var roomMatch = await _beatSportsDbContext.RoomMatches
            .Where(rm => rm.Id == roomRequest.RoomMatchId)
            .FirstOrDefaultAsync();

        if (roomMatch == null)
        {
            throw new NotFoundException($"Room match {roomRequest.RoomMatchId} does not exist");
        }

        switch (request.RoomRequest.ToString())
        {
            case "Accepted":
                // Chủ phòng chấp nhận yêu cầu
                roomRequest.JoinStatus = RoomRequestEnums.Accepted;
                roomRequest.DateApprove = DateTime.UtcNow;

                _beatSportsDbContext.RoomRequests.Update(roomRequest);
                // Khi được chấp nhận, thì RoomMatch có thêm thành viên
                var roomMember = new RoomMember
                {
                    CustomerId = roomRequest.CustomerId,
                    RoomMatchId = roomRequest.RoomMatchId,
                    RoleInRoom = RoleInRoomEnums.Member,
                };
                _beatSportsDbContext.RoomMembers.Add(roomMember);
                // Gửi sự kiện SignalR
                await _hubContext.Clients.Group(roomRequest.RoomMatchId.ToString()).SendAsync("UpdateRoom", "RequestAccepted", roomRequest.CustomerId);
                // Gửi email cho khách hàng
                await _emailService.SendEmailAsync(
                    roomRequest.Customer.Account.Email,
                    "Yêu cầu tham gia phòng đã được chấp nhận",
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
                                    Yêu cầu tham gia phòng đã được chấp nhận
                                </div>
                                <div class='content'>
                                    <p>Chào {roomRequest.Customer.Account.FirstName} {roomRequest.Customer.Account.LastName},</p>
                                    <p>Yêu cầu tham gia phòng của bạn đã được chấp nhận với các thông tin sau:</p>
                                    <p><strong>Tên phòng:</strong> {roomMatch.RoomName}</p>
                                    <p><strong>Thể loại:</strong> {roomMatch.SportCategory}</p>
                                    <p><strong>Thời gian bắt đầu:</strong> {roomMatch.StartTimeRoom}</p>
                                    <p><strong>Thời gian kết thúc:</strong> {roomMatch.EndTimeRoom}</p>
                                </div>
                                <div class='footer'>
                                    <p>© 2024 BeatSports. All rights reserved.</p>
                                </div>
                            </div>
                        </body>
                        </html>"
                );
                break;

            case "Declined":
                //roomRequest.JoinStatus = RoomRequestEnums.Declined;
                //roomRequest.DateApprove = DateTime.UtcNow;

                _beatSportsDbContext.RoomRequests.Remove(roomRequest);
                // Gửi sự kiện SignalR
                await _hubContext.Clients.Group(roomRequest.RoomMatchId.ToString()).SendAsync("UpdateRoom", "RequestDeclined", roomRequest.CustomerId);

                break;
        }
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        return new BeatSportsResponse
        {
            Message = roomRequest.JoinStatus == RoomRequestEnums.Accepted ? "Room request approved successfully." : "Room request declined."
        };
    }
}