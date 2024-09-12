using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace BeatSportsAPI.Application.Features.Transactions.Queries.GetAllTransactions;
public class GetAllTransactionsHandler : IRequestHandler<GetAllTransactionsCommand, PaginatedTransactionResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IMapper _mapper;

    public GetAllTransactionsHandler(IBeatSportsDbContext beatSportsDbContext, IMapper mapper)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedTransactionResponse> Handle(GetAllTransactionsCommand request, CancellationToken cancellationToken)
    {
        if (request.KeyWord == null)
        {
            request.KeyWord = "";
        }

        if (request.Filter == null)
        {
            request.Filter = "";
        }

        // Bước 1: Lấy tất cả các giao dịch mà không bị xóa
        var query = _beatSportsDbContext.Transactions
            .ToList()
            .OrderByDescending(b => b.Created);

        // Bước 2: Lấy toàn bộ dữ liệu người dùng để lọc về phía client
        var allWallets = _beatSportsDbContext.Wallets
            .Include(x => x.Account)
            .ToList();  // Tải toàn bộ dữ liệu vào bộ nhớ

        var filteredTransactions = query.ToList();  // Tải toàn bộ giao dịch vào bộ nhớ

        // Bước 3: Lọc dữ liệu giao dịch dựa trên filter
        if (request.Filter.Equals("RoomMatchId"))
        {
            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                if (Guid.TryParse(request.KeyWord, out var roomMatchId))
                {
                    filteredTransactions = filteredTransactions
                        .Where(t => t.RoomMatchId == roomMatchId)
                        .ToList();
                }
            }
        }
        if (request.Filter.Equals("TransactionType"))
        {
            filteredTransactions = filteredTransactions
                .Where(t => t.TransactionType.ToLower().Contains(request.KeyWord.ToLower()))
                .ToList();
        }
        else if (request.Filter.Equals("From"))
        {
            var fromUserIds = allWallets
                .Where(x => RemoveDiacritics(x.Account.FirstName + " " + x.Account.LastName)
                    .ToLower().Contains(RemoveDiacritics(request.KeyWord).ToLower()))
                .Select(x => x.Id)
                .ToList();

            filteredTransactions = filteredTransactions
                .Where(t => fromUserIds.Contains(t.WalletId))
                .ToList();
        }
        else if (request.Filter.Equals("To"))
        {
            var toUserIds = allWallets
                .Where(x => RemoveDiacritics(x.Account.FirstName + " " + x.Account.LastName)
                    .ToLower().Contains(RemoveDiacritics(request.KeyWord).ToLower()))
                .Select(x => x.Id)
                .ToList();

            filteredTransactions = filteredTransactions
                .Where(t => toUserIds.Contains(t.WalletTargetId))
                .ToList();
        }

        if (request.StartTime.HasValue && request.EndTime.HasValue)
        {
            filteredTransactions = filteredTransactions
                .Where(t => t.TransactionDate >= request.StartTime.Value && t.TransactionDate <= request.EndTime.Value)
                .ToList();
        }

        // Bước 4: Tính toán phân trang
        var totalCount = filteredTransactions.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

        var paginatedTransactions = filteredTransactions
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Bước 5: Tạo response phân trang
        var result = new PaginatedTransactionResponse
        {
            PageNumber = request.PageIndex,
            TotalPages = totalPages,
            TotalCount = totalCount,
            Items = new List<TransactionResponseV2>()
        };

        foreach (var transaction in paginatedTransactions)
        {
            var fromUser = allWallets
                .FirstOrDefault(x => x.Id == transaction.WalletId);

            var toUser = allWallets
                .FirstOrDefault(x => x.Id == transaction.WalletTargetId);

            var toUserResponse = new UserInfo();
            var fromUserResponse = new UserInfo();
            if (fromUser != null && transaction.TransactionType != "RefundRoomMaster" && transaction.TransactionType != "RefundRoomMember")
            {
                fromUserResponse.Name = fromUser.Account.FirstName + " " + fromUser.Account.LastName;
                fromUserResponse.WalletId = transaction.WalletId;
                fromUserResponse.Role = fromUser.Account.Role;
            }
            if (transaction.TransactionType == "RefundRoomMaster" || transaction.TransactionType == "RefundRoomMember")
            {
                if (fromUser != null)
                {
                    // For refund transactions, set 'to' user details based on 'fromUser'
                    toUserResponse.Name = fromUser.Account.FirstName + " " + fromUser.Account.LastName;
                    toUserResponse.WalletId = fromUser.Id;
                    toUserResponse.Role = fromUser.Account.Role;
                }
            }
            if (toUser != null)
            {
                toUserResponse.Name = toUser.Account.FirstName + " " + toUser.Account.LastName;
                toUserResponse.WalletId = transaction.WalletTargetId;
                toUserResponse.Role = toUser.Account.Role;
            }

            result.Items.Add(new TransactionResponseV2
            {
                TransactionId = transaction.Id,
                From = fromUserResponse,
                To = toUserResponse,
                TransactionAmount = transaction.TransactionAmount,
                TransactionStatus = transaction.TransactionStatus,
                AdminCheckStatus = transaction.AdminCheckStatus.ToString(),
                TransactionDate = transaction.TransactionDate,
                TransactionType = transaction.TransactionType,
                RoomMatchId = transaction.RoomMatchId,
            });
        }

        return result;
    }


    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}