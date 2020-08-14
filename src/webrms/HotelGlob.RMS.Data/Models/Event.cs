using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelGlob.RMS.Data.Models
{
    public class Event : Entity
    {
        public int HotelId { get; set; }
        public virtual HotelSettings Hotel { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
        [Display(Name = "Load coefficient")]
        public double Coef { get; set; }
        [Display(Name = "Price coefficient")]
        public double PriceCoef { get; set; }
    }
}
