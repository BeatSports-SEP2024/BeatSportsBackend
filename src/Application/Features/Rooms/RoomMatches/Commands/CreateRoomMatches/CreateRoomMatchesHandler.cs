using System.Globalization;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Application.Features.Hubs;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Commands.CreateRoomMatches;
public class CreateRoomMatchesHandler : IRequestHandler<CreateRoomMatchesCommand, RoomMatchResponse>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IHubContext<RoomRequestHub> _hubContext;
    private readonly IEmailService _emailService;

    public CreateRoomMatchesHandler(IBeatSportsDbContext dbContext, IHubContext<RoomRequestHub> hubContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _hubContext = hubContext;
        _emailService = emailService;
    }

    public async Task<RoomMatchResponse> Handle(CreateRoomMatchesCommand request, CancellationToken cancellationToken)
    {
        //check Level
        var level = await _dbContext.Levels.Where(x => x.Id == request.LevelId).SingleOrDefaultAsync();
        if (level == null || level.IsDelete)
        {
            throw new BadRequestException($"Level with Level ID:{request.LevelId} does not exist or have been deleted");
        }

        //check booking
        var booking = await _dbContext.Bookings
            .Include(b => b.CourtSubdivision)
            .ThenInclude(cs => cs.CourtSubdivisionSettings)
            .ThenInclude(css => css.SportCategories)
            .Where(b => b.Id == request.BookingId && !b.IsDelete && b.BookingStatus == BookingEnums.Approved.ToString() && b.IsRoomBooking == false)
            .FirstOrDefaultAsync();

        var isRoomUseBookingId = await _dbContext.RoomMatches
            .Where(rm => rm.BookingId == request.BookingId).FirstOrDefaultAsync();

        if (booking == null || isRoomUseBookingId != null)
        {
            throw new BadRequestException("This booking may be deleted or this booking is used");
        }

        if (booking.CourtSubdivision == null || booking.CourtSubdivision.CourtSubdivisionSettings == null ||
            booking.CourtSubdivision.CourtSubdivisionSettings.SportCategories == null)
        {
            throw new ArgumentException("Invalid booking details, please check CourtSubdivision or CourtSubdivisionSettings or SportCategories");
        }

        string sportCategoryName = booking.CourtSubdivision.CourtSubdivisionSettings.SportCategories.Name.ToString();
        SportCategoriesEnums sportCategoryEnum;

        if (!SportCategoryMapper.SportCategoryMapping.TryGetValue(sportCategoryName, out sportCategoryEnum))
        {
            throw new ArgumentException($"Invalid sport category: {sportCategoryName}");
        }

        //string dateFormat = "dd/MM/yyyy HH:mm";

        //DateTime startTimeRoom;
        //bool isValidDate = DateTime.TryParseExact(
        //    request.StartTimeRoom, 
        //    dateFormat,          
        //    CultureInfo.InvariantCulture, 
        //    DateTimeStyles.None,   
        //    out startTimeRoom      
        //);

        //if (!isValidDate)
        //{
        //    throw new ArgumentException("Invalid date format for StartTimeRoom. Please use 'day/month/year hours:minutes' format.");
        //}

        var ratingRoomExist = await _dbContext.RatingRooms.Where(r => r.Id == request.RatingRoomId).SingleOrDefaultAsync();

        // chia tiền
        var teamSize = (decimal)request.MaximumMember / 2;
        var teamCost = (booking.TotalAmount * (decimal)(ratingRoomExist?.WinRatePercent ?? 0));

        var room = new RoomMatch()
        {
            IsPrivate = request.IsPrivate,
            SportCategory = sportCategoryEnum,
            RoomName = request.RoomName,
            BookingId = request.BookingId,
            LevelId = request.LevelId,
            RatingRoomId = request.RatingRoomId,
            RoomMatchTypeName = request.RoomMatchTypeName,
            StartTimeRoom = booking.PlayingDate.Add(booking.StartTimePlaying),
            EndTimeRoom = booking.PlayingDate.Add(booking.EndTimePlaying),
            MaximumMember = request.MaximumMember,
            TotalCostEachMember = (double)(teamCost / teamSize),
            RuleRoom = request.RuleRoom,
            Note = request.Note
        };

        _dbContext.RoomMatches.Add(room);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var roomMember = new RoomMember()
        {
            CustomerId = booking.CustomerId,
            RoomMatchId = room.Id,
            RoleInRoom = RoleInRoomEnums.Master,
            Team = "A", // chủ phòng auto team A, nếu swap thì mới đổi team khác
            MatchingResultStatus = "NoResult" // 1. Tạo phòng chưa có kết quả (NoResult)
        };

        _dbContext.RoomMembers.Add(roomMember);
        await _dbContext.SaveChangesAsync(cancellationToken);

        booking.IsRoomBooking = true;

        _dbContext.Bookings.Update(booking);
        await _dbContext.SaveChangesAsync(cancellationToken);
        // Gửi email thông báo tạo phòng thành công
        var customer = await _dbContext.Customers
            .Include(c => c.Account)
            .Where(c => c.Id == booking.CustomerId && !c.IsDelete)
            .FirstOrDefaultAsync();

        if (customer != null && customer.Account != null)
        {
            var toEmail = customer.Account.Email;
            var subject = "Xác nhận tạo phòng chơi thành công";
            var body = $@"
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
                            Xác nhận tạo phòng chơi thành công
                        </div>
                        <div class='content'>
                            <p>Kính gửi {customer.Account.FirstName} {customer.Account.LastName},</p>
                            <p>Thông báo rằng phòng chơi của bạn đã được tạo thành công với các thông tin sau:</p>
                            <p><strong>Tên phòng:</strong> {room.RoomName}</p>
                            <p><strong>Thể loại thể thao:</strong> {sportCategoryName}</p>
                            <p><strong>Thời gian bắt đầu:</strong> {room.StartTimeRoom:dd/MM/yyyy HH:mm}</p>
                            <p><strong>Thời gian kết thúc:</strong> {room.EndTimeRoom:dd/MM/yyyy HH:mm}</p>
                            <p><strong>Quy tắc phòng:</strong> {room.RuleRoom}</p>
                            <p><strong>Ghi chú:</strong> {room.Note}</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 BeatSports. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(toEmail, subject, body);
        }
        await _hubContext.Clients.All.SendAsync("UpdateRoomList"/*, "NewRoomCreated", room.Id.ToString()*/);

        return new RoomMatchResponse
        {
            Message = $"Create RoomMatch successfully",
            RoomMatchId = room.Id.ToString(),
        };
    }
}