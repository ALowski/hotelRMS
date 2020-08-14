namespace Hotels.Config.ConfigModel
{
    public class RoomTypeDescription
    {
        public int Number { get; set; }

        public string Description { get; set; }

        public int PeopleNum { get; set; }

        public int Quantity { get; set; }

        public double OperationalCost { get; set; }

        public double MarketBasePrice { get; set; }

        public double LowerBound { get; set; }

        public double UpperBound { get; set; }
    }
}