using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelGlob.RMS.Data.Models
{
    public class Parser_RoomInfo : Entity
    {
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }
        public virtual RoomType RoomType { get; set; }
        [Display(Name = "Meal Type")]
        public int MealTypeId { get; set; }
        public virtual MealType MealType { get; set; }
        public int HotelId { get; set; }
        public virtual HotelSettings Hotel { get; set; }
        [Display(Name = "Target Room Type")]
        public string RoomName { get; set; }
        [Display(Name = "Guests Number")]
        public int PeopleNum { get; set; }
        public bool Cancelation {get;set;}
        public bool Prepayment { get; set; }
        public string City { get; set; }
        [Display(Name = "Target Hotel")]
        public string HotelName { get; set; }
        [Display(Name = "Target Meal Type")]
        public string MealName { get; set; }
    }
}
