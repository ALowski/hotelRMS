using System;
using System.Collections.Generic;
using System.Linq;
using Hotels.Basic;
using Hotels.Config;
using Hotels.Config.ConfigModel;
using Hotels.Core.Tools;

namespace Hotels.Data
{
    public class DummyDataProvider : IDataProvider
    {
        private readonly IDefoConfiguration _config;
        private readonly List<InternalOrder> _orders;
        private readonly IEnumerable<InternalParserRoomData> _parserRoomData;
        private readonly List<InflationCoef> _inflationCoefs;
        private readonly IEnumerable<InternalEvent> _events;
        private readonly Dictionary<Tuple<int, int,int,int>, IEnumerable<InternalOrder>> _filteredOrders = new Dictionary<Tuple<int,int, int,int>, IEnumerable<InternalOrder>>();

        public DummyDataProvider(DateTime start, IEnumerable<InflationCoef> inflationCoefs, IEnumerable<InternalOrder> orders, IDefoConfiguration config, IEnumerable<InternalEvent> events = null, IEnumerable<InternalParserRoomData> parserRoomData = null)
        {
            _parserRoomData = parserRoomData;
            this._config = config;
            this._events = events;
            DateTime date = new DateTime(start.Year, start.Month, 1);
            this._inflationCoefs = inflationCoefs.Where(c => c.Date < date).OrderBy(c => c.Date).ToList();
            CalculatePlanedInflation(date);

            foreach (InternalOrder internalOrder in orders)
            {
                this.BuildOrder(internalOrder, true);
            }
            this._orders = this.BuildOrders(start, orders);
            this.BuildFilteredOrders();
        }

