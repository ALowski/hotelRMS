using System;
using Hotels.Basic;

namespace Hotels.Core.DemandSlopePredictors
{
    public sealed class LeastDistanceDemandSlopePredictor : IDemandSlopePredictor
    {
        private const double MinimalValue = -1e-10;

        private int _currentDay;
        private double _sum1;
        private double _sumX;
        private double _sumY;
        private double _sumX2;
        private double _sumXy;
        private double _sumY2;
        private double _x;
        private double _y;

        public LeastDistanceDemandSlopePredictor()
        {
            _currentDay = 0;
            _sum1 = -1d;
            _sumX = _sumY = _sumX2 = _sumXy = _sumY2 = _x = _y = 0d;
        }

        public void AddBooking(Booking booking)
        {
            var dayOfYear = booking.CheckIn.DayOfYear;

            if (_currentDay != dayOfYear)
            {
                Increment();

                _x = booking.PricePerNight;
                _y = booking.LengthOfStay;
                _currentDay = dayOfYear;
            }
            else
            {
                _y += booking.LengthOfStay;
            }
        }

        public double Calculate()
        {
            Increment();

            double denominator = _sum1*_sumXy - _sumX*_sumY;

            if (Math.Abs(denominator) < 1e-12)
            {
                return MinimalValue;
            }

            var a = (_sum1*_sumX2 - _sum1*_sumY2 - _sumX*_sumX + _sumY*_sumY) / denominator;
            return (-a - Math.Sqrt(a*a + 4)) / 2.0;
        }

        private void Increment()
        {
            _sum1 += 1;
            _sumX += _x;
            _sumY += _y;
            _sumX2 = _x*_x;
            _sumXy = _x*_y;
            _sumY2 = _y*_y;
        }
    }
}