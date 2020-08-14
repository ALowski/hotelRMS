using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class MealTypeViewModel
    {
        [Display(Name = "Type")]
        public int Number { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Operational Cost")]
        public double OperationalCost { get; set; }
        [Display(Name = "Price")]
        public double MarketBasePrice { get; set; }
    }
}