using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hotels.Config.ConfigModel
{
    public class Categories
    {
        [JsonConstructor]
        public Categories()
        {
            StayPeriods = new List<OptionDescription>();
            BookingPeriods = new List<OptionDescription>();
            PriceReductions = new List<PriceReduction>();
        }

        public int Total { get; set; }

        public IEnumerable<OptionDescription> StayPeriods {get; set; } 

        public IEnumerable<OptionDescription> BookingPeriods {get; set; }

        public IEnumerable<PriceReduction> PriceReductions { get; set; }

        //public IEnumerable<PriceConstraint> PriceConstraints { get; set; }

        //public IEnumerable<CategoryDescription> Descriptions { get; set; }
    }    
}