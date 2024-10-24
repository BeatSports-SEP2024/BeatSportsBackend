﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSportsAPI.Application.Common.Models;
using BeatSportsAPI.Application.Common.Response;
using BeatSportsAPI.Domain.Enums;
using MediatR;

namespace BeatSportsAPI.Application.Features.Rooms.RoomMatches.Queries.GetAllRoomMatches;
public class GetAllRoomMatchesCommand : IRequest<RoomRequestsResponseForGetAll>
{
    //[Required]
    //public int PageIndex { get; set; }
    //[Required]
    //public int PageSize { get; set; }
    public Guid CustomerId { get; set; }
    public string? Level { get; set; }
    //Query để search cho tên chủ phòng và địa chỉ
    public string? Query { get; set; }
    [EnumDataType(typeof(SportCategoriesEnums))]
    public SportCategoriesEnums sportFilter { get; set; }
}