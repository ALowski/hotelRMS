using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelGlob.RMS.Data.Models
{
    public class RoomType: Entity
    {
        public string Name { get; set; }
        public string RoomTypeCode { get; set; }
        public int HotelId { get; set; }
        public virtual HotelSettings Hotel { get; set; }
    }
}
