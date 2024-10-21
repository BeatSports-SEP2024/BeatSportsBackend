using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.CourtSubdivisions.Commands.AcceptCourtSubdivision;
public class AcceptCourtSubdivisionHandler : IRequestHandler<AcceptCourtSubdivisionCommand, BeatSportsResponse>
{
    private readonly IBeatSportsDbContext _beatSportsDbContext;
    private readonly IEmailService _emailService;

    public AcceptCourtSubdivisionHandler(IBeatSportsDbContext beatSportsDbContext, IEmailService emailService)
    {
        _beatSportsDbContext = beatSportsDbContext;
        _emailService = emailService;
    }

    public Task<BeatSportsResponse> Handle(AcceptCourtSubdivisionCommand request, CancellationToken cancellationToken)
    {
        var courtSubs = _beatSportsDbContext.CourtSubdivisions
            .Include(c => c.Court)
                .ThenInclude(o => o.Owner)
                    .ThenInclude(a => a.Account)
            .Where(cs => !cs.IsDelete && cs.CourtId == request.CourtId && cs.CreatedStatus == (CourtSubdivisionCreatedStatus)StatusEnums.Pending)
            .ToList();

        if (!courtSubs.Any())
        {
            throw new BadRequestException("Court Sub is not existed");
        }

        foreach (var courtSub in courtSubs)
        {
            if (request.Status == StatusEnums.Accepted)
            {
                courtSub.CreatedStatus = (CourtSubdivisionCreatedStatus)StatusEnums.Accepted;
                courtSub.ReasonOfRejected = "";

                var ownerEmail = courtSub.Court.Owner.Account.Email;
                var ownerName = courtSub.Court.Owner.Account.FirstName + " " + courtSub.Court.Owner.Account.LastName;
                var courtSubName = courtSub.CourtSubdivisionName;

                _emailService.SendEmailAsync(
                    ownerEmail,
                        "Đơn tạo sân nhỏ của bạn đã được chấp thuận",
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
                                    Thông tin đơn tạo sân nhỏ
                                </div>
                                <div class='content'>
                                    <p>Kính gửi {ownerName},</p>
                                    <p>Chúng tôi rất vui mừng thông báo với bạn rằng đơn tạo sân nhỏ cho sân '{courtSubName}' của bạn đã được chấp thuận.</p>
                                    <p>Cảm ơn sự kiên nhẫn của bạn.</p>
                                    <p>Trân trọng,</p>
                                </div>
                                <div class='footer'>
                                    <p>© 2024 BeatSports. All rights reserved.</p>
                                </div>
                            </div>
                        </body>
                        </html>"
                );
            }
            if (request.Status == StatusEnums.Rejected)
            {
                courtSub.CreatedStatus = (CourtSubdivisionCreatedStatus)StatusEnums.Rejected;
                courtSub.ReasonOfRejected = request.ReasonOfReject;

                var ownerEmail = courtSub.Court.Owner.Account.Email;
                var ownerName = courtSub.Court.Owner.Account.FirstName + " " + courtSub.Court.Owner.Account.LastName;
                var courtSubName = courtSub.CourtSubdivisionName;

                _emailService.SendEmailAsync(
                    ownerEmail,
                    "Đơn tạo sân nhỏ của bạn đã bị từ chối",
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
                                    Thông tin đơn tạo sân nhỏ
                                </div>
                                <div class='content'>
                                    <p>Kính gửi {ownerName},</p>
                                    <p>Chúng tôi rất tiếc phải thông báo với bạn rằng đơn tạo sân nhỏ cho sân '{courtSubName}' của bạn đã bị từ chối.</p>
                                    <p>Lý do: {request.ReasonOfReject}</p>
                                    <p>Vui lòng thực hiện các thay đổi cần thiết và gửi lại.</p>
                                    <p>Cảm ơn sự thông cảm của bạn.</p>
                                    <p>Trân trọng,</p>
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

        _beatSportsDbContext.CourtSubdivisions.UpdateRange(courtSubs);
        _beatSportsDbContext.SaveChanges();

        return Task.FromResult(new BeatSportsResponse
        {
            Message = "Court Subdivision is approved/rejected",
        });
    }
}