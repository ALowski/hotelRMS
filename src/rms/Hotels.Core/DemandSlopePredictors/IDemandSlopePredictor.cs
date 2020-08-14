using Hotels.Basic;

namespace Hotels.Core.DemandSlopePredictors
{
    public interface IDemandSlopePredictor
    {
        void AddBooking(Booking booking);
        double Calculate();
    }
}
