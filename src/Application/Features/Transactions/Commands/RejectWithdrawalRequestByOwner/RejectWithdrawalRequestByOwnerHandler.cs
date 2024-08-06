using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Features.Transactions.Commands.ApproveWithdrawalRequestByOwner;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Transactions.Commands.RejectWithdrawalRequestByOwner;
public class RejectWithdrawalRequestByOwnerHandler : IRequestHandler<RejectWithdrawalRequestByOwnerCommand, BeatSportsResponseV2>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IEmailService _emailService;

    public RejectWithdrawalRequestByOwnerHandler(IBeatSportsDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public Task<BeatSportsResponseV2> Handle(RejectWithdrawalRequestByOwnerCommand request, CancellationToken cancellationToken)
    {
        var transaction = _dbContext.Transactions
                        .Where(x => x.Id == request.TransactionId && x.AdminCheckStatus == AdminCheckEnums.Pending)
                        .FirstOrDefault();

        if (transaction == null)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Transaction không tồn tại!"
            });
        }

        var owner = _dbContext.Owners
            .Include(c => c.Account)
                    .Where(x => x.Id == request.OwnerId)
                    .FirstOrDefault();

        if (owner == null)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Owner không tồn tại"
            });
        }

        var ownerWallet = _dbContext.Wallets
                            .Where(x => x.AccountId == owner.AccountId)
                            .FirstOrDefault();

        if (transaction.WalletId != ownerWallet.Id)
        {
            return Task.FromResult(new BeatSportsResponseV2
            {
                Status = 400,
                Message = "Duyệt đơn thất bại, ownerId không trùng khớp vói hóa đơn!"
            });
        }

        transaction.AdminCheckStatus = AdminCheckEnums.Cancel;
        transaction.ReasonOfRejected = request.ReasonOfRejected;
        ownerWallet.Balance += (int)transaction.TransactionAmount;

        _dbContext.Transactions.Update(transaction);
        _dbContext.Wallets.Update(ownerWallet);
        _dbContext.SaveChanges();

        _emailService.SendEmailAsync(
                owner.Account.Email,
                "Đơn rút tiền bị từ chối",
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
                            background-color: #ff0000;
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
                            Đơn rút tiền bị từ chối
                        </div>
                        <div class='content'>
                            <p>Chào {owner.Account.FirstName} {owner.Account.LastName},</p>
                            <p>Yêu cầu rút tiền của bạn đã bị từ chối với lý do sau:</p>
                            <p><strong>Lý do:</strong> {transaction.ReasonOfRejected}</p>
                            <p><strong>Số tiền:</strong> {transaction.TransactionAmount} VND</p>
                            <p>Số tiền đã được hoàn lại vào ví của bạn.</p>
                        </div>
                        <div class='footer'>
                            <p>© 2024 BeatSports. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>"
            );

        return Task.FromResult(new BeatSportsResponseV2
        {
            Status = 200,
            Message = "Từ chối đơn rút tiền của owner thành công!"
        });
    }
}
