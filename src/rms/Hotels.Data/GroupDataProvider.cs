using Hotels.Basic;
using Hotels.Config;
using Hotels.Config.ConfigModel;
using Hotels.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels.Data
{
    public class GroupDataProvider:IGroupDataProvider
    {
        IEnumerable<InternalOrder> orders;
        IEnumerable<InternalOrder> groupOrders;
        IEnumerable<InternalCalculation> calculations;
        private readonly IDefoConfiguration _config;
        public GroupDataProvider(IEnumerable<InternalOrder> orders, IEnumerable<InternalOrder> groupOrders, IEnumerable<InternalCalculation> calculations, IDefoConfiguration config)
        {
            _config = config;
            this.orders = BuildOrders(orders);
            this.groupOrders = BuildOrders(groupOrders);
            this.calculations = calculations;
            
        }

        public IEnumerable<Booking> GetPlanBookings(DateTime date, int roomType)
        {
            return orders.Where(c => c.CheckIn.Date.Equals(date) && c.RoomType == roomType&& c.Status == BookingStatus.CheckIn)
                .Select(x => new Booking
            {
                CheckIn = x.CheckIn,                
                Type= x.Type,
                Status = x.Status,
                LengthOfStay = 1,
                OrderTime = x.BookingTime,
                RoomPrice = x.RoomPrice,
                MealPrice= x.MealPrice,
                    PricePerNight = x.RoomPrice + (_config.ConfigurationRoot.MealTypes.UseInDynamicCalculation ? x.MealPrice : 0)
                });
        }
        public InternalCalculation GetPredictions(DateTime date)
        {
            return calculations.Where(c => c!=null&&c.PredictionDate.Date == date.Date).FirstOrDefault();
        }
        public IEnumerable<Booking> GetGroupBookings( int seasonType, int roomType)
        {
            return groupOrders.Where(c => c.SeasonType == seasonType && c.RoomType == roomType)
                 .Select(x => new Booking
                 {
                     CheckIn = x.CheckIn,
                     Type = x.Type,
                     Status = x.Status,
                     LengthOfStay = 1,
                     OrderTime = x.BookingTime,
                     RoomPrice = x.RoomPrice,
                     MealPrice = x.MealPrice,
                     PricePerNight = x.RoomPrice + (_config.ConfigurationRoot.MealTypes.UseInDynamicCalculation ? x.MealPrice : 0)
                 }); 
        }
        private List<InternalOrder> BuildOrders(IEnumerable<InternalOrder> temp)
        {
            var result = new List<InternalOrder>();
            foreach (InternalOrder item in temp)
            {
                for (int i = 0; i < item.LengthOfStay; i++)
                {
                    var order = new InternalOrder
                    {
                        BookingTime = item.BookingTime,
                        CheckIn = item.CheckIn.AddDays(i),
                        Type= item.Type,
                        LengthOfStay = item.LengthOfStay,
                        RoomPrice = item.RoomPrice,
                        MealPrice = item.MealPrice,
                        RoomType = item.RoomType,
                        CategoryInfo = item.CategoryInfo,
                        Status = item.Status,
                        MealType = item.MealType
                    };
                    BuildOrder(order);
                    result.Add(order);
                }
            }

            return result;
        }
        private void BuildOrder(InternalOrder order)
        {
            order.SeasonType = 0;
            int w = order.CheckIn.GetWeekNumber();
            foreach (SeasonDescription x in _config.ConfigurationRoot.Seasons.SeasonDescriptions)
                if (w >= x.Start && w <= x.Finish)
                {
                    order.SeasonType = x.Number;
                    break;
                }
            //order.WeekdayType = 0;
            //switch (order.CheckIn.DayOfWeek)
            //{
            //    case DayOfWeek.Sunday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Sunday; break;
            //    case DayOfWeek.Monday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Monday; break;
            //    case DayOfWeek.Tuesday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Tuesday; break;
            //    case DayOfWeek.Wednesday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Wednesday; break;
            //    case DayOfWeek.Thursday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Thursday; break;
            //    case DayOfWeek.Friday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Friday; break;
            //    case DayOfWeek.Saturday: order.WeekdayType = _config.ConfigurationRoot.Weekdays.Saturday; break;
            //}
            //if (withCategory)
            //{
            //    order.CategoryInfo = 0;
            //    foreach (OptionDescription x in _config.ConfigurationRoot.Categories.StayPeriods)
            //        if (order.LengthOfStay >= x.LowerBound && order.LengthOfStay <= x.UpperBound)
            //        {
            //            order.CategoryInfo = x.Number;
            //            break;
            //        }
            //    var total = _config.ConfigurationRoot.Categories.BookingPeriods.Count();
            //    if (total > 0)
            //        order.CategoryInfo *= total;
            //    foreach (OptionDescription x in _config.ConfigurationRoot.Categories.BookingPeriods)
            //        if ((order.CheckIn.Date - order.BookingTime.Date).Days >= x.LowerBound && (order.CheckIn.Date - order.BookingTime.Date).Days <= x.UpperBound)
            //        {
            //            order.CategoryInfo += x.Number;
            //            break;
            //        }
            //}
        }
    }
}
