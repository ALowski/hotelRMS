using Hotels.Config.ConfigModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class CategoryPriceReduction
    {
        [Display(Name = "Booking Period Number")]
        public int BookingPeriodNumber { get; set; }
        [Display(Name = "Stay Period Number")]
        public int StayPeriodNumber { get; set; }
        [Display(Name = "Price Reduction")]
        public double Reduction { get; set; }
    }
}