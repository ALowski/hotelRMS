using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class PriceConstraint
    {
        [Display(Name = "Lower Price")]
        public string Less { get; set; }
        [Display(Name = "Upper Price")]
        public string More { get; set; }

        public static List<PriceConstraint> Map(IEnumerable<Hotels.Config.ConfigModel.PriceConstraint> constraints)
        {
            if (constraints != null && constraints.Any())
                return constraints.Select(c => new PriceConstraint { More = c.More.ToString(), Less = c.Less.ToString() }).ToList();
            return new List<PriceConstraint>();
        }

        public static List< Hotels.Config.ConfigModel.PriceConstraint> Map(List<PriceConstraint> constraints)
        {
            if (constraints != null && constraints.Any())
                return constraints.Select(c => new Hotels.Config.ConfigModel.PriceConstraint { More = Convert.ToInt32(c.More), Less = Convert.ToInt32(c.Less)}).ToList();
            return new List<Hotels.Config.ConfigModel.PriceConstraint>();
        }
    }
}