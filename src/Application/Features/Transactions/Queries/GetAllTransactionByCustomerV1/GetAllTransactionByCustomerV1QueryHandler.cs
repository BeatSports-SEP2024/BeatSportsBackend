using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactionByCustomerV1;
public class GetAllTransactionByCustomerV1QueryHandler : IRequestHandler<GetAllTransactionByCustomerV1Query, List<TransactionResponse>>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;

    public GetAllTransactionByCustomerV1QueryHandler(IBeatSportsDbContext beatSportsDbContext)
    {
        _beatSportsDbContext = beatSportsDbContext;
    }

    public async Task<List<TransactionResponse>> Handle(GetAllTransactionByCustomerV1Query request, CancellationToken cancellationToken)
    {
        var owner = _beatSportsDbContext.Customers.Where(x => x.Id == request.CustomerId).FirstOrDefault();
        var wallet = _beatSportsDbContext.Wallets.Where(x => x.AccountId == owner.AccountId).FirstOrDefault();
        var listTransaction = await _beatSportsDbContext.Transactions.Where(x =>
                                                                      // Trường hợp khi ví của customer là điểm đi
                                                                      // Tức là nó "Nạp tiền"
                                                                      (x.WalletId == wallet.Id &&
                                                                      x.TransactionType.Trim() == "Nạp tiền")
                                                                      ||
                                                                      (x.WalletId == wallet.Id &&
                                                                      x.TransactionType.Trim() == "JoinRoom" || x.TransactionType.Trim() == "OutRoom" || x.TransactionType.Trim() == "RefundRoomMember")
                                                                      ||
                                                                      // Trường hợp trừ tiền là khi nó chuyển tiền
                                                                      (x.WalletId == wallet.Id &&
                                                                      x.TransactionType.Trim() == "Giao dịch trong App" &&
                                                                      // Lấy cả 2 trường hợp là đã vô khung kh thể hủy và
                                                                      // chưa vô khung
                                                                      (x.AdminCheckStatus == AdminCheckEnums.Accepted || x.AdminCheckStatus == AdminCheckEnums.Pending)))
            .OrderByDescending(x => x.LastModified).ThenByDescending(x => x.Created).ToListAsync();
        var response = listTransaction.Select(t => new TransactionResponse
        {
            TransactionId = t.Id,
            WalletId = t.WalletId,
            WalletTargetId = t.WalletTargetId,
            TransactionMessage = t.TransactionMessage,
            TransactionPayload = t.TransactionPayload,
            TransactionStatus = t.TransactionStatus.ToString(),
            AdminCheckStatus = t.AdminCheckStatus.ToString(),
            TransactionAmount = t.TransactionAmount,
            TransactionDate = t.TransactionDate,
            TransactionType = t.TransactionType
        }).ToList();

        return response;
    }
}
