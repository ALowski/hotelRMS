using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hotels.Basic;
using Hotels.Config;
using Hotels.Config.ConfigModel;
using Hotels.Core.DaytimeExtrapolators;
using Hotels.Core.DemandSlopePredictors;
using Hotels.Core.ForecastRounders;
using Hotels.Core.LoadPredictors;
using Hotels.Core.QuadraticProgramming;
using Hotels.Core.QuadraticProgramming.ProblemSolvers;
using Hotels.Core.QuadraticProgramming.ProblemTransformers;
using Hotels.Core.Tools;

namespace Hotels.Core
{
    public interface ISolver
    {
        void Run();        
    }
    public sealed class CreateDBSolver : ISolver
    {
        private ConfigurationRoot _defoConfiguration;
        IEnumerable<BookingEx> _bookings;
        private DateTime _today;
        public CreateDBSolver(DateTime start, ConfigurationRoot df)
        {
            _today = start;
            _defoConfiguration = df;
        }
        public void Run()
        {
            var result = new List<BookingEx>();
            double[,] _emptyRooms= new double[425,_defoConfiguration.RoomTypes.Total];
            DateTime startDate = _today.AddDays(-365);
            Random rand = new Random();
            for (int i = 0; i < 425; i++)
                foreach (var r in _defoConfiguration.RoomTypes.RoomTypeDescriptions)
                    _emptyRooms[i,r.Number] = rand.Next(r.Quantity / 2, r.Quantity);
            foreach (var r in _defoConfiguration.RoomTypes.RoomTypeDescriptions)
            {
                for (int i = 0; i < 365; i++)
                {
                    int maxIndex=30;
                    while (_emptyRooms[i, r.Number] > 0)
                    {
                        var booking = new BookingEx();
                        booking.RoomType = r.Number;
                        booking.CheckIn = startDate.AddDays(i);
                        booking.OrderTime = booking.CheckIn.AddDays(-rand.Next(0, 60));
                        booking.RoomPrice = rand.Next((int)r.LowerBound, (int)r.UpperBound);
                        booking.MealType= rand.Next(0,_defoConfiguration.MealTypes.Total);
                        booking.Type = rand.NextDouble() > 0.15 ? BookingType.Usual : BookingType.Group;
                        var st = rand.NextDouble();
                        if (st < 0.12 && booking.Type == BookingType.Usual)
                            booking.Status = BookingStatus.NoShows;
                        else if (st < 0.24)
                            booking.Status = BookingStatus.Canceled;
                        else
                            booking.Status = BookingStatus.CheckIn;

                        booking.LengthOfStay = rand.Next(1, maxIndex+1);
                        if (booking.Status == BookingStatus.CheckIn)
                        {
                            for (int j = 1; j < booking.LengthOfStay; j++)
                            {
                                if (_emptyRooms[i+j, r.Number] > 0)
                                    _emptyRooms[i + j, r.Number]--;
                                else
                                {
                                    maxIndex = j;
                                    booking.LengthOfStay = maxIndex;
                                }
                            }
                            _emptyRooms[i, r.Number]--;
                        }
                        result.Add(booking);                       
                    }
                }
            }
            _bookings = result;
            System.IO.File.WriteAllLines("D:\\"+startDate.ToShortDateString() + ".csv", _bookings.Select(c => string.Join(";", new string[] {c.OrderTime.ToString(),c.CheckIn.ToString(), c.LengthOfStay.ToString(),c.PricePerNight.ToString(), c.RoomType.ToString(), c.MealType.ToString(),((int) c.Type).ToString(), ((int)c.Status).ToString() })));
        }
        public IEnumerable<BookingEx> GetBookings()
        {
            return _bookings;
        }
    }

