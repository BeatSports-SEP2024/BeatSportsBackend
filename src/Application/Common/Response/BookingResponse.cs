﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Mappings;
using BeatSportsAPI.Application.Features.Bookings.Queries.GetBookingDetailReadyForFinishBooking;
using BeatSportsAPI.Domain.Entities;
using BeatSportsAPI.Domain.Entities.CourtEntity;

namespace BeatSportsAPI.Application.Common.Response;
public class BookingResponse : IMapFrom<Booking>
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    //public Guid RoomMatchId { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsRoomBooking { get; set; }
    public bool IsDeposit { get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string BookingStatus { get; set; }
}

public class BookingByCustomerId : IMapFrom<Booking>
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    //public Guid RoomMatchId { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    public string? CourtSubName { get; set; }
    public string? CourtName { get; set; }

    public string? SportType { get; set; }
    public string? CourtTypeSettings { get; set; }
    public List<SportSettingsMatchesTypeResponse>? ListSportSettingMatchesType { get; set; }

    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsRoomBooking { get; set; }
    public bool IsDeposit { get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string BookingStatus { get; set; }
}
public class SportSettingsMatchesTypeResponse
{
    public Guid SportSettingsMatchesTypeId { get; set; }
    public string? MatchTypeName { get; set; }
    public int? TotalMember { get; set; }
}

public class BookingDashboard : IMapFrom<Booking>
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    //public Guid RoomMatchId { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    public string? CourtSubName { get; set; }
    public string? CourtName { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsRoomBooking { get; set; }
    public bool IsDeposit { get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string BookingStatus { get; set; }
}

public class BookingDetailByCustomer 
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? CampaignId { get; set; }
    public string? FullName { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CampaignName { get; set; }
    //public decimal MaxValueDiscount { get; set; }
    //public decimal MinValueApply { get; set; }   
    public Guid CourtId { get; set; }
    public string? CourtName { get; set; }
    public string? CourtAddress { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string BookingStatus { get; set; }
    public decimal DiscountPrice { get; set; }
}

public class BookingHistoryByCustomerId
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? CampaignId { get; set; }
    public Guid CourtSubdivisionId { get; set; }
    public string? CourtSubName { get; set; }
    public string? CourtName { get; set; }
    public string? CourtAddress { get; set; }
    public string? CourtImage { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsRoomBooking { get; set; }
    public bool IsDeposit { get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string BookingStatus { get; set; }
    public Guid? FeedbackId { get; set; }
}
public class BookingHistoryDetailByCustomerId
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public string? QRUrl { get; set; }
    public string? FirstName { get; set; }
    public DateTime UnixTimestampMinCancellation { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CustomerAddress { get; set; }

    public Guid? CampaignId { get; set; }
    public string? CampaignName { get; set; }
    public decimal? PercentDiscount { get; set; }
    public decimal? MinValueApply { get; set; }
    public decimal? MaxValueDiscount { get; set; }
    public decimal BasePrice { get; set; }


    public Guid CourtSubdivisionId { get; set; }
    public string? CourtSubName { get; set; }
    public Guid CourtId { get; set; }
    public string? CourtName { get; set; }
    public string? CourtImageUrl { get; set; }
    public string? CourtAddress { get; set; }
    public string? CourtWallImage { get; set; }
    public string? CourtAvatarImage { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPriceInTimePeriod { get; set; }
    public decimal TotalPriceDiscountCampaign { get; set; }
    public List<CourtDetailInBookingDetailReadyForFinishBookingReponse>? ListCourtByTimePeriod { get; set; }

    public bool IsRoomBooking { get; set; }
    public bool IsDeposit { get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string? BookingStatus { get; set; }
    public bool IsCheckIn { get; set; }

}

public class VenueBarchartResponse
{
    public string Days { get; set; }
    public decimal TotalVenueOfDays { get; set; }
    public DateTime Date { get; set; }
}

public class GetBookingByCourtIdResponse
{
    public Guid CustomerId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CourtId { get; set; }
    public string? CustomerBookName { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CourtAddress { get; set; }
    public string? CourtName { get; set; }
    public DateTime BookingDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? StatusBooking { get; set; }
}