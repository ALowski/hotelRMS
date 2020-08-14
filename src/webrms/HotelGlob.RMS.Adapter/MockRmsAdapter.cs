using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelGlob.RMS.Data.Models;
using Hotels.Basic;

namespace HotelGlob.RMS.Adapter
{
    public class MockRmsAdapter : IRmsAdapter
    {
        public IEnumerable<Calculation> Run(DateTime startDate, int planningHorizon, int pastPeriod,
            IEnumerable<Reservation> reservations, IEnumerable<Inflation> inflations, IEnumerable<Event> events, IEnumerable<Parser_RoomData> parserRoomData,
            string settings, double rating)
        { 
            Random random = new Random();
            return new List<Calculation>
            {
                new Calculation
                {
                    PredictionDate = DateTime.Now.AddDays(1),
                    Predictions = new List<Prediction>
                    {
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() }
                    }
                },
                new Calculation
                {
                    PredictionDate = DateTime.Now.AddDays(2),
                    Predictions = new List<Prediction>
                    {
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() }
                    }
                },
                new Calculation
                {
                    PredictionDate = DateTime.Now.AddDays(3),
                    Predictions = new List<Prediction>
                    {
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() }
                    }
                },
                new Calculation
                {
                    PredictionDate = DateTime.Now.AddDays(4),
                    Predictions = new List<Prediction>
                    {
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() }
                    }
                },
                new Calculation
                {
                    PredictionDate = DateTime.Now.AddDays(5),
                    Predictions = new List<Prediction>
                    {
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() }
                    }
                },
                new Calculation
                {
                    PredictionDate = DateTime.Now.AddDays(6),
                    Predictions = new List<Prediction>
                    {
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() }
                    }
                },
                new Calculation
                {
                    PredictionDate = DateTime.Now.AddDays(7),
                    Predictions = new List<Prediction>
                    {
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() }
                    }
                },
                new Calculation
                {
                    PredictionDate = DateTime.Now.AddDays(8),
                    Predictions = new List<Prediction>
                    {
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() },
                        new Prediction{ CategoryType = random.Next(0,4), MealTypeId = random.Next(0,4), RoomTypeId = random.Next(0,4), ExpectedLoad = random.Next(), Price = random.NextDouble() }
                    }
                }
            };
        }
        public void RunGroupCalculation(DateTime start, DateTime end, IEnumerable<Reservation> reservations, IEnumerable<Reservation> groupReservations, IEnumerable<Calculation> predictions,
            string settings)
        {
            return;
        }
        public IEnumerable<GroupCalculationResult> GetGroupCalculationResult()
        {
            return null;
        }
        public IEnumerable<GroupCalculationResult> RunGroupCalculationPriceCalculation(GroupCalcInput input)
        {
            return null;
        }
    }
}