        public sealed class GroupSolver : ISolver
    {
        private DateTime _today;
        private ConfigurationRoot _defoConfiguration;
        private DateTime _start;
        private DateTime _end;
        private List<InternalCalculation> _calculations;
        //private readonly GroupCalcInput _input;
        private readonly IGroupDataProvider _DataProvider;
        private double[][] cancelation;
        private readonly int[] _seasons;
        private List<GroupCalculationResult> result;
        public GroupSolver(DateTime today, ConfigurationRoot defoConfiguration,DateTime start, DateTime end,IGroupDataProvider DataProvider)
        {
            _calculations = new List<InternalCalculation>();
            result = new List<GroupCalculationResult>();
            _today = today;
            _start = start;
            _end = end;
            _defoConfiguration = defoConfiguration;
            //_input = input;
            _DataProvider = DataProvider;
            int _s = _defoConfiguration.Seasons.Total;
            int _r = _defoConfiguration.RoomTypes.Total;
            cancelation=ArrayHelper.CreateArray<double>(_s, _r);
            for (var s = 0; s < _s; ++s)
                for (var r = 0; r < _r; ++r)
                    cancelation[s][r] = -1;
            var seasons = new int[53];//53 weeks in year
            foreach (var x in _defoConfiguration.Seasons.SeasonDescriptions)
                for (var d = x.Start; d <= x.Finish; ++d)
                    seasons[d - 1] = x.Number;
            int daysNumber = (end -start).Days + 1;
            _seasons = new int[daysNumber];
            
            for (var d = 0; d < daysNumber; ++d)
            {
                var date = start.AddDays(d);
                var weekNumber = date.GetWeekNumber();
                _seasons[d] = (weekNumber < 54) ? seasons[weekNumber - 1] : -1;                
            }
        }
        public IEnumerable<GroupCalculationResult> GetResult()
        {
            return result;
        }
        public IEnumerable<GroupCalculationResult> CalculatePrices(GroupCalcInput input)
        {
            var catId = _defoConfiguration.GetCategoryId((_end - _start).Days + 1, (_start - _today).Days);
            var output = new List<GroupCalculationResult>();
            foreach (var item in input.Inputs)
            {
                double realNumberOfRooms = 0;
                var d = (item.Date - _start).Days;
                var s = _seasons[d];
                var dayIndex = result.FindIndex(c => c.Date == item.Date);
                var calculation = _calculations.Where(c => c != null && c.PredictionDate.Date == item.Date).FirstOrDefault();
                if (calculation == null)
                    continue;
                foreach (var roomType in item.RoomTypes.Where(c=>c.Value.Value>0).ToList())
                {
                    var r = Int32.Parse(roomType.Key);
                    var roomTypeDesc = _defoConfiguration.RoomTypes.RoomTypeDescriptions.FirstOrDefault(c => c.Number == r);
                    if (roomTypeDesc == null)
                        continue;
                    
                    var calcIndex = result[dayIndex].Results.FindIndex(c => c.RoomType == r);
                    var calc = result[dayIndex].Results[calcIndex];
                    double cancelationNumber = roomType.Value.Value * cancelation[s][r];
                    var trueRoomNum = roomType.Value.Value - cancelationNumber;

                    var predictions = calculation.Predictions.Where(c=>c.RoomType==r);
                    if (predictions == null)
                        continue;
                    
                    if (cancelationNumber == roomType.Value.Value && calc.EmptyRooms - trueRoomNum < 0)
                        continue;


                    var replaceNumber = trueRoomNum - calc.PlanEmptyRooms;
                    double price = 0;
                    foreach (var i in predictions.Where(c=>c.ExpectedLoad>0 && c.Price>0).OrderBy(c => c.Price).ToList())
                    {
                        if (replaceNumber <= 0)
                            break;
                        if (replaceNumber >= i.ExpectedLoad)
                            price += i.Price* i.ExpectedLoad;
                        else
                            price += i.Price * replaceNumber;
                        
                        replaceNumber-=i.ExpectedLoad;
                    }
                    var tempPrice = price / (double)trueRoomNum;
                    var minPrice = Math.Max(roomTypeDesc.OperationalCost, tempPrice);
                    var pred = predictions.Where(c => c.MealType == 0 && c.CategoryType == catId).FirstOrDefault();
                    var maxPrice = tempPrice;
                    if (pred != null)
                        maxPrice = Math.Max(pred.Price, tempPrice);
                    
                    calc.MinPrice = Math.Max((cancelationNumber * maxPrice + trueRoomNum * minPrice) / roomType.Value.Value, roomTypeDesc.OperationalCost);
                    calc.MaxPrice = maxPrice< calc.MinPrice?double.NaN:maxPrice;
                    realNumberOfRooms += trueRoomNum;                    
                }

                if (item.MealTypes != null && item.MealTypes.Any())
                {
                    var mealCost = item.MealTypes.Sum(c => c.Value.Value);
                    foreach (var roomType in item.RoomTypes.Where(c => c.Value.Value > 0).ToList())
                    {
                        var calcIndex = result[dayIndex].Results.FindIndex(c => c.RoomType == Int32.Parse(roomType.Key));
                        var calc = result[dayIndex].Results[calcIndex];
                        calc.MinPrice -= mealCost / realNumberOfRooms;                        
                    }
                }
                output.Add(result[dayIndex]);
            }
            output.ForEach(c => c.Results.ForEach(s => s.RoomType=_defoConfiguration.RoomTypes.RoomTypeDescriptions[s.RoomType].Number));
            return output;
        }
        public void Run()
        {
            for (var i = _start; i <= _end; i = i.AddDays(1))
            {
                var d = (i - _start).Days;
                var s = _seasons[d];
                var calc = new GroupCalculationResult { Date = i, Results = new List<GroupCalculationDayResult>() };
                var calculation = _DataProvider.GetPredictions(i);
                if (calculation == null)
                    continue;
                _calculations.Add(calculation);
                foreach (var roomType in _defoConfiguration.RoomTypes.RoomTypeDescriptions)
                {
                    var r = _defoConfiguration.RoomTypes.RoomTypeDescriptions.IndexOf(roomType);
                    if (cancelation[s][r] < 0)
                    {
                        var groupBookings = _DataProvider.GetGroupBookings(_seasons[d], r);
                        if (groupBookings.Any())
                            cancelation[s][r] = (double)groupBookings.Where(c => c.Status != BookingStatus.CheckIn).Count() /groupBookings.Count();
                        else
                            cancelation[s][r] = 0;
                    }
                    var bookings = _DataProvider.GetPlanBookings(i, r);
                    var planGroupBookings = bookings.Where(c => c.Type == BookingType.Group);
                    var planGroupBookingsCancel = Math.Min(planGroupBookings.Count() * cancelation[s][r], planGroupBookings.Count());
                    var dayResult = new GroupCalculationDayResult { RoomType = r, EmptyRooms = 0, PlanEmptyRooms = 0, MaxPrice = 0, MinPrice = 0 };

                    var predictions = calculation.Predictions.Where(c => c.RoomType == r);
                    if (predictions == null)
                        continue;
                    double planBookingsCancel = 0.0;
                    var planBookings = bookings.Where(c => c.Type == BookingType.Usual);
                    dayResult.EmptyRooms = roomType.Quantity - planBookings.Count() - planGroupBookings.Count() + planGroupBookingsCancel;
                    dayResult.PlanEmptyRooms = dayResult.EmptyRooms;
                    if (predictions != null)
                    {
                        planBookingsCancel = Math.Min(planBookings.Count(), predictions.Sum(c => c.NoShows + c.Cancelation));
                        dayResult.EmptyRooms += planBookingsCancel;
                        dayResult.PlanEmptyRooms = dayResult.EmptyRooms - predictions.Sum(c => c.ExpectedLoad);
                    }
                    calc.Results.Add(dayResult);
                }
                result.Add(calc);
            }            
        }        
    
    }
        public sealed class Solver : ISolver
    {
        private readonly IDataProvider _dataProvider;
        private readonly IEnumerable<ChildRooms> _childRooms;        
        private readonly IForecastRounder _forecastRounder;
        private readonly IProblemSolver _problemSolver;
        private readonly IProblemTransformer _problemTransformer;
        private int _planningHorizon;
        private double _rating;
        private DateTime _today;
        private ConfigurationRoot _defoConfiguration;

        /* season weekday roomtype category */
        private readonly double[][][,,] L;
        private readonly double[][][,,] U;
        private readonly double[][][,,] b;
        
        private readonly IDemandSlopePredictor[][][][][] _sp;
        private readonly ILoadPredictor[][][][][] _lp;
        private readonly ILoadPredictor[][][][][] _cp;
        private readonly ILoadPredictor[][][][][] _np;
        
        /* roomtype and meal */
        private readonly double[,] h;

        /* day roomtype */
        private readonly double[][] R;

        /* day roomtype category */
        private readonly double[][,,] a;
        private readonly double[][,,] p;
        private readonly double[][,,] cancelation;
        private readonly double[][,,] noShows;
        private readonly double[][,,] f;
        private readonly double[][,,] UFinal;
        private readonly double[][,,] LFinal;
        private readonly double[][,,] y;

        /* day */
        private readonly int[] futureSeasons;
        private readonly double[][] eventCoefs;
        private readonly int[] futureWeekdays;
        private readonly int[] pastSeasons;
        private readonly bool[] pastEvents;
        private readonly int[] pastWeekdays;
        private readonly bool[][][] pastDays;
        private readonly bool[][][][] futureDays;
        private readonly DateTime historyStart ;
        private readonly PriceRelation[] T;
        
        private readonly Dictionary<int, OptionDescription> catStayLength;
        private readonly Dictionary<int, int> catStartDay;

