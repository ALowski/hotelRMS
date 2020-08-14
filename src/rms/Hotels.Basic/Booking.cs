using System;

namespace Hotels.Basic
{
    public class Booking
    {
        public DateTime OrderTime { get; set; }
        public DateTime CheckIn { get; set; }
        public int LengthOfStay { get; set; }
        public double PricePerNight { get; set; }//if UseInDynamicCalculation==true then PricePerNight=RoomPrice + MealPrice else PricePerNight=RoomPrice
        public double RoomPrice { get; set; }
        public double MealPrice { get; set; }
        public BookingStatus Status { get; set; }
        public BookingType Type { get; set; }
    }
    public enum BookingStatus
    {
        CheckIn , Canceled,NoShows
    }
    public enum BookingType
    {
        Usual,
        Group,
        RackRate,
        Event
    }
}
