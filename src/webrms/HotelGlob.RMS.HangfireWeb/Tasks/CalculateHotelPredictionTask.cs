using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using HotelGlob.RMS.Adapter;
using HotelGlob.RMS.Data;
using HotelGlob.RMS.Data.Models;
using System.Configuration;

namespace HotelGlob.RMS.HangfireWeb.Tasks
{
    public class CalculateHotelPredictionTask
    {
        public static string GetJobName(int hotelId) => string.Concat("CalculateHotelPredictionTask.Run_", hotelId); 

        [AutomaticRetry(Attempts = 0)]
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
                    context.Logs.Add(new Log{ LogType = LogType.Error, CreatedOn = DateTime.Now, Body = "Configuration not found.", HotelId = hotelId});
                    context.SaveChanges();
                    return;
                }

                //Calculation lastCalculation = context.Calculations.Where(c => c.HotelId == hotelId).OrderByDescending(c => c.CalculatedOn).FirstOrDefault();

                //if ((lastCalculation == null && context.Reservations.Any(r => r.HotelId == hotelId)) ||
                //    (lastCalculation != null && context.Reservations.Any(r => r.HotelId == hotelId && r.CreatedOn > lastCalculation.CalculatedOn)))
                if(hotel.IsNeedRecalc)
                {
                    IEnumerable<Reservation> reservations = context.Reservations.Where(r => r.HotelId == hotelId).ToList();
                    IEnumerable<Event> events = context.Events.Where(r => r.HotelId == hotelId).ToList();
                    
                    IEnumerable<Parser_RoomData> actualParserData = new List<Parser_RoomData>();
                    if (context.Parser_RoomDatas.Any(r => r.Parser_RoomInfo.HotelId == hotelId))
                    {
                        var lastDate = context.Parser_RoomDatas.Where(r => r.Parser_RoomInfo.HotelId == hotelId).Max(r => r.CreationDate);
                        actualParserData = context.Parser_RoomDatas.Where(r => r.Parser_RoomInfo.HotelId == hotelId && r.CreationDate == lastDate).ToList();
                    }

                    IEnumerable<Inflation> inflations = context.Inflations.ToList();

                    IRmsAdapter rmsAdapter = new RmsAdapter();
                    string startDateStr= ConfigurationManager.AppSettings["start_date"];
                    DateTime startDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(startDateStr))
                        startDate = DateTime.ParseExact(startDateStr, "dd.MM.yyyy", null);
                    var result = rmsAdapter.Run(startDate, hotel.PlanningHorizon, hotel.HistoryPeriod, reservations, inflations, events, actualParserData, hotel.Settings,hotel.BookingRating).ToList();

                    var calculatedOn = DateTime.Now;
                    foreach (Calculation calculation in result)
                    {
                        calculation.CalculatedOn = calculatedOn;
                        calculation.HotelId = hotel.Id;
                        calculation.Settings = hotel.Settings;
                    }

                    if (result.Any())
                    {
                        context.Calculations.AddRange(result);
                        context.Logs.Add(new Log { LogType = LogType.Info, CreatedOn = DateTime.Now, Body = "New predictions have been successfully calculated.", HotelId = hotelId });
                        hotel.IsNeedRecalc = false;
                        context.SaveChanges();
                    }
                }
                else
                {
                    context.Logs.Add(new Log { LogType = LogType.Info, CreatedOn = DateTime.Now, Body = "New data not found for calculate predictions.", HotelId = hotelId });
                    context.SaveChanges();
                }
            }
        }
    }
}