        public Solver(
            DateTime today,
            int planningHorizon,
            int pastPeriod,
            IDataProvider dataProvider,
            IProblemSolver problemSolver,
            IDefoConfiguration defoConfiguration,
            IForecastRounder forecastRounder,
            IProblemTransformer problemTransformer,
            double rating)
        {
            _today = today.Date;
            _planningHorizon = planningHorizon;
            _rating = rating;
            _dataProvider = dataProvider;
            _problemSolver = problemSolver;
            _forecastRounder = forecastRounder;
            _problemTransformer = problemTransformer;
            // replace Id with index
            _childRooms = new List<ChildRooms>();
            if (defoConfiguration?.ConfigurationRoot?.RoomTypes?.ChildRooms!=null&&defoConfiguration.ConfigurationRoot.RoomTypes.ChildRooms.Any())
            {
                _childRooms = defoConfiguration.ConfigurationRoot.RoomTypes.ChildRooms.Select(c => new ChildRooms
                {
                    Child = _defoConfiguration.RoomTypes.RoomTypeDescriptions.FindIndex(s => s.Number == c.Child),
                    Parent = _defoConfiguration.RoomTypes.RoomTypeDescriptions.FindIndex(s => s.Number == c.Parent),
                    Quantity = c.Quantity
                });
            }
        
            _defoConfiguration = defoConfiguration.ConfigurationRoot;
            TotalRoomTypes = _defoConfiguration.RoomTypes.Total;
            TotalCategories = _defoConfiguration.Categories.Total;
            TotalMealTypes = _defoConfiguration.MealTypes.UseInDynamicCalculation?_defoConfiguration.MealTypes.Total:1;
            
            int _s = _defoConfiguration.Seasons.Total;
            int _w = _defoConfiguration.Weekdays.Total;
            int _r = TotalRoomTypes;
            int _c = TotalCategories;
            int _m = TotalMealTypes;
            if (_s == 0 || _w == 0 || _c == 0 || _r == 0 || _m == 0)
                return;
            _sp = ArrayHelper.CreateArray<IDemandSlopePredictor>(_s, _w, _r, _c, _m);
            _lp = ArrayHelper.CreateArray<ILoadPredictor>(_s, _w, _r, _c, _m);
            _cp = ArrayHelper.CreateArray<ILoadPredictor>(_s, _w, _r, _c, _m);
            _np = ArrayHelper.CreateArray<ILoadPredictor>(_s, _w, _r, _c, _m);

            h = new double[_r, _m];// ArrayHelper.CreateArray<double>(_r, _m);
            R = ArrayHelper.CreateArray<double>(planningHorizon, _r);

            L = ArrayHelper.CreateArray<double[,,]>(_s, _w);
            U = ArrayHelper.CreateArray<double[,,]>(_s, _w);
            b = ArrayHelper.CreateArray<double[,,]>(_s, _w);

            a = ArrayHelper.CreateArray<double[,,]>(planningHorizon);
            eventCoefs= ArrayHelper.CreateArray<double>(planningHorizon, 2);
            p = ArrayHelper.CreateArray<double[,,]>(planningHorizon);
            cancelation = ArrayHelper.CreateArray<double[,,]>(planningHorizon);
            noShows = ArrayHelper.CreateArray<double[,,]>(planningHorizon);
            f = ArrayHelper.CreateArray<double[,,]>(planningHorizon);
            UFinal = ArrayHelper.CreateArray<double[,,]>(planningHorizon);
            LFinal = ArrayHelper.CreateArray<double[,,]>(planningHorizon);
            y = ArrayHelper.CreateArray<double[,,]>(planningHorizon);
            pastDays =  ArrayHelper.CreateArray<bool>(_s, _w, planningHorizon);
            futureDays =  ArrayHelper.CreateArray<bool>(_s, _w, _c, planningHorizon);

            for (var i = 0; i < planningHorizon; ++i)
            {
                a[i] = new double[_r, _c, _m];
                p[i] = new double[_r, _c, _m];
                cancelation[i] = new double[_r, _c, _m];
                noShows[i] = new double[_r, _c, _m];
                f[i] = new double[_r, _c, _m];
                UFinal[i] = new double[_r, _c, _m];
                LFinal[i] = new double[_r, _c, _m];
                y[i] = new double[_r, _c, _m];
            }
            for (var s = 0; s < _s; ++s)
                for (var w = 0; w < _w; ++w)
                {
                    b[s][w] = new double[_r, _c, _m];
                    L[s][w] = new double[_r, _c, _m];
                    U[s][w] = new double[_r, _c, _m];
                }


            for (var r = 0; r < _r; ++r)
            {
                var roomType = _defoConfiguration.RoomTypes.RoomTypeDescriptions[r];
                for (var m = 0; m < _m; ++m)
                {
                    if (_defoConfiguration.MealTypes.UseInDynamicCalculation)
                        h[r, m] = roomType.OperationalCost + _defoConfiguration.MealTypes.MealTypeDescriptions[m].OperationalCost* roomType.PeopleNum;
                    for (var d = 0; d < planningHorizon; ++d) R[d][r] = roomType.Quantity;
                    for (var s = 0; s < _s; ++s)
                        for (var w = 0; w < _w; ++w)
                            for (var c = 0; c < _c; ++c)
                            {
                                var mbp = (roomType.MarketBasePrice != 0) ? roomType.MarketBasePrice : (roomType.LowerBound + roomType.UpperBound) / 2;

                                L[s][w][r, c, m] = (roomType.LowerBound != 0) ? roomType.LowerBound : mbp * 0.5;
                                U[s][w][r, c, m] = (roomType.UpperBound != 0) ? roomType.UpperBound : mbp * 1.5;
                                if (_defoConfiguration.MealTypes.UseInDynamicCalculation)
                                {
                                    L[s][w][r, c, m] += _defoConfiguration.MealTypes.MealTypeDescriptions[m].MarketBasePrice * roomType.PeopleNum;
                                    U[s][w][r, c, m] += _defoConfiguration.MealTypes.MealTypeDescriptions[m].MarketBasePrice * roomType.PeopleNum;
                                }
                            }
                }
            }
            for (var s = 0; s < _s; ++s)            
                for (var w = 0; w < _w; ++w)
                    for (var r = 0; r < _r; ++r)
                        for (var c = 0; c < _c; ++c)
                            for (var m = 0; m < _m; ++m)
                            {
                                var reductionSum = 0.0;
                                var reductions = _defoConfiguration.Seasons.PriceReductions.Where(v => v.Number == s).FirstOrDefault();
                                if (reductions != null)
                                    reductionSum += reductions.Amount;
                                reductions = _defoConfiguration.Weekdays.PriceReductions.Where(v => v.Number == w).FirstOrDefault();
                                if (reductions != null)
                                    reductionSum += reductions.Amount;
                                reductions = _defoConfiguration.Categories.PriceReductions.Where(v => v.Number == c).FirstOrDefault();
                                if (reductions != null)
                                    reductionSum += reductions.Amount;

                                L[s][w][r, c, m] *= (1 - reductionSum);
                                U[s][w][r, c, m] *= (1 - reductionSum);
                            }
            var t = new List<PriceRelation>();
            if (_defoConfiguration.MealTypes.UseInDynamicCalculation)
            {
                foreach (var m in _defoConfiguration.MealTypes.PriceConstraints)
                    for (var r = 0; r < _r; ++r)
                    {
                        var less = _defoConfiguration.MealTypes.MealTypeDescriptions.FindIndex(c => c.Number == m.Less);
                        var more = _defoConfiguration.MealTypes.MealTypeDescriptions.FindIndex(c => c.Number == m.More);
                        t.Add(new PriceRelation { R1 = r, R2 = r, M1 = less, M2 = more });
                    }
            }
            foreach (var x in _defoConfiguration.RoomTypes.PriceConstraints)
                for (var m = 0; m < _m; ++m)
                {
                    var less = _defoConfiguration.RoomTypes.RoomTypeDescriptions.FindIndex(c => c.Number == x.Less);
                    var more = _defoConfiguration.RoomTypes.RoomTypeDescriptions.FindIndex(c => c.Number == x.More);
                    t.Add(new PriceRelation { R1 = less, R2 = more, M1 = m, M2 = m });
                }
            T = t.ToArray();


            var weekdays = new[]
            {
                _defoConfiguration.Weekdays.Sunday,
                _defoConfiguration.Weekdays.Monday,
                _defoConfiguration.Weekdays.Tuesday,
                _defoConfiguration.Weekdays.Wednesday,
                _defoConfiguration.Weekdays.Thursday,
                _defoConfiguration.Weekdays.Friday,
                _defoConfiguration.Weekdays.Saturday
            };

            var seasons = new int[53];//53 weeks in year
            foreach (var x in _defoConfiguration.Seasons.SeasonDescriptions)
                for (var d = x.Start; d <= x.Finish; ++d)
                    seasons[d - 1] = x.Number;

            pastSeasons = new int[pastPeriod];
            pastWeekdays = new int[pastPeriod];
            pastEvents = new bool[pastPeriod];
            for (var d = 1; d <= pastPeriod; ++d)
            {
                var pastDay = _today.AddDays(-d);
                var events = _dataProvider.GetEvents(pastDay);
                pastEvents[pastPeriod - d] = false;
                if (events!=null)
                    pastEvents[pastPeriod - d] = events.Any();
                var weekNumber = pastDay.GetWeekNumber();
                pastSeasons[pastPeriod - d] = (weekNumber < 54) ? seasons[weekNumber - 1] : -1;
                pastWeekdays[pastPeriod - d] = weekdays[(int)pastDay.DayOfWeek];
            }

            futureSeasons = new int[planningHorizon];
            futureWeekdays = new int[planningHorizon];

            for (var d = 0; d < planningHorizon; ++d)
            {
                var futureDay = _today.AddDays(d);
                var events = _dataProvider.GetEvents(futureDay);
                eventCoefs[d][0] = 1;
                eventCoefs[d][1] = 1;
                if (events != null&& events.Any())
                {
                    foreach (var item in events)
                    {
                        eventCoefs[d][0] *= item.Coef;
                        eventCoefs[d][1] *= item.Coef;
                    }
                } 
                var weekNumber = futureDay.GetWeekNumber();
                futureSeasons[d] = (weekNumber < 54) ? seasons[weekNumber - 1] : -1;
                futureWeekdays[d] = weekdays[(int)futureDay.DayOfWeek];
            }
            //Calculate exception days for categories and length of stay
            catStartDay = new Dictionary<int, int>();
            catStayLength = new Dictionary<int, OptionDescription>();
            var bookingsTotal = _defoConfiguration.Categories.BookingPeriods.Count();
            foreach (OptionDescription x in _defoConfiguration.Categories.StayPeriods)
                foreach (OptionDescription i in _defoConfiguration.Categories.BookingPeriods)
                {
                    catStartDay.Add(x.Number * bookingsTotal + i.Number, i.LowerBound);
                    catStayLength.Add(x.Number * bookingsTotal + i.Number, x);
                }
            for (var s = 0; s < _s; ++s)
                for (var w = 0; w < _w; ++w)
                {
                    pastDays[s][w] = pastSeasons.Zip(pastWeekdays, (i, j) => (i == s && j == w)).Zip(pastEvents, (i, j) => (i  && !j)).ToArray();
                    for (var c = 0; c < _c; ++c)
                    {
                        futureDays[s][w][c] = futureSeasons.Zip(futureWeekdays, (i, j) => (i == s && j == w)).ToArray();
                        for (int d = 0; d < _planningHorizon; ++d)
                            futureDays[s][w][c][d] = catStartDay[c] > d ? false : futureDays[s][w][c][d];
                    }
                }
            historyStart= _today.AddDays(-pastSeasons.Length);
        }

