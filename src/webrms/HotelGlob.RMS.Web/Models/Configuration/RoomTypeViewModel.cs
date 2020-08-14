using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class RoomTypeViewModel
    {
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        [Display(Name = "Guests Number")]
        public int PeopleNum { get; set; }
        [Display(Name = "Operational Cost")]
        public double OperationalCost { get; set; }
        [Display(Name = "Market Base Price (optional)")]
        public double MarketBasePrice { get; set; }
        [Display(Name = "Lower Price Bound")]
        public double LowerBound { get; set; }
        [Display(Name = "Upper Price Bound")]
        public double UpperBound { get; set; }

        public int Number { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Room Type Code")]
        public string RoomTypeCode { get; set; } 

    }
}