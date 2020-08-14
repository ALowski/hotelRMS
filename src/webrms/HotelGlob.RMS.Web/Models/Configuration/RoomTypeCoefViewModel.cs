using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class RoomTypeCoefViewModel
    {
        [Display(Name = "Room Type Code")]
        public string RoomTypeCode { get; set; }
        [Display(Name = "Guests Number")]
        public int PeopleNum { get; set; }
        [Display(Name = "Reduction Coefficient")]
        public double Coef { get; set; }
    }
}