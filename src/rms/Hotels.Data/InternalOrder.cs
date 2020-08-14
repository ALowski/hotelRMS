using System;
using Hotels.Basic;

namespace Hotels.Data
{
    public class InternalOrder
    {
        public DateTime BookingTime;
        public DateTime CheckIn;
        public int LengthOfStay;
        public int PeopleNum { get; set; }
        public double RoomPrice;
        public double MealPrice;
        public int RoomType;
        public int MealType;
        public int SeasonType;
        public int WeekdayType;
        public int CategoryInfo;
        public BookingStatus Status;
        public BookingType Type;

        //public void SetParams(bool withCategory) 
        //{
        //    SeasonType = 0;
        //    int w = CheckIn.GetWeekNumber();
        //    foreach (SeasonDescription x in _config.ConfigurationRoot.Seasons.SeasonDescriptions)
        //        if (w >= x.Start && w <= x.Finish)
        //        {
        //            SeasonType = x.Number;
        //            break;
        //        }
        //    WeekdayType = 0;
        //    switch (CheckIn.DayOfWeek)
        //    {
        //        case DayOfWeek.Sunday: WeekdayType = _config.ConfigurationRoot.Weekdays.Sunday; break;
        //        case DayOfWeek.Monday: WeekdayType = _config.ConfigurationRoot.Weekdays.Monday; break;
        //        case DayOfWeek.Tuesday: WeekdayType = _config.ConfigurationRoot.Weekdays.Tuesday; break;
        //        case DayOfWeek.Wednesday: WeekdayType = _config.ConfigurationRoot.Weekdays.Wednesday; break;
        //        case DayOfWeek.Thursday: WeekdayType = _config.ConfigurationRoot.Weekdays.Thursday; break;
        //        case DayOfWeek.Friday: WeekdayType = _config.ConfigurationRoot.Weekdays.Friday; break;
        //        case DayOfWeek.Saturday: WeekdayType = _config.ConfigurationRoot.Weekdays.Saturday; break;
        //    }
        //    if (withCategory)
        //    {
        //        CategoryInfo = 0;
        //        foreach (OptionDescription x in _config.ConfigurationRoot.Categories.StayPeriods)
        //            if (LengthOfStay >= x.LowerBound && LengthOfStay <= x.UpperBound)
        //            {
        //                CategoryInfo = x.Number;
        //                break;
        //            }
        //        var total = _config.ConfigurationRoot.Categories.BookingPeriods.Count();
        //        if (total > 0)
        //            CategoryInfo *= total;
        //        foreach (OptionDescription x in _config.ConfigurationRoot.Categories.BookingPeriods)
        //            if ((CheckIn.Date - BookingTime.Date).Days >= x.LowerBound && (CheckIn.Date - BookingTime.Date).Days <= x.UpperBound)
        //            {
        //                CategoryInfo += x.Number;
        //                break;
        //            }
        //    }
        //}

        //internal int SeasonType
        //{
        //    get
        //    {
        //        int w = CheckIn.GetWeekNumber();
        //        foreach (SeasonDescription x in _config.ConfigurationRoot.Seasons.SeasonDescriptions)
        //            if (w >= x.Start && w <= x.Finish)
        //                return x.Number;
        //        return 0;
        //    }
        //}

        //internal int WeekdayType
        //{
        //    get
        //    {
        //        switch (CheckIn.DayOfWeek)
        //        {
        //            case DayOfWeek.Sunday: return _config.ConfigurationRoot.Weekdays.Sunday;
        //            case DayOfWeek.Monday: return _config.ConfigurationRoot.Weekdays.Monday;
        //            case DayOfWeek.Tuesday: return _config.ConfigurationRoot.Weekdays.Tuesday;
        //            case DayOfWeek.Wednesday: return _config.ConfigurationRoot.Weekdays.Wednesday;
        //            case DayOfWeek.Thursday: return _config.ConfigurationRoot.Weekdays.Thursday;
        //            case DayOfWeek.Friday: return _config.ConfigurationRoot.Weekdays.Friday;
        //            case DayOfWeek.Saturday: return _config.ConfigurationRoot.Weekdays.Saturday;
        //            default: return 0;
        //        }
        //    }
        //}

        //internal int CategoryInfo
        //{
        //    get
        //    {
        //        int res = 0;
        //        foreach (OptionDescription x in _config.ConfigurationRoot.Categories.StayPeriods)
        //            if (LengthOfStay >= x.LowerBound && LengthOfStay <= x.UpperBound)
        //            {
        //                res = x.Number;
        //                break;
        //            }
        //        var total = _config.ConfigurationRoot.Categories.BookingPeriods.Count();
        //        if (total > 0)
        //            res *= total;
        //        foreach (OptionDescription x in _config.ConfigurationRoot.Categories.BookingPeriods)
        //            if ((CheckIn.Date - BookingTime.Date).Days >= x.LowerBound && (CheckIn.Date - BookingTime.Date).Days <= x.UpperBound)
        //            {
        //                res += x.Number;
        //                break;
        //            }
        //        return res;
        //    }
        //}
    }
}
