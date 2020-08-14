using Newtonsoft.Json;
using System.Linq;

namespace Hotels.Config.ConfigModel
{
    public class ConfigurationRoot
    {
        [JsonConstructor]
        public ConfigurationRoot()
        {
            Seasons = new Seasons();
            Weekdays = new Weekdays();
            RoomTypes = new RoomTypes();
            MealTypes = new MealTypes();
            Categories = new Categories();
        }
        public Seasons Seasons { get; set; }

        public Weekdays Weekdays { get; set; }

        public RoomTypes RoomTypes { get; set; }

        public MealTypes MealTypes { get; set; }

        public Categories Categories { get; set; }

        public int GetCategoryId(int lengthOfStay, int BookingPeriod )
        {
            var cat = 0;
            foreach (OptionDescription x in Categories.StayPeriods)
                if (lengthOfStay >= x.LowerBound && lengthOfStay <= x.UpperBound)
                {
                    cat = x.Number;
                    break;
                }
            var total = Categories.BookingPeriods.Count();
            if (total > 0)
                cat *= total;
            foreach (OptionDescription x in Categories.BookingPeriods)
                if (BookingPeriod >= x.LowerBound && BookingPeriod <= x.UpperBound)
                {
                    cat += x.Number;
                    break;
                }
            return cat;
        }
    }
}
