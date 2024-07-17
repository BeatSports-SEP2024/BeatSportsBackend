using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Application.Common.Response;
public class FeedbackResponse
{
    public Guid FeedbackId { get; set; }
    public Guid BookingId { get; set; }
    public Guid CourtId { get; set; }
    public decimal FeedbackStar { get; set; }
    public bool FeedbackAvailable { get; set; }
    public string? FeedbackStatus { get; set; }
    public string FeedbackContent { get; set; } 
}

//For Detail Court screen
public class FeedbackResponseV2
{
    public Guid? FeedbackId { get; set; }
    //public Guid? CourtId { get; set; }
    public decimal? FeedbackStar { get; set; }
    public string? FeedbackContent { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? FullName { get; set; }
}