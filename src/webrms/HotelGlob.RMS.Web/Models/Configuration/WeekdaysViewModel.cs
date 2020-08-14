using Hotels.Config.ConfigModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class WeekdaysViewModel
    {
        public List<double> PriceReductions { get; set; }

        public WeekdaysViewModel()
        {
            this.PriceReductions = new List<double> { 1,1,1,1,1,1,1};
        }
        public static WeekdaysViewModel Map(Weekdays weekdays)
        {
            WeekdaysViewModel model = new WeekdaysViewModel();
            if (weekdays != null && weekdays.PriceReductions.Any())
                foreach (var red in weekdays.PriceReductions)
                {
                    if (red.Number == weekdays.Monday)
                        model.PriceReductions[0] = red.Amount;
                    if (red.Number == weekdays.Tuesday)
                        model.PriceReductions[1] = red.Amount;
                    if (red.Number == weekdays.Wednesday)
                        model.PriceReductions[2] = red.Amount;
                    if (red.Number == weekdays.Thursday)
                        model.PriceReductions[3] = red.Amount;
                    if (red.Number == weekdays.Friday)
                        model.PriceReductions[4] = red.Amount;
                    if (red.Number == weekdays.Saturday)
                        model.PriceReductions[5] = red.Amount;
                    if (red.Number == weekdays.Sunday)
                        model.PriceReductions[6] = red.Amount;                  
                }
            return model;
        }
        public static Weekdays Map(WeekdaysViewModel weekdays)
        {
            if (weekdays == null || !weekdays.PriceReductions.Any())
                return null;
            var result = new Weekdays();
            var reductions = new List<PriceReduction>();
            var red = weekdays.PriceReductions.Distinct().ToList();
            for (int i = 0; i < red.Count(); i++) 
                reductions.Add(new PriceReduction { Number = i, Amount = red[i] });
            result.PriceReductions = reductions;
            result.Monday = reductions.FirstOrDefault(c => c.Amount == weekdays.PriceReductions[0]).Number;
            result.Tuesday = reductions.FirstOrDefault(c => c.Amount == weekdays.PriceReductions[1]).Number;
            result.Wednesday = reductions.FirstOrDefault(c => c.Amount == weekdays.PriceReductions[2]).Number;
            result.Thursday = reductions.FirstOrDefault(c => c.Amount == weekdays.PriceReductions[3]).Number;
            result.Friday = reductions.FirstOrDefault(c => c.Amount == weekdays.PriceReductions[4]).Number;
            result.Saturday = reductions.FirstOrDefault(c => c.Amount == weekdays.PriceReductions[5]).Number;
            result.Sunday = reductions.FirstOrDefault(c => c.Amount == weekdays.PriceReductions[6]).Number;
            result.Total = reductions.Count();
            return result;
        }

    }
}