        public DummyDataProvider(DateTime start, string ordersPath, string inflationPath, IDefoConfiguration config)
        {
            _config = config;
            DateTime date = new DateTime(start.Year, start.Month, 1);
            _inflationCoefs = ParseInflations(inflationPath).Where(c => c.Date < date).OrderBy(c => c.Date).ToList();
            CalculatePlanedInflation(date);

            this._orders = this.BuildOrders(start, ParseOrders(ordersPath).ToList());
            this.BuildFilteredOrders();
        }
        public IEnumerable<Booking> GetGroupBookingsForPlan(DateTime end, DateTime today, int roomType, int seasonType)
        {
            IEnumerable<InternalOrder> filter = _orders.Where(x =>
                x.SeasonType == seasonType
                && x.Type==BookingType.Group
                && x.RoomType == roomType
                && x.BookingTime <= today
                && x.CheckIn <= end
                && x.CheckIn >= today);
            var result = (filter.OrderByDescending(x => x.CheckIn))
               .Select(x => new Booking
               {
                   CheckIn = x.CheckIn,
                   Status = x.Status,
                   LengthOfStay = 1,
                   Type = x.Type,
                   OrderTime = x.BookingTime,
                   RoomPrice = x.RoomPrice,
                   MealPrice= x.MealPrice,
                   PricePerNight = x.RoomPrice + (_config.ConfigurationRoot.MealTypes.UseInDynamicCalculation ? x.MealPrice : 0)
               });
            return result;
        }
        public IEnumerable<Booking> GetGroupBookings(DateTime start, DateTime today, int seasonType, int roomType)
        {
            return _orders.Where(x =>
                x.SeasonType == seasonType 
                && x.RoomType == roomType
                && x.Type == BookingType.Group
                && x.CheckIn >= start
                && x.CheckIn < today).OrderByDescending(x => x.CheckIn)
                .Select(x => new Booking
                {
                    CheckIn = x.CheckIn,
                    Status = x.Status,
                    Type = x.Type,
                    LengthOfStay = 1,
                    OrderTime = x.BookingTime,
                    RoomPrice = x.RoomPrice,
                    MealPrice = x.MealPrice,
                    PricePerNight = x.RoomPrice + (_config.ConfigurationRoot.MealTypes.UseInDynamicCalculation ? x.MealPrice : 0)
                }).ToList();
        }
        public IEnumerable<Booking> GetBookings(DateTime start, DateTime today, int seasonType, int weekdayType, int roomType, int categoryInfo, int stayStart, int stayEnd, int mealType)
        {
            List<InternalOrder> filter =new List<InternalOrder>();
            var key = new Tuple<int, int, int, int>(seasonType, weekdayType, roomType, categoryInfo);
            if (_filteredOrders.ContainsKey(key))
                filter = _filteredOrders[key].ToList();
            else
                filter = _orders.Where(x => x.Type == BookingType.Usual&& x.SeasonType == seasonType && x.WeekdayType == weekdayType && x.RoomType == roomType && x.CategoryInfo == categoryInfo).ToList();
            filter = filter.Where(x =>
                x.MealType == mealType
                && x.LengthOfStay>=stayStart
                && x.LengthOfStay<=stayEnd
                && x.CheckIn >= start
                && x.CheckIn < today).ToList();
            return  filter.OrderByDescending(x => x.CheckIn)
                .Select(x => new Booking
                {
                    CheckIn = x.CheckIn,
                    Status = x.Status,
                    Type = x.Type,
                    LengthOfStay = 1,
                    OrderTime = x.BookingTime,
                    RoomPrice = x.RoomPrice,
                    MealPrice = x.MealPrice,
                    PricePerNight = x.RoomPrice + (_config.ConfigurationRoot.MealTypes.UseInDynamicCalculation ? x.MealPrice : 0)
                }).ToList();
        }
        public IEnumerable<InternalEvent> GetEvents(DateTime date)
        {
            return _events?.Where(c => c.Start <= date && c.End >= date).ToList();
        }
        public IEnumerable<InternalParserRoomData> GetParserRoomData(DateTime date, int roomType, int mealType, int LStayStart, int LStayEnd)
        {
            return _parserRoomData?.Where(c => ((LStayStart <= c.StayLength && LStayEnd >= c.StayLength&& c.Date.Date == date.Date) ||(c.StayLength==1 && (c.Date.Date - date.Date).TotalDays< (LStayEnd)))&& c.RoomType== roomType &&c.MealType== mealType ).ToList();
        }
        public IEnumerable<Booking> GetBookings(DateTime start, DateTime today, int seasonType, int weekdayType, int roomType, int categoryInfo, int mealType)
        {
            List<InternalOrder> filter = new List<InternalOrder>();
            var key = new Tuple<int, int, int, int>(seasonType, weekdayType, roomType, categoryInfo);
            if (_filteredOrders.ContainsKey(key))
                filter = _filteredOrders[key].ToList();
            else
                filter = _orders.Where(x => x.Type == BookingType.Usual&& x.SeasonType == seasonType && x.WeekdayType == weekdayType && x.RoomType == roomType && x.CategoryInfo == categoryInfo).ToList();
            filter = filter.Where(x =>
                x.MealType == mealType
                && x.CheckIn >= start
                && x.CheckIn < today).ToList();
            return filter.OrderByDescending(x => x.CheckIn)
                .Select(x => new Booking
                {
                    CheckIn = x.CheckIn,
                    Status = x.Status,
                    Type = x.Type,
                    LengthOfStay = 1,
                    OrderTime = x.BookingTime,
                    RoomPrice = x.RoomPrice,
                    MealPrice = x.MealPrice,
                    PricePerNight = x.RoomPrice + (_config.ConfigurationRoot.MealTypes.UseInDynamicCalculation ? x.MealPrice : 0)
                }).ToList();
        }
        public IEnumerable<Booking> GetBookingsForPlan(DateTime end, DateTime today, int roomType, int seasonType, int weekdayType, int categoryInfo, int stayStart, int stayEnd, int mealType)
        {
            IEnumerable<InternalOrder> filter = _orders.Where(x =>
                    x.SeasonType == seasonType
                    && x.Type == BookingType.Usual
                    && x.LengthOfStay >= stayStart
                    && x.LengthOfStay <= stayEnd
                    && x.CategoryInfo == categoryInfo
                    && x.MealType == mealType
                    && x.WeekdayType == weekdayType
                    && x.RoomType == roomType
                    && x.BookingTime <= today
                    && x.CheckIn <= end
                    && x.CheckIn >= today);
            var result = (filter.OrderByDescending(x => x.CheckIn))
               .Select(x => new Booking
               {
                   CheckIn = x.CheckIn,
                   Status = x.Status,
                   LengthOfStay = 1,
                   Type = x.Type,
                   OrderTime = x.BookingTime,
                   RoomPrice = x.RoomPrice,
                   MealPrice = x.MealPrice,
                   PricePerNight = x.RoomPrice + (_config.ConfigurationRoot.MealTypes.UseInDynamicCalculation ? x.MealPrice : 0)
               });
            return result;
        }
        public IEnumerable<Booking> GetBookingsForPlan(DateTime end, DateTime today, int roomType, int seasonType, int weekdayType, int categoryInfo, int mealType)
        {
            IEnumerable<InternalOrder> filter = _orders.Where(x =>
                x.SeasonType == seasonType
                && x.Type == BookingType.Usual
                && x.CategoryInfo == categoryInfo
                && x.MealType == mealType
                && x.WeekdayType == weekdayType
                && x.RoomType == roomType
                && x.BookingTime <= today
                && x.CheckIn <= end
                && x.CheckIn >= today);
             var result=(filter.OrderByDescending(x => x.CheckIn) )
                .Select(x => new Booking
                {
                    CheckIn = x.CheckIn,
                    Status = x.Status,
                    Type = x.Type,
                    LengthOfStay = 1,
                    OrderTime = x.BookingTime,
                    RoomPrice = x.RoomPrice,
                    MealPrice = x.MealPrice,
                    PricePerNight = x.RoomPrice + (_config.ConfigurationRoot.MealTypes.UseInDynamicCalculation ? x.MealPrice : 0)
                });
             return result;
        }

