using System;
using System.Collections.Generic;
using System.Linq;
using HotelGlob.RMS.Data.Models;
using Hotels.Core;
using Hotels.Core.ForecastRounders;
using Hotels.Core.QuadraticProgramming.ProblemSolvers;
using Hotels.Core.QuadraticProgramming.ProblemTransformers;
using Hotels.Data;
using Hotels.Basic;
using Hotels.Config;

namespace HotelGlob.RMS.Adapter
{
    public class RmsAdapter : IRmsAdapter
    {
        private GroupSolver groupSolver;

        public IEnumerable<Calculation> Run(DateTime startDate, int planningHorizon, int pastPeriod,
            IEnumerable<Reservation> reservations, IEnumerable<Inflation> inflations, IEnumerable<Event> events, IEnumerable<Parser_RoomData> parserRoomData,
            string settings, double rating)
        {
            //new DummyDataProvider(startDate, ConfigurationManager.AppSettings["orders"], ConfigurationManager.AppSettings["inflation"], settings)

            var configuration = new DefoConfiguration(settings);
            Dictionary<int, int> roomTypesIndexes = new Dictionary<int, int>();
            foreach (var r in configuration.ConfigurationRoot.RoomTypes.RoomTypeDescriptions)
                roomTypesIndexes.Add(r.Number, configuration.ConfigurationRoot.RoomTypes.RoomTypeDescriptions.FindIndex(c => c.Number == r.Number));
            Dictionary<int, int> mealTypesIndexes = new Dictionary<int, int>();
            if(configuration.ConfigurationRoot.MealTypes?.MealTypeDescriptions!=null&& configuration.ConfigurationRoot.MealTypes.MealTypeDescriptions.Any())
                foreach (var m in configuration.ConfigurationRoot.MealTypes.MealTypeDescriptions)
                    mealTypesIndexes.Add(m.Number, configuration.ConfigurationRoot.MealTypes.MealTypeDescriptions.FindIndex(c => c.Number == m.Number));
            var solver = new Solver(startDate, planningHorizon, pastPeriod,
                new DummyDataProvider(
                    startDate,
                    inflations.Select(i => new InflationCoef { Date = i.Date, Coef = i.Coef }).ToList(),
                    reservations.Where(c => mealTypesIndexes.Keys.Contains(c.MealTypeId) && roomTypesIndexes.Keys.Contains(c.RoomTypeId)).
                    Select(r => new InternalOrder
                    {
                        BookingTime = r.OrderDate,
                        CheckIn = r.CheckInDate,
                        PeopleNum= r.PeopleNum,
                        LengthOfStay = r.DaysCount,
                        RoomPrice = r.RoomPrice,
                        MealPrice = r.MealPrice,
                        RoomType = roomTypesIndexes[r.RoomTypeId],
                        Status = MapStatus(r.ReservationStatus),
                        MealType = mealTypesIndexes[r.MealTypeId],
                        Type = MapType(r.ReservationType)
                    }).ToList(),
                    configuration,
                    events.Select(c=>new InternalEvent { Name=c.Name,Coef=c.Coef,PriceCoef=c.PriceCoef,Start=c.Start,End= c.End}), 
                    parserRoomData?.Select(c=>new InternalParserRoomData { AverageOccupancy=c.AverageOccupancy,Date=c.Date,HotelName=c.Parser_RoomInfo.HotelName,MealType= mealTypesIndexes[c.Parser_RoomInfo.MealTypeId] , RoomType = roomTypesIndexes[c.Parser_RoomInfo.RoomTypeId], Price=c.Price, StayLength=c.StayLength})),
                new MatLabProblemSolver(),
                configuration,
                new FloorForecastRounder(),
                new MatLabProblemTransformer(),rating);

            solver.Run();

            List<Calculation> result = new List<Calculation>();

            for (int d = 0; d < planningHorizon; ++d)
            {
                var predictionDate = startDate.AddDays(d);

                List<Prediction> predictions = new List<Prediction>();
                for (int r = 0; r < solver.TotalRoomTypes; ++r)
                {
                    var rt=configuration.ConfigurationRoot.RoomTypes.RoomTypeDescriptions[r];
                    var pnMin = rt.PeopleNum;
                    var pnMax = rt.PeopleNum;
                    if (configuration.ConfigurationRoot.RoomTypes.RoomTypeCoefs.Any())
                    {
                        var pn = configuration.ConfigurationRoot.RoomTypes.RoomTypeCoefs.Select(c => c.PeopleNum);
                        pnMin = Math.Min(pn.Min(), rt.PeopleNum);
                        pnMax = Math.Max(pn.Max(), rt.PeopleNum);
                    }
                    for (int c = 0; c < solver.TotalCategories; ++c)
                        for(int n= pnMin; n<= pnMax; n++)
                            for (int m = 0; m < solver.TotalMealTypes; ++m)
                            {
                                int mealTypeId = (int) MealTypeEnum.OB;
                                if (configuration.ConfigurationRoot.MealTypes?.MealTypeDescriptions != null && configuration.ConfigurationRoot.MealTypes.MealTypeDescriptions.Any())
                                    mealTypeId = configuration.ConfigurationRoot.MealTypes.MealTypeDescriptions[m].Number;
                                predictions.Add(new Prediction
                                {
                                    MealTypeId = mealTypeId,
                                    CategoryType = c,
                                    RoomTypeId = rt.Number,
                                    PeopleNum = n,
                                    Price = solver.GetPrice(predictionDate, r, c, m, n),
                                    Cancelation = solver.GetCancelation(predictionDate, r, c, m),
                                    NoShows = solver.GetNoShows(predictionDate, r, c, m),
                                    ExpectedLoad = solver.GetExpectedLoad(predictionDate, r, c, m)
                                });
                            }
                }
                result.Add(new Calculation
                {
                    PredictionDate = predictionDate,
                    Predictions = predictions
                });
            }
            return result;
        }
        public IEnumerable<GroupCalculationResult> GetGroupCalculationResult()
        {
            return groupSolver.GetResult();
        }
        public void RunGroupCalculation(DateTime start, DateTime end, IEnumerable<Reservation> reservations, IEnumerable<Reservation> groupReservations, IEnumerable<Calculation> predictions,
            string settings)
        {
            var configuration = new DefoConfiguration(settings);
            Dictionary<int, int> roomTypesIndexes = new Dictionary<int, int>();
            foreach (var r in configuration.ConfigurationRoot.RoomTypes.RoomTypeDescriptions)
                roomTypesIndexes.Add(r.Number, configuration.ConfigurationRoot.RoomTypes.RoomTypeDescriptions.FindIndex(c => c.Number == r.Number));
            Dictionary<int, int> mealTypesIndexes = new Dictionary<int, int>();
            if (configuration.ConfigurationRoot.MealTypes?.MealTypeDescriptions != null && configuration.ConfigurationRoot.MealTypes.MealTypeDescriptions.Any())
                foreach (var m in configuration.ConfigurationRoot.MealTypes.MealTypeDescriptions)
                    mealTypesIndexes.Add(m.Number, configuration.ConfigurationRoot.MealTypes.MealTypeDescriptions.FindIndex(c => c.Number == m.Number));
            groupSolver = new GroupSolver(DateTime.Today, configuration.ConfigurationRoot, start, end,
                     new GroupDataProvider(reservations.Where(c=> mealTypesIndexes.Keys.Contains(c.MealTypeId)&& roomTypesIndexes.Keys.Contains(c.RoomTypeId)).
                     Select(r => new InternalOrder
                     {
                         BookingTime = r.OrderDate,
                         CheckIn = r.CheckInDate,
                         LengthOfStay = r.DaysCount,
                         RoomPrice = r.RoomPrice,
                         MealPrice = r.MealPrice,
                         RoomType = roomTypesIndexes[r.RoomTypeId],
                         Status = MapStatus(r.ReservationStatus),
                         MealType = mealTypesIndexes[r.MealTypeId],
                         Type = MapType(r.ReservationType)
                     }).ToList(),
                    groupReservations.Where(c => mealTypesIndexes.Keys.Contains(c.MealTypeId) && roomTypesIndexes.Keys.Contains(c.RoomTypeId)).
                    Select(r => new InternalOrder
                    {
                        BookingTime = r.OrderDate,
                        CheckIn = r.CheckInDate,
                        LengthOfStay = r.DaysCount,
                        RoomPrice = r.RoomPrice,
                        MealPrice = r.MealPrice,
                        RoomType = roomTypesIndexes[r.RoomTypeId],
                        Status = MapStatus(r.ReservationStatus),
                        MealType = mealTypesIndexes[r.MealTypeId],
                        Type = MapType(r.ReservationType)
                    }).ToList(), predictions.Select(c =>c==null? null : new InternalCalculation
                    {
                        PredictionDate = c.PredictionDate,
                        Predictions = c.Predictions.Where(t => mealTypesIndexes.Keys.Contains(t.MealTypeId)).
                        Select(s => new InternalPrediction
                        {
                            Cancelation = s.Cancelation,
                            CategoryType = s.CategoryType,
                            ExpectedLoad = s.ExpectedLoad,
                            MealType = mealTypesIndexes.Keys.Contains(s.MealTypeId)? mealTypesIndexes[s.MealTypeId] : 0,
                            NoShows = s.NoShows,
                            Price = s.Price,
                            RoomType = roomTypesIndexes[s.RoomTypeId]
                        })
                    }), configuration
                    ));
               

            groupSolver.Run();
        }
        public IEnumerable<GroupCalculationResult> RunGroupCalculationPriceCalculation(GroupCalcInput input)
        {
            return groupSolver.CalculatePrices(input);
        }
        public IEnumerable<Reservation> CreateDB(DateTime start, string settings, int hotelId)
        {
            var configuration = new DefoConfiguration(settings);
            var solver = new CreateDBSolver(start, configuration.ConfigurationRoot);
            solver.Run();
            var bookings = solver.GetBookings();
            return bookings.Select(c => new Reservation { CreatedOn= c.OrderTime, CheckInDate = c.CheckIn, OrderDate = c.OrderTime, DaysCount = c.LengthOfStay, HotelId = hotelId, MealTypeId = c.MealType, RoomPrice = c.PricePerNight, RoomTypeId = c.RoomType, ReservationStatus = MapStatus(c.Status), ReservationType = MapType(c.Type) });
        }
        private BookingType MapType(ReservationType type)
        {
            switch (type)
            {
                case ReservationType.Event: return BookingType.Event;
                case ReservationType.Group: return BookingType.Group;
                case ReservationType.RackRate: return BookingType.RackRate;
                case ReservationType.Usual: return BookingType.Usual;
                default: return BookingType.Usual;
            }
        }
        private BookingStatus MapStatus(ReservationStatus status)
        {
            switch (status)
            {
                case ReservationStatus.CheckIn: return BookingStatus.CheckIn;
                case ReservationStatus.IsCancelled: return BookingStatus.Canceled;
                case ReservationStatus.NoShow: return BookingStatus.NoShows;
                default: return BookingStatus.CheckIn;
            }
        }
        private int GetMealTypeIndex(int? Id, DefoConfiguration config)
        {
            return Id.HasValue ? config.ConfigurationRoot.MealTypes.MealTypeDescriptions.FindIndex(k => k.Number == Id.Value) : 0;
        }
        private ReservationStatus MapStatus(BookingStatus status)
        {
            switch (status)
            {
                case BookingStatus.CheckIn: return ReservationStatus.CheckIn;
                case BookingStatus.Canceled: return ReservationStatus.IsCancelled;
                case BookingStatus.NoShows: return ReservationStatus.NoShow;
                default: return ReservationStatus.CheckIn;
            }
        }
        private ReservationType MapType(BookingType type)
        {
            switch (type)
            {
                case BookingType.Event: return ReservationType.Event;
                case BookingType.Group: return ReservationType.Group;
                case BookingType.RackRate: return ReservationType.RackRate;
                case BookingType.Usual: return ReservationType.Usual;
                default: return ReservationType.Usual;
            }
        }
    }
}
