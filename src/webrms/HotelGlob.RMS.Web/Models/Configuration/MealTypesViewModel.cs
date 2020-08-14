using HotelGlob.RMS.Data.Models;
using Hotels.Config.ConfigModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class MealTypesViewModel
    {
        [Display(Name = "Use In Dynamic Price Calculation")]
        public bool UseInDynamicCalculation { get; set; }
        public List<MealTypeViewModel> MealTypes { get; set; }
        public List<PriceConstraint> PriceConstraints { get; set; }

        public MealTypesViewModel()
        {
            MealTypes = new List<MealTypeViewModel>();
            PriceConstraints = new List<PriceConstraint>();
        }
        public static MealTypesViewModel Map(MealTypes mealTypes)
        {
            var result = new MealTypesViewModel();
            if (mealTypes == null)
                return result;
            result.UseInDynamicCalculation = mealTypes.UseInDynamicCalculation;
            if (mealTypes.MealTypeDescriptions != null && mealTypes.MealTypeDescriptions.Any())
                result.MealTypes = mealTypes.MealTypeDescriptions.Select(c => new MealTypeViewModel { Description = c.Description, MarketBasePrice = c.MarketBasePrice, Number = c.Number, OperationalCost = c.OperationalCost }).ToList();

            result.PriceConstraints = PriceConstraint.Map(mealTypes.PriceConstraints);
            return result;
        }
        public static MealTypes Map(MealTypesViewModel mealTypes)
        {
            var result = new MealTypes();
            if (mealTypes == null)
                return result;
            if (mealTypes.MealTypes != null && mealTypes.MealTypes.Any())
                result.MealTypeDescriptions = mealTypes.MealTypes.Select(c => new MealTypeDescription { Description = c.Description, MarketBasePrice = c.MarketBasePrice, Number = c.Number, OperationalCost = c.OperationalCost }).ToList();
            else
                result.MealTypeDescriptions.Add(new MealTypeDescription { MarketBasePrice = 0, OperationalCost = 0, Number = (int)MealTypeEnum.OB });
            result.UseInDynamicCalculation = mealTypes.UseInDynamicCalculation;
            result.PriceConstraints = PriceConstraint.Map(mealTypes.PriceConstraints);
            result.Total = result.MealTypeDescriptions.Count();
            return result;
        }

    }
}