        public void Run()
        {
            if (TotalCategories == 0 || TotalMealTypes == 0 || TotalRoomTypes == 0 || b.Length==0|| b[0].Length==0)
                return;
            List<Task> tasks = new List<Task>();
            for (var Ss = 0; Ss < b.Length; ++Ss)
                for (var Rr = 0; Rr < TotalRoomTypes; ++Rr)
                {
                    var bookings = _dataProvider.GetGroupBookings(historyStart, _today,Ss,Rr);
                    double groupCancelation = 0.0;
                    if (bookings.Any())
                        groupCancelation = (double)bookings.Where(c => c.Status != BookingStatus.CheckIn).Count() / bookings.Count();
                    double[] occupancyTemp = new double[_planningHorizon];
                    foreach (var booking in _dataProvider.GetGroupBookingsForPlan(_today.AddDays(_planningHorizon - 1), _today, Rr, Ss))
                    {
                        if (booking.Status != BookingStatus.CheckIn)
                            continue;
                        var day = (booking.CheckIn.Date - _today.Date).Days; ;
                        
                        for (var d = System.Math.Max(-1, day); d < System.Math.Max(0, day + booking.LengthOfStay); ++d)
                            if (d > -1 && d < _planningHorizon)
                                occupancyTemp[d]--;
                    }
                    for (int d = 0; d < _planningHorizon; ++d)
                        if (futureSeasons[d]==Ss)
                        {
                            occupancyTemp[d] -= groupCancelation* occupancyTemp[d];
                            if (occupancyTemp[d] >= 0)
                                continue;
                            _childRooms.Where(t => t.Parent == Rr).ToList().ForEach(t => R[d][t.Child] += occupancyTemp[d] * t.Quantity);
                            R[d][Rr] += occupancyTemp[d];
                        }
                }
            for (var Ss = 0; Ss < b.Length; ++Ss)
                for (var Ww = 0; Ww < b[0].Length; ++Ww)
                    for (var Rr = 0; Rr < TotalRoomTypes; ++Rr)
                        for (var Cc = 0; Cc < TotalCategories; ++Cc)
                            for (var Mm = 0; Mm < TotalMealTypes; ++Mm)
                            {
                                var s = Ss;
                                var w = Ww;
                                var r = Rr;
                                var c = Cc;
                                var m = Mm;
                                tasks.Add(Task.Run(() =>
                                {
                                    _sp[s][w][r][c][m] = new LeastSquareDemandSlopePredictor();
                                    _lp[s][w][r][c][m] = new GeneralLoadPredictor();
                                    _cp[s][w][r][c][m] = new GeneralLoadPredictor();
                                    _np[s][w][r][c][m] = new GeneralLoadPredictor();

                                    _lp[s][w][r][c][m].Initialize(_today, pastDays[s][w], futureDays[s][w][c], _planningHorizon);
                                    _cp[s][w][r][c][m].Initialize(_today, pastDays[s][w], futureDays[s][w][c], _planningHorizon);
                                    _np[s][w][r][c][m].Initialize(_today, pastDays[s][w], futureDays[s][w][c], _planningHorizon);

                                    var bookings = _dataProvider.GetBookings(historyStart, _today, s, w, r, c, m);
                                    bookings.Where(p => p.Status == BookingStatus.CheckIn).ToList().ForEach(p =>
                                    {
                                        _sp[s][w][r][c][m].AddBooking(p);
                                        _lp[s][w][r][c][m].AddBooking(p);
                                    });
                                    bookings.Where(p => p.Status == BookingStatus.Canceled).ToList().ForEach(p =>
                                    {
                                        _cp[s][w][r][c][m].AddBooking(p);
                                    });
                                    bookings.Where(p => p.Status == BookingStatus.NoShows).ToList().ForEach(p =>
                                    {
                                        _np[s][w][r][c][m].AddBooking(p);
                                    });
                                    //foreach (var booking in _dataProvider.GetBookings(historyStart, _today, s, w, r, c, m))
                                    //{
                                    //    switch (booking.Status)
                                    //    {
                                    //        case BookingStatus.None:
                                    //            _sp[s][w][r][c][m].AddBooking(booking);
                                    //            _lp[s][w][r][c][m].AddBooking(booking);
                                    //            break;
                                    //        case BookingStatus.Canceled:
                                    //            _cp[s][w][r][c][m].AddBooking(booking);
                                    //            break;
                                    //        case BookingStatus.NoShows:
                                    //            _np[s][w][r][c][m].AddBooking(booking);
                                    //            break;

                                    //    }
                                    //}

                                    b[s][w][r, c, m] = _sp[s][w][r][c][m].Calculate();
                                    var fTemp = new double[_planningHorizon];
                                    var fPrice = new double[_planningHorizon];
                                    _lp[s][w][r][c][m].Calculate(fTemp, fPrice);
                                    //calculate canceled
                                    var cTemp = new double[_planningHorizon];
                                    var cPrice = new double[_planningHorizon];
                                    _cp[s][w][r][c][m].Calculate(cTemp, cPrice);
                                    //calculate no shows
                                    var nTemp = new double[_planningHorizon];
                                    var nPrice = new double[_planningHorizon];
                                    _np[s][w][r][c][m].Calculate(nTemp, nPrice);

                                    double[] occupancyTemp = new double[_planningHorizon];

                                    foreach (var booking in _dataProvider.GetBookingsForPlan(_today.AddDays(_planningHorizon - 1), _today, r, s, w, c, m))
                                    {
                                        if (booking.Status != BookingStatus.CheckIn)
                                            continue;
                                        var day = (booking.CheckIn.Date - _today.Date).Days; ;

                                        for (var d = System.Math.Max(-1, day); d < System.Math.Max(0, day + booking.LengthOfStay); ++d)
                                            if (d > -1 && d < _planningHorizon)
                                                occupancyTemp[d]--;
                                    }

                                    for (int d = 0; d < _planningHorizon; ++d)
                                    {
                                        occupancyTemp[d] += nTemp[d] + cTemp[d];
                                        if (occupancyTemp[d] < 0)
                                        {
                                            _childRooms.Where(t => t.Parent == r).ToList().ForEach(t => R[d][t.Child] += occupancyTemp[d] * t.Quantity);
                                            R[d][r] += occupancyTemp[d];
                                        }
                                        
                                        if (futureDays[s][w][c][d])
                                        {
                                            LFinal[d][r, c, m] = L[s][w][r, c, m] * eventCoefs[d][1];
                                            UFinal[d][r, c, m] = U[s][w][r, c, m] * eventCoefs[d][1];
                                            cancelation[d][r, c, m] = cTemp[d];
                                            noShows[d][r, c, m] = nTemp[d];

                                            if (occupancyTemp[d] < 0)
                                                f[d][r, c, m] += occupancyTemp[d];
                                            f[d][r, c, m] =( f[d][r, c, m] + fTemp[d])*eventCoefs[d][0];
                                            if (f[d][r, c, m] < 0)
                                                f[d][r, c, m] = 0;
                                            a[d][r, c, m] = f[d][r, c, m] - fPrice[d] * b[s][w][r, c, m];
                                            a[d][r, c, m] = a[d][r, c, m] > 0 ? a[d][r, c, m] : 0;
                                            
                                        }
                                    }
                                }));
                                
                            }
            for (int d = 0; d < _planningHorizon; ++d)
            {
                var s = futureSeasons[d];
                var w = futureWeekdays[d];
                for (var r = 0; r < TotalRoomTypes; ++r)
                {
                    double fSum = 0.0;
                    for (var c = 0; c < TotalCategories; ++c)
                        for (var m = 0; m < TotalMealTypes; ++m)
                        {
                            if (eventCoefs[d][1] >1 && eventCoefs[d][0] >1 && futureDays[s][w][c][d])
                            {
                                var fTemp = a[d][r, c, m] + b[s][w][r, c, m] * UFinal[d][r, c, m];
                                if(fTemp>0)
                                    fSum += fTemp;
                            }
                        }
                    if (fSum > R[d][r])
                    {
                        eventCoefs[d][0] = R[d][r] / fSum;
                        for (var c = 0; c < TotalCategories; ++c)
                            for (var m = 0; m < TotalMealTypes; ++m)
                            {
                                if (eventCoefs[d][1] >1 && eventCoefs[d][0] >1 && futureDays[s][w][c][d])
                                {
                                    var bp = a[d][r, c, m] - f[d][r, c, m];
                                    f[d][r, c, m] *= eventCoefs[d][0];
                                    a[d][r, c, m] = f[d][r, c, m] + bp;
                                }
                            }
                    }
                }
            }
            Task.WaitAll(tasks.ToArray());
            RecalculatePrices2();
        }

