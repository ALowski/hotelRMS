using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelGlob.RMS.Data.Models
{
    public class MealType: Entity
    {
        public string UID { get; set;}
        public string Name { get; set; }
    }
    public enum MealTypeEnum
    {
        OB=1,
        ВB=2,
        HB=3,
        HBPlus=4,
        FB=5,
        FBPlus=6,
        AI=7,
        AIPlus=8,
        UAI=9
    }
}
