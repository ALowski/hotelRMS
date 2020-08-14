using System;
using Hotels.Basic;

namespace Hotels.Core.LoadPredictors
{
    public interface ILoadPredictor
    {
        void Initialize(DateTime today, bool[] pastDays, bool[] futureDays, int planningHorizont);
        void AddBooking(Booking booking);
        double[] Calculate(double[] forecast, double[] price);
        //double Continue(Booking booking, double partOfDay, double[] forecast);
        double[] GetForecast();
        double[] GetPrice();
    }
    public class PastDay
    {
        public int Stats { get; set; }
        public double Price { get; set; }
        public bool Value { get; set; }
    }
}
