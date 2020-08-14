using HotelGlob.RMS.Data.Models;
using System;
using System.Collections.Generic;

namespace HotelGlob.Booking.Adapter
{
    public interface IParserAdapter
    {
        IEnumerable<Parser_RoomData> Run(DateTime startDate, int planningHorizon,
            IEnumerable<Parser_RoomInfo> rooms, string settings, string bookingHotelName, out double rating);
    }
}
