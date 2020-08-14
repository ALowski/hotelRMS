using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels.Basic
{
    public class InternalParserRoomData
    {
        public DateTime Date { get; set; }
        public int RoomType { get; set; }
        public int MealType { get; set; }
        public string HotelName { get; set; }
        public double Price { get; set; }
        public double Rating { get; set; }
        public int AverageOccupancy { get; set; }
        public int StayLength { get; set; }
    }
}
