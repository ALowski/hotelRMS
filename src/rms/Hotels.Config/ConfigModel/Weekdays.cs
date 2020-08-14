using System.Collections.Generic;

namespace Hotels.Config.ConfigModel
{
    public class Weekdays
    {
        public Weekdays()
        {
            PriceReductions = new List<PriceReduction>();
        }
        public int Total { get; set; }

        public int Monday { get; set; }

        public int Tuesday { get; set; }

        public int Wednesday { get; set; }

        public int Thursday { get; set; }

        public int Friday { get; set; }

        public int Saturday { get; set; }

        public int Sunday { get; set; }

        public IEnumerable<PriceReduction> PriceReductions { get; set; }
    }
}