        public int GetBookingsCount(DateTime start, DateTime today, int seasonType, int weekdayType, int roomType, int categoryInfo)
        {
            return _orders.Count(x => x.SeasonType == seasonType
                                && x.WeekdayType == weekdayType
                                && x.RoomType == roomType
                                && x.CategoryInfo == categoryInfo
                                && x.CheckIn >= start
                                && x.CheckIn < today);
        }

        public double GetInflationCoef(DateTime start, DateTime end)
        {
            double coef = 1;
            _inflationCoefs.Where(c => c.Date > start && c.Date <= end.AddMonths(-1)).ToList().ForEach(c => coef *= c.Coef/100);
            var firstMonthCoef = _inflationCoefs.FirstOrDefault(c => c.Date.Month == start.Month && start.Year == start.Year);
            if(firstMonthCoef!=null)
                coef*=(1+(firstMonthCoef.Coef/100-1)*(DateTime.DaysInMonth(start.Year, start.Month)-start.Day)/DateTime.DaysInMonth(start.Year, start.Month));
            var lastMonthCoef = _inflationCoefs.FirstOrDefault(c => c.Date.Month == end.Month && end.Year == start.Year);
            if (lastMonthCoef != null)
                coef *= (1 + (lastMonthCoef.Coef / 100 - 1) * start.Day / DateTime.DaysInMonth(start.Year, start.Month));
            return coef;   
        }
        
        private void BuildFilteredOrders()
        {
            foreach (var x in _config.ConfigurationRoot.Seasons.SeasonDescriptions.Select(c => c.Number).Distinct())
            {
                var seasonFilter = _orders.Where(v => v.Type == BookingType.Usual && v.SeasonType == x).ToList();
                for (int i = 0; i < _config.ConfigurationRoot.Weekdays.Total; i++)
                {
                    var weekFilter = seasonFilter.Where(v => v.WeekdayType == i).ToList();
                    foreach (var y in _config.ConfigurationRoot.RoomTypes.RoomTypeDescriptions.Select(c => c.Number).Distinct())
                    {
                        var roomFilter = weekFilter.Where(v => v.RoomType == y).ToList();
                        for (int j = 0; j < _config.ConfigurationRoot.Categories.Total; j++)
                            _filteredOrders.Add(new Tuple<int, int, int, int>(x, i, y, j), roomFilter.Where(v => v.CategoryInfo == j).ToList());
                    }
                }
            }
        }

        private List<InternalOrder> BuildOrders(DateTime start, IEnumerable<InternalOrder> temp)
        {
            var result = new List<InternalOrder>();
            Dictionary<DateTime, double> coefs = new Dictionary<DateTime, double>();

            foreach (InternalOrder item in temp)
            {
                double coef = 0;
                if (coefs.ContainsKey(item.CheckIn.Date))
                    coef = coefs[item.CheckIn.Date];
                else
                {
                    coef = GetInflationCoef(item.CheckIn, start);
                    coefs.Add(item.CheckIn.Date, coef);
                }
                var roomPrice = item.RoomPrice * coef;
                var mealPrice = item.MealPrice * coef;
                int? peopleNum = _config?.ConfigurationRoot?.RoomTypes?.RoomTypeDescriptions?.FirstOrDefault(c => c.Number == item.RoomType)?.PeopleNum;

                if (peopleNum.HasValue && item.PeopleNum != peopleNum)
                {
                    var peopleCoef=_config.ConfigurationRoot.RoomTypes.RoomTypeCoefs?.FirstOrDefault(c => c.Number == item.RoomType && c.PeopleNum == item.PeopleNum)?.Coef;
                    if(peopleCoef.HasValue&& peopleCoef.Value!= 0)
                        roomPrice /= peopleCoef.Value;
                }
                item.MealPrice = mealPrice;
                item.RoomPrice = roomPrice;
                BuildOrder(item, false);
                result.Add(item);
                //for (int i = 0; i < item.LengthOfStay; i++)
                //{
                //    var order = new InternalOrder
                //    {
                //        BookingTime = item.BookingTime,
                //        CheckIn = item.CheckIn.AddDays(i),
                //        LengthOfStay = item.LengthOfStay,
                //        RoomPrice = roomPrice,
                //        MealPrice = mealPrice,
                //        RoomType = item.RoomType,
                //        CategoryInfo = item.CategoryInfo,
                //        Status = item.Status,
                //        MealType = item.MealType,
                //        Type = item.Type
                //    };
                //    BuildOrder(order, false);
                //    result.Add(order);
                //}
            }

            return result;
        }

