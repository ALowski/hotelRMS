using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Hotels.Config.ConfigModel;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class CategoriesViewModel
    {
        public List<CategoryOptionViewModel> StayPeriods { get; set; }

        public List<CategoryOptionViewModel> BookingPeriods { get; set; }
        public List<CategoryPriceReduction> PriceReductions { get; set; }

        public CategoriesViewModel()
        {
            StayPeriods = new List<CategoryOptionViewModel>();
            BookingPeriods = new List<CategoryOptionViewModel>();
            PriceReductions = new List<CategoryPriceReduction>();
        }

        public static CategoriesViewModel Map(Categories categories)
        {
            var result = new CategoriesViewModel();
            if (categories == null)
                return result;
            if (categories.BookingPeriods != null && categories.BookingPeriods.Any())
                result.BookingPeriods = categories.BookingPeriods.Select(c => new CategoryOptionViewModel { Number=c.Number, LowerBound=c.LowerBound,UpperBound=c.UpperBound}).ToList();
            if (categories.StayPeriods != null && categories.StayPeriods.Any())
                result.StayPeriods = categories.StayPeriods.Select(c => new CategoryOptionViewModel { Number = c.Number, LowerBound = c.LowerBound, UpperBound = c.UpperBound }).ToList();
            if (categories.PriceReductions != null && categories.PriceReductions.Any())
            {
                var total = categories.BookingPeriods.Any() ? categories.BookingPeriods.Count() : 1;
                foreach (var item in categories.PriceReductions)
                {
                    int l = (item.Number / total);
                    int b = item.Number - l * total;
                    result.PriceReductions.Add(new CategoryPriceReduction { BookingPeriodNumber = b, StayPeriodNumber = l, Reduction = item.Amount });                    
                }
            }
            return result;
        }
        public static Categories Map(CategoriesViewModel categories)
        {
            var result = new Categories();
            if (categories == null)
                return result;
            if (categories.BookingPeriods != null && categories.BookingPeriods.Any())
                result.BookingPeriods = categories.BookingPeriods.Select(c => new OptionDescription { Number = c.Number, LowerBound = c.LowerBound, UpperBound = c.UpperBound }).ToList();
            if (categories.StayPeriods != null && categories.StayPeriods.Any())
                result.StayPeriods = categories.StayPeriods.Select(c => new OptionDescription { Number = c.Number, LowerBound = c.LowerBound, UpperBound = c.UpperBound }).ToList();
            var total = categories.BookingPeriods.Any() ? categories.BookingPeriods.Count() : 1;
            if (categories.PriceReductions != null && categories.PriceReductions.Any())
                result.PriceReductions = categories.PriceReductions.Select(c => new PriceReduction { Number = c.StayPeriodNumber * total + c.BookingPeriodNumber, Amount = c.Reduction });
            result.Total = total * (result.StayPeriods.Any()? result.StayPeriods.Count():1);
            return result;
        }
    }
}