using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels.Basic
{
    public class InternalPrediction
    {
        public int RoomType { get; set; }
        public int CategoryType { get; set; }
        public int MealType { get; set; }
        public double ExpectedLoad { get; set; }
        public double NoShows { get; set; }
        public double Price { get; set; }
        public double Cancelation { get; set; }
    }
}
