using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class ChildRoomTypeViewModel
    {
        [Display(Name = "Quantity inside the block")]
        public int Quantity { get; set; }
        [Display(Name = "Blocks Room Type")]
        public string Parent { get; set; }
        [Display(Name = "Inner Room Type")]
        public string Child { get; set; }
    }
}