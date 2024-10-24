﻿using System.ComponentModel.DataAnnotations.Schema;
using BeatSportsAPI.Domain.Entities.CourtEntity;
using BeatSportsAPI.Domain.Entities.PaymentEntity;
using BeatSportsAPI.Domain.Entities.Room;

namespace BeatSportsAPI.Domain.Entities;
public class Booking : BaseAuditableEntity
{
    [ForeignKey("Customer")]
    public Guid CustomerId { get; set; }
    //[ForeignKey("RoomMatch")]
    //public Guid? RoomMatchId { get; set; }
    [ForeignKey("Campaign")]
    public Guid? CampaignId { get; set; }
    [ForeignKey("CourtSubdivision")]
    public Guid CourtSubdivisionId { get; set; }
    [ForeignKey("Transaction")]
    public Guid? TransactionId { get; set; }
    public DateTime BookingDate { get; set; }   
    public decimal TotalAmount { get; set; }
    public decimal TotalPriceInTimePeriod { get; set; }
    public decimal TotalPriceDiscountCampaign { get; set; }
    public string? PayloadDescriptionPriceOfTimePeriod { get; set; }
    public string? QRUrlForCheckIn { get; set; }
    /// <summary>
    /// kh còn đụng tới chỗ booking status => kh đụng tới việc tiền tự động chui vào ví owner
    /// Cronjob tự check để đổi BookingStatus
    /// </summary>
    public bool IsCheckIn { get; set; } = false;
    public bool IsRoomBooking { get; set; }
    /// <summary>
    /// Dùng để lưu thời gian cần thể nó hủy
    /// </summary>
    public DateTime UnixTimestampMinCancellation { get; set; }
    public bool IsDeposit {  get; set; }
    public DateTime PlayingDate { get; set; }
    public TimeSpan StartTimePlaying { get; set; }
    public TimeSpan EndTimePlaying { get; set; }
    public string BookingStatus { get; set; }
    public virtual CourtSubdivision CourtSubdivision { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
    public virtual Campaign? Campaign { get; set; }
    public virtual Feedback? Feedback { get; set; }
    public virtual RoomMatch? RoomMatch { get; set; }
    public virtual Transaction? Transaction { get; set; }
}
