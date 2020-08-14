using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class CategoryOptionViewModel
    {
        public int Number { get; set; }

        [Display(Name = "Lower Bound")]
        public int LowerBound { get; set; }
        [Display(Name = "Upper Bound")]
        public int UpperBound { get; set; }
    }
}