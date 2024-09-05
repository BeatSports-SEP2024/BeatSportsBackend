﻿using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Application.Features.Hubs;
using BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.UpdateRoomRequests;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using CloudinaryDotNet;
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

        var customer = _beatSportsDbContext.Customers
                        .Where(x => x.Id == request.CustomerId).FirstOrDefault();

        var email = roomRequest.Customer?.Account?.Email;
        if (email == null)
        {
            // Log lỗi hoặc xử lý trường hợp email bị null
            throw new BadRequestException("Email address is missing");
        }

        switch (request.RoomRequest.ToString())
        {
            case "Accepted":
                // Chủ phòng chấp nhận yêu cầu
                roomRequest.JoinStatus = RoomRequestEnums.Accepted;
                roomRequest.DateApprove = DateTime.UtcNow;

                _beatSportsDbContext.RoomRequests.Update(roomRequest);

                //Kiểm tra roomatch tối đa bao nhiêu thành viên trong 1 đội
                var teamMemberCount = roomMatch.MaximumMember / 2;
                // kiểm tra xem dựa theo số teamMemberCount thì team A được bao nhiêu rồi
                // nếu đầy rồi thì appove thì th kia xuống teamB, còn chưa đầy thì cho dô A
                var roomMemberTeamA = _beatSportsDbContext.RoomMembers
                    .Where(rm => rm.RoomMatchId == request.RoomRequestId && rm.Team == "A")
                    .ToList()
                    .Count();
                var roomMemberTeamB = _beatSportsDbContext.RoomMembers
                    .Where(rm => rm.RoomMatchId == request.RoomRequestId && rm.Team == "B")
                    .ToList()
                    .Count();

                // Khi được chấp nhận, thì RoomMatch có thêm thành viên
                if (roomMemberTeamA < teamMemberCount)
                {
                    var roomMember = new RoomMember
                    {
                        CustomerId = roomRequest.CustomerId,
                        RoomMatchId = roomRequest.RoomMatchId,
                        RoleInRoom = RoleInRoomEnums.Member,
                        Team = "A"
                    };
                    _beatSportsDbContext.RoomMembers.Add(roomMember);
                }
                else if (roomMemberTeamB < teamMemberCount)
                {
                    var roomMember = new RoomMember
                    {
                        CustomerId = roomRequest.CustomerId,
                        RoomMatchId = roomRequest.RoomMatchId,
                        RoleInRoom = RoleInRoomEnums.Member,
                        Team = "B"
                    };
                    _beatSportsDbContext.RoomMembers.Add(roomMember);
                }

                var notification = new Notification
                {
                    AccountId = customer.AccountId,
                    Title = "Yêu cầu tham gia phòng",
                    Message = $" đã được chấp thuận!",
                    RoomMatchId = roomRequest.RoomMatchId.ToString(),
                    IsRead = false,
                    Type = "RoomRequestAccepted"
                };
                _beatSportsDbContext.Notifications.Add(notification);

                //// Gửi sự kiện SignalR
                //await _hubContext.Clients.Group(roomRequest.RoomMatchId.ToString()).SendAsync("UpdateRoom", "RequestAccepted", roomRequest.CustomerId);
                // Gửi email cho khách hàng
                await _emailService.SendEmailAsync(
                    email,
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
                                    <p><strong>Thể loại:</strong> {roomMatch.SportCategory.GetDescriptionFromEnum()}</p>
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

                await _hubContext.Clients.Group(roomRequest.RoomMatchId.ToString()).SendAsync("UpdateRoom", "RequestAccepted", roomRequest.CustomerId);
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

        //if (request.RoomRequest.ToString() == "Accepted")
        //{
        //    await _hubContext.Clients.Group(roomRequest.RoomMatchId.ToString()).SendAsync("UpdateRoom", "RequestAccepted", roomRequest.CustomerId);
        //}
        //else if (request.RoomRequest.ToString() == "Declined")
        //{
        //    await _hubContext.Clients.Group(roomRequest.RoomMatchId.ToString()).SendAsync("UpdateRoom", "RequestDeclined", roomRequest.CustomerId);
        //}

        var roomMatchJoinedList = _beatSportsDbContext.RoomRequests
                        .Where(x => x.CustomerId == request.CustomerId && x.JoinStatus == RoomRequestEnums.Pending)
                        .ToList();
        if (roomMatchJoinedList.Count > 0)
        {
            foreach (var roomReq in roomMatchJoinedList)
            {
                _beatSportsDbContext.RoomRequests.Remove(roomReq);
            }
        }
        await _beatSportsDbContext.SaveChangesAsync(cancellationToken);

        //var notification = new Notification
        //{
        //    AccountId = customer.AccountId,
        //    Title = "Yêu cầu tham gia phòng",
        //    Message = $" đã được chấp thuận!",
        //    RoomMatchId = roomRequest.RoomMatchId.ToString(),
        //    IsRead = false,
        //    Type = "RoomRequestAccepted"
        //};
        //_beatSportsDbContext.Notifications.Add(notification);

        //await _beatSportsDbContext.SaveChangesAsync();

        return new BeatSportsResponse
        {
            Message = roomRequest.JoinStatus == RoomRequestEnums.Accepted ? "Room request approved successfully." : "Room request declined."
        };
    }
}