using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetById;
public class GetCourtByIdHandler : IRequestHandler<GetCourtByIdCommand, CourtResponseV3>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetCourtByIdHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<CourtResponseV3> Handle(GetCourtByIdCommand request, CancellationToken cancellationToken)
    {
       

        var query = _dbContext.Courts
            .Where(x => !x.IsDelete && x.Id == request.CourtId)
            .OrderByDescending(b => b.Created)
            .Include(x => x.Owner).ThenInclude(x => x.Account)
            .Include(x => x.CourtSubdivision)
            .FirstOrDefault();

        var listCourtSub = query.CourtSubdivision.Select(b => new CourtSubdivisionResponse
        {
            Id = b.Id,
            CourtId = b.CourtId,
            CourtSubdivisionName = b.CourtSubdivisionName,
            BasePrice = b.BasePrice,
            IsActive = b.IsActive,
        }).ToList();

        var court = new CourtResponseV3
        {
            Id = query.Id,
            OwnerName = query.Owner.Account.FirstName + " " + query.Owner.Account.LastName,
            CourtName = query.CourtName,
            Description = query.Description,
            Address = query.Address,
            WallpaperUrls = "https://res.cloudinary.com/dcbkmwm3v/image/upload/v1721128187/ygodonohp6lac35vjydl.png?fbclid=IwZXh0bgNhZW0CMTEAAR3Ve_wvlx0OYcbc-8MZtCNCcyqyzNa0IUsgtWOHqYExNIvN4XCnxTLcWCQ_aem_z5vVNkzROC3aMrbL8s2-Mg", // Lấy ảnh đầu tiên
            CoverImgUrls = ImageUrlSplitter.SplitAndGetFirstImageUrls(query.ImageUrls), // Chuỗi gốc cho ảnh bìa
            CourtImgsList = ImageUrlSplitter.SplitImageUrls(query.ImageUrls), // Danh sách tất cả ảnh
            GoogleMapURLs = query.GoogleMapURLs,
            TimeStart = query.TimeStart,
            TimeEnd = query.TimeEnd,
            CourtSubdivision = listCourtSub,
        };

        return Task.FromResult(court);
    }
}