        private void BuildOrder(InternalOrder order, bool withCategory)
        {
            order.SeasonType = 0;
            int w = order.CheckIn.GetWeekNumber();
            foreach (SeasonDescription x in _config.ConfigurationRoot.Seasons.SeasonDescriptions)
                if (w >= x.Start && w <= x.Finish)
                {
                    order.SeasonType = x.Number;
                    break;
                }
            order.WeekdayType = 0;
            switch (order.CheckIn.DayOfWeek)
            {
                case DayOfWeek.Sunday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Sunday; break;
                case DayOfWeek.Monday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Monday; break;
                case DayOfWeek.Tuesday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Tuesday; break;
                case DayOfWeek.Wednesday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Wednesday; break;
                case DayOfWeek.Thursday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Thursday; break;
                case DayOfWeek.Friday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Friday; break;
                case DayOfWeek.Saturday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Saturday; break;
            }
            if (withCategory)
            {
                order.CategoryInfo=_config.ConfigurationRoot.GetCategoryId(order.LengthOfStay, (order.CheckIn.Date - order.BookingTime.Date).Days);
                //order.CategoryInfo = 0;
                //foreach (OptionDescription x in _config.ConfigurationRoot.Categories.StayPeriods)
                //    if (order.LengthOfStay >= x.LowerBound && order.LengthOfStay <= x.UpperBound)
                //    {
                //        order.CategoryInfo = x.Number;
                //        break;
                //    }
                //var total = _config.ConfigurationRoot.Categories.BookingPeriods.Count();
                //if (total > 0)
                //    order.CategoryInfo *= total;
                //foreach (OptionDescription x in _config.ConfigurationRoot.Categories.BookingPeriods)
                //    if ((order.CheckIn.Date - order.BookingTime.Date).Days >= x.LowerBound && (order.CheckIn.Date - order.BookingTime.Date).Days <= x.UpperBound)
                //    {
                //        order.CategoryInfo += x.Number;
                //        break;
                //    }
            }
        }

        private void CalculatePlanedInflation(DateTime start)
        {
            //calculate Inflation for planning period with Holts method

            if (_inflationCoefs.Any())
            {
                var array = _inflationCoefs.Where(c => c.Date < start && c.Date >= start.AddMonths(-12)).OrderBy(c => c.Date).Select(c => c.Coef).ToArray();
                if (array.Any())
                {
                    double[] newCoefs = HoltsCalculation.Calculate(12, array);

                    foreach (var coef in newCoefs)
                    {
                        _inflationCoefs.Add(new InflationCoef { Date = start, Coef = coef });
                        start = start.AddMonths(1);
                    }
                }
            }
        }

        private IEnumerable<InternalOrder> ParseOrders(string ordersPath)
        {
            return System.IO.File.ReadAllLines(ordersPath).Select(x =>
            {
                string[] s = x.Split(';');
                var item = new InternalOrder
                {
                    BookingTime = DateTime.Parse(s[0]),
                    CheckIn = DateTime.Parse(s[1]),
                    LengthOfStay = int.Parse(s[2]),
                    RoomPrice = double.Parse(s[3]),
                    RoomType = int.Parse(s[4]),
                    Type = (BookingType) int.Parse(s[5]),
                    Status =(BookingStatus) int.Parse(s[6]),
                    MealType = s.Length<8?0:int.Parse(s[7])
                };
                BuildOrder(item, true);
                return item;
            });
        }

        private static IEnumerable<InflationCoef> ParseInflations(string inflationPath)
        {
            return System.IO.File.ReadAllLines(inflationPath).Select(x =>
            {
                string[] s = x.Split(';');
                return new InflationCoef
                {
                    Date = DateTime.Parse(s[0]),
                    Coef = double.Parse(s[1])
                };
            });
        }
    }
}