        private ProblemInput GetProblem(int d, double W, bool withA)
        {
            var s = futureSeasons[d];
            var w = futureWeekdays[d];
            return _problemTransformer.Transform(d, TotalRoomTypes, TotalCategories, TotalMealTypes, _childRooms, a[d], b[s][w], h, LFinal[d], UFinal[d], R[d], W, T, withA);
        }
        private ProblemInput GetProblemForW(int d, double inflation)
        {
            var s = futureSeasons[d];
            var w = futureWeekdays[d];
            return _problemTransformer.TransformForW(d, TotalRoomTypes, TotalCategories, TotalMealTypes, a[d], b[s][w], h, LFinal[d], UFinal[d], R[d]);
        }

        private ProblemOutput GetSolution(int d, int s, int w, out double? WValue)
        {
            WValue = null;
            var problemForW = _problemTransformer.TransformForW(d, TotalRoomTypes, TotalCategories, TotalMealTypes, a[d], b[s][w], h, LFinal[d], UFinal[d], R[d]); 
            var result = _problemSolver.Solve(problemForW);
            if (!result.HasSolution)
                return result;
            WValue = result.Value;
            var problem = _problemTransformer.Transform(d, TotalRoomTypes, TotalCategories, TotalMealTypes, _childRooms, a[d], b[s][w], h, LFinal[d], UFinal[d], R[d], WValue.Value, T, true);
            result = _problemSolver.Solve(problem);
            return result;
        }
        private void RecalculatePrices2()// This method better
        {

            //ILoadPredictor[][][][][][] lp = ArrayHelper.CreateArray<ILoadPredictor>(_defoConfiguration.Seasons.Total, _defoConfiguration.Weekdays.Total, TotalRoomTypes, TotalCategories, TotalMealTypes,45);
            try
                {
                    for (int d = 0; d < _planningHorizon; ++d)
                    {

                        var inflation = _dataProvider.GetInflationCoef(_today, _today.AddDays(d));
                        for (int i = 0; i < R[d].Length; ++i)
                            if (!(R[d][i] >= 0)) R[d][i] = 0; //throw new Exception(); дaбавіць ў лог
                        var s = futureSeasons[d];
                        var w = futureWeekdays[d];
                        double? WValue = null;
                        var result = GetSolution(d, s, w, out WValue);
                        //trying to solve with improved a[]
                        if (!result.HasSolution && WValue.HasValue)
                        {
                            var problem = _problemTransformer.Transform(d, TotalRoomTypes, TotalCategories, TotalMealTypes, _childRooms, a[d], b[s][w], h, LFinal[d], UFinal[d], R[d], WValue.Value, T, false);
                            var resultAdditional = _problemSolver.Solve(problem);
                            if (resultAdditional.HasSolution)
                            {
                                var x = resultAdditional.Solution;

                                for (int i = 0, k = 0; i < TotalRoomTypes; ++i)
                                    for (int j = 0; j < TotalCategories; ++j)
                                        for (int m = 0; m < TotalMealTypes; ++m)
                                        {
                                            if (resultAdditional.SkipedIndexes == null || !resultAdditional.SkipedIndexes.Any(t => t.i == i && t.j == j))
                                            {
                                                if (-x[k] * b[s][w][i, j, m] > a[d][i, j, m])
                                                    a[d][i, j, m] = f[d][i, j, m] - x[k] * b[s][w][i, j, m];
                                                ++k;
                                            }
                                        }
                                result = GetSolution(d, s, w, out WValue);
                            }
                        }
                        var solution = result.Solution;

                        for (int i = 0, k = 0; i < TotalRoomTypes; ++i)
                            for (int j = 0; j < TotalCategories; ++j)
                                for (int m = 0; m < TotalMealTypes; ++m)
                                {
                                    if (result.SkipedIndexes == null || !result.SkipedIndexes.Any(t => t.i == i && t.j == j))
                                    {
                                        if (result.HasSolution)
                                        {
                                            p[d][i, j, m] = solution[k];
                                            y[d][i, j, m] = solution[k + solution.Length / 2];
                                            f[d][i, j, m] = a[d][i, j, m] + solution[k] * b[s][w][i, j, m] * inflation;
                                            ++k;
                                        }
                                    }
                                    else if (a[d][i, j, m] <= 0 || R[d][i] < 1)
                                    {
                                        f[d][i, j, m] = 0;
                                        p[d][i, j, m] = 0;
                                    }
                                    else if ((-a[d][i, j, m] / b[s][w][i, j, m]) <= L[s][w][i, j, m])
                                        p[d][i, j, m] = LFinal[d][i, j, m];
                                    else
                                    {
                                        f[d][i, j, m] = 0;
                                        p[d][i, j, m] = UFinal[d][i, j, m];
                                    }

                                    //if (a[d][i, j] <= 0) - no data 
                                    //if( R[d][i]<1) - no vacant rooms
                                    //(-a[d][i, j] / b[s][w][i, j]) <= L[s][w][i, j]* inflation- demand is too low

                                    //Calculate planed forecast effect on future days -temporary unused
                                    //if (f[d][i, j, m] > 0)
                                    //{
                                    //    double[] forecasts = new double[catStayLength[j].UpperBound - catStayLength[j].LowerBound + 1];
                                    //    for (int l = catStayLength[j].LowerBound; l <= catStayLength[j].UpperBound; l++)
                                    //    {
                                    //        var fTemp = new double[_planningHorizon];
                                    //        if (lp[s][w][i][j][m][l] == null)
                                    //        {
                                    //            lp[s][w][i][j][m][l] = new GeneralLoadPredictor();
                                    //            lp[s][w][i][j][m][l].Initialize(_today, pastDays[s][w], futureDays[s][w][j], _planningHorizon);
                                    //            var bookings = _dataProvider.GetBookings(historyStart, _today, s, w, i, j, l, l, m);
                                    //            foreach (var booking in bookings)
                                    //                if (!booking.IsCancelled)
                                    //                    lp[s][w][i][j][m][l].AddBooking(booking);

                                    //            lp[s][w][i][j][m][l].Calculate(fTemp);
                                    //        }
                                    //        else
                                    //            fTemp = lp[s][w][i][j][m][l].GetForecast();
                                    //        forecasts[l]= fTemp[d]>0?fTemp[d]:0;
                                    //    }
                                    //    var sum = forecasts.Sum();
                                    //    if (sum>0)
                                    //        for (int l = catStayLength[j].LowerBound; l <= catStayLength[j].UpperBound; l++)
                                    //            for (int t = 1; t <= l && d + t < _planningHorizon; t++)
                                    //                R[d + t][i] -= f[d][i, j, m] * forecasts[l] / sum;
                                    //}
                                    p[d][i, j, m] *= inflation;
                                }
                    }
                    //LoadRound(); 
            }
            catch (Exception ex)
            {
                return;
            }
            
        }
        private void LoadRound()
        {
            for (int i = 0, k = 0; i < TotalRoomTypes; ++i)
                for (int j = 0; j < TotalCategories; ++j)
                    for (int m = 0; m < TotalMealTypes; ++m)
                    {
                        double dif = 0;
                        for (int d = 0; d < _planningHorizon; ++d)
                        {
                            f[d][i, j, m] += dif;
                            dif = f[d][i, j, m] - (int)f[d][i, j, m];
                            f[d][i, j, m] = (int)f[d][i, j, m];
                        }
                    }
        }
        private void RecalculatePrices()
        {
            try
            {
                double[] inflations = new double[_planningHorizon];
                List<ProblemInput> problems = new List<ProblemInput>();
                ProblemOutput[] finalResults = new ProblemOutput[_planningHorizon];
                for (int d = 0; d < _planningHorizon; ++d)
                {
                    finalResults[d] = new ProblemOutput
                    {
                        HasSolution = false,
                        Value = double.NaN
                    };
                    inflations[d] = _dataProvider.GetInflationCoef(_today, _today.AddDays(d));
                    for (int i = 0; i < R[d].Length; ++i)
                        if (R[d][i] < 0) R[d][i] = 0; //throw new Exception(); дaбавіць ў лог
                    
                    var problem = GetProblemForW(d, inflations[d]);
                    problems.Add(problem);
                    finalResults[d].SkipedIndexes = problem.SkipedIndexes;
                }
                //Solve to find W coef
                var resultsForW = _problemSolver.Solve(problems.ToArray());
                problems.Clear();
                resultsForW.Where(v=>v.HasSolution).ToList().ForEach(v=>problems.Add(GetProblem(v.D,resultsForW[v.D].Value,true)));
                //Solve to find results
                var results = _problemSolver.Solve(problems.ToArray());
                problems.Clear();
                foreach (var item in resultsForW)
                { 
                    var d= item.D;
                    var result = results.Where(v=>v.D==d).FirstOrDefault();
                    if(item.HasSolution)
                        finalResults[d] = result;
                    if (item.HasSolution && !result.HasSolution)
                        problems.Add(GetProblem(d, resultsForW[d].Value, false));                    
                }
                //trying to solve with improved a[]
                if (problems.Any())
                {
                    results = _problemSolver.Solve(problems.ToArray());
                    problems.Clear();
                    foreach (var item in results.Where(v => v.HasSolution).ToList())
                    {
                        var d = item.D;
                        var s = futureSeasons[d];
                        var w = futureWeekdays[d];
                        var x = item.Solution;
                        

                        for (int i = 0, k = 0; i < TotalRoomTypes; ++i)
                            for (int j = 0; j < TotalCategories; ++j)
                                for (int m = 0; m < TotalMealTypes; ++m)
                                {
                                    if (item.SkipedIndexes==null||!item.SkipedIndexes.Any(t => t.i == i && t.j == j))
                                    {
                                        if (-x[k] * b[s][w][i, j, m] > a[d][i, j, m])
                                            a[d][i, j, m] = f[d][i, j, m] - x[k] * b[s][w][i, j, m];                                        
                                        ++k;
                                    }
                                }
                        problems.Add(GetProblemForW(d, inflations[d]));
                    }
                    resultsForW = _problemSolver.Solve(problems.ToArray());
                    problems.Clear();

                    resultsForW.Where(v => v.HasSolution).ToList().ForEach(v => problems.Add(GetProblem(v.D, v.Value, true)));
                    
                    results = _problemSolver.Solve(problems.ToArray());
                    results.Where(c => c.HasSolution).ToList().ForEach(v => finalResults[v.D] = v);
                }
                for (int d = 0; d < _planningHorizon; ++d)
                {
                    if (finalResults[d].SkipedIndexes == null && !finalResults[d].HasSolution)
                        break;
                    var s = futureSeasons[d];
                    var w = futureWeekdays[d];
                    var solution = finalResults[d].Solution;
                    for (int i = 0, k = 0; i < TotalRoomTypes; ++i)
                        for (int j = 0; j < TotalCategories; ++j)
                            for (int m = 0; m < TotalMealTypes; ++m)
                            {
                                if (finalResults[d].SkipedIndexes==null||!finalResults[d].SkipedIndexes.Any(t => t.i == i && t.j == j))
                                {
                                    if (finalResults[d].HasSolution)
                                    {
                                        p[d][i, j, m] = solution[k];
                                        y[d][i, j, m] = solution[k + solution.Length / 2];
                                        f[d][i, j, m] = a[d][i, j, m] + solution[k] * b[s][w][i, j, m] * inflations[d];
                                        ++k;
                                    }
                                }
                                else if (a[d][i, j, m] <= 0 || R[d][i] < 1)
                                {
                                    f[d][i, j, m] = 0;
                                    p[d][i, j, m] = 0;
                                }
                                else if ((-a[d][i, j, m] / b[s][w][i, j, m]) <= L[s][w][i, j, m])
                                    p[d][i, j, m] = LFinal[d][i, j, m];
                                else
                                {
                                    f[d][i, j, m] = 0;
                                    p[d][i, j, m] = UFinal[d][i, j, m];
                                }
                                
                                //if (a[d][i, j] <= 0) - no data 
                                //if( R[d][i]<1) - no vacant rooms
                                //(-a[d][i, j] / b[s][w][i, j]) <= L[s][w][i, j]* inflation- demand is too low

                                p[d][i, j, m] *= inflations[d];
                            }
                }
                //LoadRound();
            }
            catch (Exception ex)
            {
                return;
            }            
        }

