﻿namespace BeatSportsAPI.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? IpAddress { get; }
}
