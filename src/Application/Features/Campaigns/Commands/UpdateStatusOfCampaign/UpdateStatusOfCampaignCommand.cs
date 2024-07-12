﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Campaigns.Commands.UpdateStatusOfCampaign;
public class UpdateStatusOfCampaignCommand : IRequest<BeatSportsResponse>
{
    [Required]
    public Guid CampaignId { get; set; }
    public StatusEnums Status { get; set; }
    public string ReasonOfReject { get; set; }
}
