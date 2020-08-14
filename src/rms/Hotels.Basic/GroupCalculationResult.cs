using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels.Basic
{
    public class GroupCalculationDayResult
    {
        public int RoomType { get; set; }
        public double EmptyRooms { get; set; }
        public double PlanEmptyRooms { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public double Price { get; set; }
    }
    public class GroupCalculationResult
    {
        public DateTime Date { get; set; }
        public List<GroupCalculationDayResult> Results { get; set; }
    }
}
