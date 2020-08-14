using Newtonsoft.Json;
using System.Collections.Generic;

namespace Hotels.Config.ConfigModel
{
    public class RoomTypes
    {
        [JsonConstructor]
        public RoomTypes()
        {
            RoomTypeDescriptions = new List<RoomTypeDescription>();
            RoomTypeCoefs = new List<RoomTypeCoef>();
            ChildRooms = new List<ChildRooms>();
            PriceConstraints = new List<PriceConstraint>();
        }

        public int Total { get; set; }
        
        public List<RoomTypeDescription> RoomTypeDescriptions { get; set; }

        public IEnumerable<RoomTypeCoef> RoomTypeCoefs { get; set; }

        public IEnumerable<ChildRooms> ChildRooms { get; set; }

        public IEnumerable<PriceConstraint> PriceConstraints { get; set; }
    }
}