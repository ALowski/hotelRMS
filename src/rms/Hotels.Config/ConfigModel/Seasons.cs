using System.Collections.Generic;

namespace Hotels.Config.ConfigModel
{
    public class Seasons
    {
        public Seasons()
        {
            SeasonDescriptions = new List<SeasonDescription>();
            PriceReductions = new List<PriceReduction>();
        }
        public int Total { get; set; }

        public List<SeasonDescription> SeasonDescriptions { get; set; }

        public IEnumerable<PriceReduction> PriceReductions { get; set; }
    }
}