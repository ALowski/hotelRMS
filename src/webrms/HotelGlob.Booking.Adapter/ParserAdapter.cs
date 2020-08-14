using HotelGlob.Booking.Domain;
using HotelGlob.Booking.Domain.Data;
using HotelGlob.Booking.Parser;
using HotelGlob.RMS.Data.Models;
using Hotels.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelGlob.Booking.Adapter
{
    public class ParserAdapter:IParserAdapter
    {
        public IEnumerable<Parser_RoomData> Run(DateTime startDate, int planningHorizon,
             IEnumerable<Parser_RoomInfo> rooms, string settings, string bookingHotelName, out double rating)
        {
            rating = 0.0;
            var configuration = new DefoConfiguration(settings);
            List<Parser_RoomData> results = new List<Parser_RoomData>();
            foreach (var items in rooms.GroupBy(c => new { c.City, c.PeopleNum }).ToList())
            {
                IBookingService service = new BookingService();
                var result = service.Run(startDate, startDate.AddDays(planningHorizon - 1), items.Select(c => new SearchRoomInfo { Cancelation = c.Cancelation, City = c.City, HotelName = c.HotelName, MealName = c.MealName, PeopleNum = c.PeopleNum, Prepayment = c.Prepayment, RoomName = c.RoomName, RoomInfoId = c.Id }), configuration.ConfigurationRoot.Categories.StayPeriods?.Select(c=>(int)(c.UpperBound+(c.LowerBound<2?1: c.LowerBound))/2), bookingHotelName, out rating);
                results.AddRange(result.Select(c => new Parser_RoomData { Parser_RoomInfoId = c.RoomInfoId, Price = c.Price, Rating=c.Rating, Date = c.Date, CreationDate = startDate, AverageOccupancy=c.AverageOccupancy, StayLength = c.StayLength }));
            }
            return results;
        }
    }
}
