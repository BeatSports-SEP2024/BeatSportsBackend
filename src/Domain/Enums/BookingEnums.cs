using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSportsAPI.Domain.Enums;
public enum BookingEnums
{
    //Booking successfully, not played
    Approved,
    //Cancel booking
    Cancel,
    //Booking successfully and already played
    Finished
}
