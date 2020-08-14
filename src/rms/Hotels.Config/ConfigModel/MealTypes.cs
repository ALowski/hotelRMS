using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hotels.Config.ConfigModel
{
    public class MealTypes
    {
        [JsonConstructor]
        public MealTypes()
        {
            MealTypeDescriptions = new List<MealTypeDescription>();
            PriceConstraints = new List<PriceConstraint>();
        }

        public int Total { get; set; }

        public bool UseInDynamicCalculation { get; set; }

        public List<MealTypeDescription> MealTypeDescriptions { get; set; }

        public IEnumerable<PriceConstraint> PriceConstraints { get; set; }
    }
}