        public int TotalMealTypes { get; set; }

        public int TotalCategories { get; set; }

        public int TotalRoomTypes { get; set; }

        public double GetPrice(DateTime date, int roomType, int categoryType, int mealType, int peopleNum)
        {
            var day = GetDayNumber(date);
            var roomTypeId = _defoConfiguration.RoomTypes.RoomTypeDescriptions[roomType].Number;
            var mealTypeId = _defoConfiguration.MealTypes.MealTypeDescriptions[mealType].Number;
            var coef = _defoConfiguration.RoomTypes.RoomTypeCoefs.FirstOrDefault(c => c.Number == roomTypeId && c.PeopleNum == peopleNum);
            var price = p[day][roomType, categoryType, mealType];
            double mealPrice = 0;
            var meal = _defoConfiguration.MealTypes.MealTypeDescriptions.FirstOrDefault(c => c.Number == mealTypeId);
            if (meal != null)
                mealPrice = meal.MarketBasePrice * peopleNum;
            double histPrice = 0;
            if (price <= 0)
            {
                var w = futureWeekdays[day];
                var s = futureSeasons[day];
                var bookings = _dataProvider.GetBookings(_today.AddDays(-180), _today, s, w, roomType, categoryType, mealType);
                var booking = bookings.OrderBy(c => c.CheckIn).LastOrDefault(p => p.Status == BookingStatus.CheckIn);
                if (booking != null && booking.PricePerNight > 0)
                {
                    histPrice = booking.PricePerNight + mealPrice;
                }
                else
                {
                    var p = ApplyParserData(date, roomType, categoryType, mealType, 0);
                    if (!_defoConfiguration.MealTypes.UseInDynamicCalculation)
                        if (coef != null && coef.Coef != 0)
                            p = (p - mealPrice)*coef.Coef + mealPrice;
                    return p;
                }
            }

            if (_defoConfiguration.MealTypes.UseInDynamicCalculation )
            {
                return ApplyParserData(date, roomType, categoryType, mealType, price>0?price: histPrice);
            }
            else
            {
                var pm = price>0?(price + mealPrice): histPrice;
                price = ApplyParserData(date, roomType, categoryType, mealType, pm);
                if (coef != null && coef.Coef != 0)
                {
                    var m1 = mealPrice * price / pm;
                    price = (price - m1) * coef.Coef + m1;
                }
                return price;
            }
        }
        public double ApplyParserData(DateTime date, int roomType, int categoryType, int mealType, double price)
        {
            var total = _defoConfiguration.Categories.BookingPeriods.Any() ? _defoConfiguration.Categories.BookingPeriods.Count() : 1;
            int l = (categoryType / total);
            int b = categoryType - l * total;
            var bookingPeriod = _defoConfiguration.Categories.BookingPeriods.FirstOrDefault(c => c.Number == b);
            var bookingPeriodNum = (date.Date - _today.Date).TotalDays;
            if (bookingPeriod == null || !(bookingPeriodNum <= bookingPeriod.UpperBound && bookingPeriodNum >= bookingPeriod.LowerBound))
                return price;
            var stayPeriod = _defoConfiguration.Categories.StayPeriods.FirstOrDefault(c => c.Number == l);
            if (stayPeriod == null)
                return price;
            var parserData = _dataProvider.GetParserRoomData(date, roomType, mealType, stayPeriod.LowerBound < 1 ? 1 : stayPeriod.LowerBound, stayPeriod.UpperBound < 1 ? 1 : stayPeriod.UpperBound);
            if (parserData==null||!parserData.Any())
                return price;
            var prices = new List<InternalParserRoomData>();
            foreach (var item in parserData.GroupBy(c => c.HotelName).ToList())
            {
                var thisDayData = item.FirstOrDefault(c => c.Date.Date == date.Date);
                if (thisDayData == null)
                    continue;
                if (stayPeriod.UpperBound > 1)
                {
                    var longStay = item.FirstOrDefault(c => c.StayLength > 1);
                    if (longStay == null)
                        continue;
                    var sumPrice = item.Where(c => c.StayLength == 1 && (c.Date.Date - date.Date).TotalDays < (longStay.StayLength))?.Sum(c => c.Price);
                    if (!sumPrice.HasValue)
                        continue;
                    thisDayData.Price *= longStay.Price / sumPrice.Value;
                }
                prices.Add(new InternalParserRoomData() { Price = thisDayData.Price, Rating = thisDayData.Rating, AverageOccupancy = thisDayData.AverageOccupancy });
            }
            if (!prices.Any())
                return price;
            prices = prices.OrderBy(c => c.Price).ToList();
            var pMin = prices.First();
            var pMax = prices.Last();
            var pMed = prices[prices.Count() / 2];
            var occupancy = pMed.AverageOccupancy / 100.0;
            var occupancyConst = 37;
            var occupancyConst2 = 77;
            if (price == 0)
            {
                var price1 = pMin.Price + (pMed.Price - pMin.Price) * occupancy;
                var price2 = pMed.Price + (pMax.Price - pMed.Price) * occupancy;
                if (occupancy < occupancyConst)
                {
                    if (_rating < pMin.Rating)
                        return pMin.Price;
                    else if(_rating < pMed.Rating)
                        return price1; 
                    else if (_rating < pMax.Rating)
                        return pMed.Price;
                    else
                        return price2;
                }
                else if (occupancy >= occupancyConst && occupancy <= occupancyConst2)
                {
                    if (_rating < pMin.Rating)
                        return pMin.Price;
                    else if (_rating < pMed.Rating)
                        return price1;
                    else if (_rating < pMax.Rating)
                        return pMax.Price - (pMax.Price - pMed.Price) * occupancy;
                    else
                        return price2;
                }
                else
                {
                    if (_rating < pMin.Rating)
                        return pMed.Price - (pMed.Price - pMin.Price) * occupancy;
                    else if (_rating < pMed.Rating)
                        return pMed.Price;
                    else if (_rating < pMax.Rating)
                        return price2;
                    else
                        return pMax.Price;
                }
            }
            else
            {
                if (price <= pMin.Price)
                {
                    if (occupancy < occupancyConst)
                    {
                        if (_rating < pMin.Rating)
                            return price;
                        else
                            return pMin.Price - (pMin.Price - price) * occupancy;
                    }
                    else if (occupancy >= occupancyConst && occupancy <= occupancyConst2)
                    {
                        if (_rating < pMin.Rating)
                            return pMin.Price;
                        else
                            return pMed.Price - (pMed.Price - pMin.Price) * occupancy;
                    }
                    else
                    {
                        if (_rating < pMed.Rating)
                            return pMed.Price + (pMed.Price - pMin.Price) * occupancy;
                        else
                            return pMed.Price;
                    }
                }
                else if (price <= pMed.Price)
                {
                    if (occupancy < occupancyConst)
                    {
                        if (_rating < pMed.Rating)
                            return price;
                        else
                            return price + (pMed.Price - price) * occupancy;
                    }
                    else if (occupancy >= occupancyConst && occupancy <= occupancyConst2)
                    {
                        if (_rating < pMed.Rating)
                            return price + (pMed.Price - price) * occupancy;
                        else
                            return pMed.Price;
                    }
                    else
                    {
                        if (_rating < pMed.Rating)
                            return pMed.Price;
                        else
                            return pMed.Price + (pMax.Price - pMed.Price) * occupancy;
                    }
                }
                else if (price <= pMed.Price)
                {
                    if (occupancy < occupancyConst)
                    {
                        if (_rating < pMed.Rating)
                            return pMed.Price - (pMed.Price - pMin.Price) * occupancy;
                        else
                            return pMed.Price;
                    }
                    else if (occupancy >= occupancyConst && occupancy <= occupancyConst2)
                    {
                        if (_rating < pMed.Rating)
                            return pMed.Price;
                        else
                            return price;
                    }
                    else
                    {
                        if (_rating < pMed.Rating)
                            return price + (pMax.Price - price) * occupancy;
                        else
                            return pMax.Price;
                    }
                }
                else
                {
                    if (occupancy < occupancyConst)
                    {
                        if (_rating < pMed.Rating)
                            return pMed.Price;
                        else
                            return pMed.Price + (pMax.Price - pMed.Price) * occupancy;
                    }
                    else if (occupancy >= occupancyConst && occupancy <= occupancyConst2)
                    {
                        if (_rating < pMax.Rating)
                            return pMed.Price + (pMax.Price - pMed.Price) * occupancy;
                        else
                            return pMax.Price;
                    }
                    else
                    {
                        if (_rating < pMax.Rating)
                            return price - (price - pMax.Price) * occupancy;
                        else
                            return price;
                    }
                }
            }
        }
        public double GetOperationalCost(int roomType, int mealType)
        {
            return h[roomType, mealType];
        }
        public double GetPriceCushion(DateTime date, int roomType, int categoryType, int mealType)
        {
            return y[GetDayNumber(date)][roomType, categoryType, mealType];
        }
        public double GetCancelation(DateTime date, int roomType, int categoryType, int mealType)
        {
            return cancelation[GetDayNumber(date)][roomType, categoryType, mealType];
        }
        public double GetNoShows(DateTime date, int roomType, int categoryType, int mealType)
        {
            return noShows[GetDayNumber(date)][roomType, categoryType, mealType];
        }
        public double GetExpectedLoad(DateTime date, int roomType, int categoryType, int mealType)
        {
            var day = GetDayNumber(date);
            //var weekDay = futureWeekdays[day];
            //var season = futureSeasons[day];

            //var expectedLoad = a[day][roomType, categoryType, mealType] + b[season][weekDay][roomType, categoryType, mealType] * p[day][roomType, categoryType, mealType];
            var expectedLoad = f[day][roomType, categoryType, mealType];
            return expectedLoad <= 0.1 ? 0 : Math.Round(expectedLoad,2);
        }

        private int GetDayNumber(DateTime date)
        {
            return (date.Date - _today.Date).Days;
        }
    }
}