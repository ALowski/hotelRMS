using System;

namespace Hotels.Core.DaytimeExtrapolators
{
    public interface IDaytimeExtrapolator
    {
        void AddBooking(DateTime bookingTime);
        void Calculate();
        double GetDayPart(DateTime currentTime);
    }
}
