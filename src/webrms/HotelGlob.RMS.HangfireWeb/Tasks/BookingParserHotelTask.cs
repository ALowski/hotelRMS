using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using HotelGlob.RMS.Adapter;
using HotelGlob.RMS.Data;
using HotelGlob.RMS.Data.Models;
using HotelGlob.Booking.Adapter;

namespace HotelGlob.RMS.HangfireWeb.Tasks
{
    public class BookingParserHotelTask
    {
        public static string GetJobName(int hotelId) => string.Concat("BookingParserHotelTask.Run_", hotelId); 

        [AutomaticRetry(Attempts = 10)]
        public void Run(int hotelId)
        {
            using (var context = new RmsDbContext())
            {
                HotelSettings hotel = context.HotelSettings.Find(hotelId);

                if (hotel == null || !hotel.IsRmsEnalbed)
                {
                    RecurringJob.RemoveIfExists(GetJobName(hotelId));
                    return;
                }

                if (string.IsNullOrWhiteSpace(hotel.Settings))
                {
                    context.Logs.Add(new Log { LogType = LogType.Error, CreatedOn = DateTime.Now, Body = "Configuration not found.", HotelId = hotelId });
                    context.SaveChanges();
                    return;
                }
                var info = context.Parser_RoomInfos.Where(c => c.HotelId == hotelId).ToList();
                if (!info.Any())
                {
                    context.Logs.Add(new Log{ LogType = LogType.Error, CreatedOn = DateTime.Now, Body = "Parser hotel configuration not found.", HotelId = hotelId});
                    context.SaveChanges();
                    return;
                }
               
                try
                {
                    IParserAdapter parserAdapter = new ParserAdapter();

                    var calculatedOn = DateTime.Now;
                    double rating = 0.0;
                    var result = parserAdapter.Run(calculatedOn, hotel.PlanningHorizon, info, hotel.Settings,hotel.BookingName, out rating).ToList();

                    if (result.Any())
                    {
                        context.Parser_RoomDatas.AddRange(result);
                        context.Logs.Add(new Log { LogType = LogType.Info, CreatedOn = DateTime.Now, Body = "New booking data have been successfully obtained.", HotelId = hotelId });
                        hotel.BookingRating = rating;
                        context.Entry(hotel).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                    }
                }
                catch(Exception ex)
                {
                    context.Logs.Add(new Log { LogType = LogType.Error, CreatedOn = DateTime.Now, Body = ex.Message+Environment.NewLine+ ex.InnerException?.Message, HotelId = hotelId });
                    context.SaveChanges();
                    throw;
                }
            }
        }
    }
}