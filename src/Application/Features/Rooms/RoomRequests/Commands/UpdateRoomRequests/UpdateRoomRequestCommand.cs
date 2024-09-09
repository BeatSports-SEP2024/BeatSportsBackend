using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Hubs;
using BeatSportsAPI.Domain.Entities.Room;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Rooms.RoomRequests.Commands.UpdateRoomRequests;
public class UpdateRoomRequestCommand : IRequest<BeatSportsResponse>
{
    public Guid CustomerId { get; set; }
    public Guid RoomMatchId { get; set; }
}

public class UpdateRoomRequestCommandHandler : IRequestHandler<UpdateRoomRequestCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IHubContext<RoomRequestHub> _hubContext;

    public UpdateRoomRequestCommandHandler(IBeatSportsDbContext beatSportsDbContext, IHubContext<RoomRequestHub> hubContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _hubContext = hubContext;
    }
    public async Task<BeatSportsResponse> Handle(UpdateRoomRequestCommand request, CancellationToken cancellationToken)
    {
        var roomMatch = _beatSportsDbContext.RoomMatches
             .Where(rm => rm.Id == request.RoomMatchId && !rm.IsDelete)
             .FirstOrDefault();
        var customer = _beatSportsDbContext.Customers
            .Where(c => c.Id == request.CustomerId && !c.IsDelete)
            .FirstOrDefault();

        if (roomMatch == null)
        {
            throw new NotFoundException($"{request.RoomMatchId} is not existed");
        }

        if (customer == null)
        {
            throw new NotFoundException($"{request.CustomerId} is not existed");
        }
        var checkMasterId = await _beatSportsDbContext.RoomMembers
            .Where(rm => rm.RoomMatchId == request.RoomMatchId && rm.CustomerId == request.CustomerId && rm.RoleInRoom == RoleInRoomEnums.Master)
            .FirstOrDefaultAsync();
        if (checkMasterId == null)
        {
            if(roomMatch.StartTimeRoom <= DateTime.Now)
            {
                throw new BadRequestException($"Bạn không thể rời phòng. Đã quá thời gian tối thiểu cho bạn rời phòng.");
            }
            var dataRoomMember = await _beatSportsDbContext.RoomMembers
                .Where(rm => rm.RoomMatchId == roomMatch.Id
                            && rm.CustomerId == request.CustomerId)
                .SingleOrDefaultAsync();
            var dataRoomRequest = await _beatSportsDbContext.RoomRequests
                .Where(rm => rm.RoomMatchId == roomMatch.Id
                            && rm.CustomerId == request.CustomerId
                            && rm.JoinStatus == RoomRequestEnums.Accepted)
                .SingleOrDefaultAsync();
            if (dataRoomMember != null && dataRoomRequest != null)
            {
                // update lại transaction rồi back tiền lại cho các thành viên khác trong room
                // chưa có kiểm tra thời gian tối thiểu cho phép thành viên đó thoát ra nha => đang là lúc nào cũng thoát được
                // chỗ này t đang suy nghĩ là cho cái transactionStatus đang pending thì thoát được, nếu chuyển qua approved rồi thì ko thoát được nữa
                var walletCusExist = await _beatSportsDbContext.Wallets.Where(w => w.AccountId == customer.AccountId).SingleOrDefaultAsync();
                if (walletCusExist == null)
                {
                    throw new NotFoundException($"Đã có lỗi xảy ra, không tìm thấy ví của khách hàng.");
                }
                var transactionJoinExist = await _beatSportsDbContext.Transactions
                                .Where(t => t.RoomMatchId == request.RoomMatchId
                                            && t.WalletId == walletCusExist.Id
                                            && t.TransactionType == "JoinRoom"
                                            && t.TransactionStatus == "Pending")
                                .SingleOrDefaultAsync();
                if (transactionJoinExist == null)
                {
                    throw new BadRequestException($"Đã có lỗi xảy ra, không tìm thấy giao dịch tham gia phòng.");
                }
                // update transaction trước xong mới cộng tiền
                transactionJoinExist.TransactionStatus = "Cancel"; // hoàn trả(Cancel) thoát khỏi phòng trả tiền lại cho member thì update lại transaction
                transactionJoinExist.TransactionType = "OutRoom"; 
                transactionJoinExist.TransactionDate = DateTime.Now;
                transactionJoinExist.TransactionMessage = "Rời phòng thành công";
                transactionJoinExist.Created = DateTime.Now;
                transactionJoinExist.LastModified = DateTime.Now;
                _beatSportsDbContext.Transactions.Update(transactionJoinExist);

                // cộng tiền về ví cho member đó
                walletCusExist.Balance += (transactionJoinExist.TransactionAmount ?? throw new BadRequestException("Có lỗi xảy ra, số dư giao dịch bằng 0."));
                _beatSportsDbContext.Wallets.Update(walletCusExist);

                _beatSportsDbContext.RoomMembers.Remove(dataRoomMember);
                _beatSportsDbContext.RoomRequests.Remove(dataRoomRequest);

                await _beatSportsDbContext.SaveChangesAsync();

                // thành viên out room
                await _hubContext.Clients.Group(roomMatch.Id.ToString()).SendAsync("UpdateRoom", "MemberLeft", customer.Id);
            }
        }
        else
        {
            var ListRoomMember = await _beatSportsDbContext.RoomMembers.Where(rm => rm.RoomMatchId == roomMatch.Id).ToListAsync();
            var ListRoomRequest = await _beatSportsDbContext.RoomRequests.Where(rm => rm.RoomMatchId == roomMatch.Id).ToListAsync();

            // sẽ dựa theo các thành viên trong bảng roomMember Update lại transaction cho các thành viên khi chủ phòng xóa phòng
            foreach (var member in ListRoomMember)
            {
                var customerExist = _beatSportsDbContext.Customers
                                .Where(c => c.Id == member.CustomerId && !c.IsDelete)
                                .FirstOrDefault();
                if (customerExist == null)
                {
                    throw new NotFoundException($"Customer is not existed");
                }

                var walletCusExist = await _beatSportsDbContext.Wallets.Where(w => w.AccountId == customerExist.AccountId).SingleOrDefaultAsync();
                if (walletCusExist == null)
                {
                    throw new NotFoundException($"Đã có lỗi xảy ra, không tìm thấy ví của khách hàng.");
                }

                // Tìm transaction cho mỗi thành viên (nếu họ đã tham gia phòng)
                // chủ phòng đang cho nó muốn out cỡ nào cũng được nên ko cần quan tâm cái này
                // && t.TransactionStatus == "Approved" hay "Pending", chỉ quan tâm loại là đang joinRoom thôi
                var joinTransaction = await _beatSportsDbContext.Transactions
                    .Where(t => t.RoomMatchId == roomMatch.Id
                                               && t.WalletId == walletCusExist.Id
                                               && t.TransactionType == "JoinRoom")
                    .SingleOrDefaultAsync();
                if (joinTransaction != null)
                {
                    // update transaction trước xong mới cộng tiền
                    joinTransaction.TransactionStatus = "Cancel"; // hoàn trả(Cancel) thoát khỏi phòng trả tiền lại cho member thì update lại transaction
                    joinTransaction.TransactionType = "OutRoom"; // chủ phòng thoát là OutRoom
                    joinTransaction.TransactionDate = DateTime.Now;
                    joinTransaction.TransactionMessage = "Rời phòng thành công";
                    joinTransaction.Created = DateTime.Now;
                    joinTransaction.LastModified = DateTime.Now;
                    _beatSportsDbContext.Transactions.Update(joinTransaction);

                    // cộng tiền về ví cho member đó
                    walletCusExist.Balance += (joinTransaction.TransactionAmount ?? throw new BadRequestException("Có lỗi xảy ra, số dư giao dịch bằng 0."));
                    _beatSportsDbContext.Wallets.Update(walletCusExist);
                }
            }

            _beatSportsDbContext.RoomMembers.RemoveRange(ListRoomMember);
            _beatSportsDbContext.RoomRequests.RemoveRange(ListRoomRequest);


            var bookingExist = await _beatSportsDbContext.Bookings.Where(b => b.Id == roomMatch.BookingId && b.CustomerId == request.CustomerId).SingleOrDefaultAsync();
            if (bookingExist != null)
            {
                bookingExist.IsRoomBooking = false;
                _beatSportsDbContext.Bookings.Update(bookingExist);
            }

            _beatSportsDbContext.RoomMatches.Remove(roomMatch);
            await _beatSportsDbContext.SaveChangesAsync();

            // chủ phòng outrooom
            await _hubContext.Clients.Group(roomMatch.Id.ToString()).SendAsync("UpdateRoom", "RoomClosed", customer.Id);
        }

        return new BeatSportsResponse
        {
            Message = "Logout room match successfully!"
        };
    }
}
