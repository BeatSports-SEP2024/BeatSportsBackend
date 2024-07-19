using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatSportsAPI.Application.Common.Exceptions;
using BeatSportsAPI.Application.Common.Interfaces;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Application.Common.Response.CourtResponse;
using BeatSportsAPI.Application.Common.Ultilities;
using BeatSportsAPI.Application.Features.Courts.Queries.GetAllCourtsByOwnerId;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Services.MapBox;

namespace BeatSportsAPI.Application.Features.Courts.Queries.GetListCourtsNearBy;
public class GetListCourtsNearByHandler : IRequestHandler<GetListCourtsNearByCommand, List<CourtResponseV3>>
{
    private readonly IBeatSportsDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetListCourtsNearByHandler(IBeatSportsDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    public async Task<List<CourtResponseV3>> Handle(GetListCourtsNearByCommand request, CancellationToken cancellationToken)
    {
        var distanceCal = new DistanceCalculation();

        if (request.KeyWords == null)
        {
            request.KeyWords = "";
        }

        if (request.Criteria == null)
        {
            request.Criteria = "";
        }

        var query = new List<Court>();

        if (request.CourtId != Guid.Empty)
        {
            query = _dbContext.Courts
           .Where(x => !x.IsDelete && x.Id == request.CourtId && (x.CourtName.Contains(request.KeyWords) || x.Address.Contains(request.KeyWords)))
           .Include(x => x.Owner)
           .Include(x => x.Feedback)
           .Include(x => x.CourtSubdivision)
           .ThenInclude(x => x.Bookings)
           .ToList();
        }
        else
        {
            query = _dbContext.Courts
            .Where(x => !x.IsDelete)
            .Include(x => x.Owner)
            .Include(x => x.Feedback)
            .Include(x => x.CourtSubdivision)
            .ThenInclude(x => x.Bookings)
            .ToList();

            query = query.Where(x => RemoveDiacritics(x.CourtName).ToLower().Contains(RemoveDiacritics(request.KeyWords).ToLower()) ||
                                     RemoveDiacritics(x.Address).ToLower().Contains(RemoveDiacritics(request.KeyWords).ToLower()))
                         .ToList();

            if (request.SportCategory != null)
            {
                var sportCategory = _dbContext.SportsCategories
                                .Where(x => x.Name.Contains(request.SportCategory))
                                .FirstOrDefault();

                var courtSubList = _dbContext.CourtSubdivisionSettings
                                .Where(x => x.SportCategoryId == sportCategory.Id)
                                .ToList();
                var tmp = query.ToList();

                foreach (var court in query)
                {
                    var flag = 0;
                    foreach (var courtSub in court.CourtSubdivision)
                    {
                        if (courtSubList.Any(x => x.Id == courtSub.CourtSubdivisionSettingId))
                        {
                            flag++;
                            break;
                        }
                    }

                    if (flag == 0)
                    {
                        tmp.Remove(court);
                    }
                }
                query = tmp;
            }

            if (request.Criteria.Equals("Đã thuê") && request.CustomerId != null)
            {
                var tmp = query.ToList();
                foreach (var court in query)
                {
                    var flag = 0;
                    foreach (var c in court.CourtSubdivision)
                    {
                        if (c.Bookings.Any(x => x.CustomerId == request.CustomerId))
                        {
                            flag++;
                            break;
                        }
                    }

                    if (flag == 0)
                    {
                        tmp.Remove(court);
                    }
                }
                query = tmp;
            }
        }

        var list = new List<CourtResponseV3>();

        foreach (var c in query)
        {
            double starAvg = 0;
            decimal price = 0;
            int rentalNumber = 0;
            if (c.Feedback.Count != 0)
            {
                starAvg = (double)c.Feedback.Average(x => x.FeedbackStar);
            }

            if (c.CourtSubdivision.Count != 0)
            {
                price = c.CourtSubdivision.MinBy(c => c.BasePrice).BasePrice;
                rentalNumber = c.CourtSubdivision.Sum(x => x.Bookings.Where(c => c.BookingStatus.Equals("Approved")).Count());
            }

            var listCourtSub = c.CourtSubdivision.Select(b => new CourtSubdivisionResponse
            {
                Id = b.Id,
                CourtId = b.CourtId,
                CourtSubdivisionName = b.CourtSubdivisionName,
                BasePrice = b.BasePrice,
                IsActive = b.IsActive,
            }).ToList();
            var imageUrls = c.ImageUrls?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(url => url.Trim())
                         .Where(url => !string.IsNullOrEmpty(url))
                         .ToArray();

            var name = _dbContext.Accounts
                .Where(x => x.Id == c.Owner.AccountId)
                .FirstOrDefault();
            list.Add(new CourtResponseV3
            {
                Id = c.Id,
                OwnerName = name.FirstName + " " + name.LastName,
                Description = c.Description,
                CourtName = c.CourtName,
                Address = c.Address,
                GoogleMapURLs = c.GoogleMapURLs,
                WallpaperUrls = "https://res.cloudinary.com/dcbkmwm3v/image/upload/v1721128187/ygodonohp6lac35vjydl.png?fbclid=IwZXh0bgNhZW0CMTEAAR3Ve_wvlx0OYcbc-8MZtCNCcyqyzNa0IUsgtWOHqYExNIvN4XCnxTLcWCQ_aem_z5vVNkzROC3aMrbL8s2-Mg", // Lấy ảnh đầu tiên
                CoverImgUrls = ImageUrlSplitter.SplitAndGetFirstImageUrls(c.ImageUrls), // Chuỗi gốc cho ảnh bìa
                CourtImgsList = ImageUrlSplitter.SplitImageUrls(c.ImageUrls), // Danh sách tất cả ảnh
                TimeStart = c.TimeStart,
                TimeEnd = c.TimeEnd,
                PlaceId = c.PlaceId,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                LatitudeDelta = c.LatitudeDelta,
                LongitudeDelta = c.LongitudeDelta,
                FbStar = starAvg,
                CourtSubCount = c.CourtSubdivision.Count,
                Price = price,
                RentalNumber = rentalNumber,
                CourtSubdivision = listCourtSub,
            });
        }

        if (request.Latitude != 0 && request.Longitude != 0 && list.Count > 0)
        {
            //Goi service tinh khoang cach
            var result = await distanceCal.GetDistancesAsync(request.Latitude, request.Longitude, query);

            int index = 0;
            result.ForEach(c =>
            {
                list.ElementAt(index).DistanceInKm = c.DistanceInKm;
                index++;
            });

            list = list.OrderBy(c => c.DistanceInKm).ToList();

        }

        if (request.Criteria.Equals("Nổi bật"))
        {
            list = list.OrderByDescending(c => c.FbStar).ToList();
        }
        else if (request.Criteria.Equals("Thuê nhiều"))
        {
            list = list.OrderByDescending(c => c.RentalNumber).ToList();
        }

        return list;
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
