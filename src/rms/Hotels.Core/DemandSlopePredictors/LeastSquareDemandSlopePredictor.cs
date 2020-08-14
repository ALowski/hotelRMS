using System;
using System.Linq;
using System.Collections.Generic;
using Hotels.Basic;
using MathWorks.MATLAB.NET.Arrays;
using Fitting;

namespace Hotels.Core.DemandSlopePredictors
{
    public sealed class LeastSquareDemandSlopePredictor : IDemandSlopePredictor
    {
        private const double MinimalValue = -1e-10;

        //private int _currentDay;
        //private List<Booking> currentList=new List<Booking>();
        private List<Booking> bookingList = new List<Booking>();
        //private double _sum1;
        //private double _sumX;
        //private double _sumY;
        //private double _sumX2;
        //private double _sumXy;
        //private double _x;
        //private double _y;


        //public LeastSquareDemandSlopePredictor()
        //{
        //    _currentDay = 0;
        //    _sum1 = -1d;
        //    _sumX = _sumY = _sumX2 = _sumXy = _x = _y = 0d;
        //}


        //public void AddBooking(Booking booking)
        //{
        //    var dayOfYear = booking.CheckIn.DayOfYear;
        //    bookingList.Add(booking);
        //    if (_currentDay != dayOfYear)
        //    {
        //        Increment();
        //        currentList.Clear();                
        //        _currentDay = dayOfYear;                
        //    }
        //    currentList.Add(booking);
        //}

        public void AddBooking(Booking booking)
        {
            bookingList.Add(booking);
        }

        //public double Calculate()
        //{
        //    var sumX=0.0;
        //    var sumY = 0.0;
        //    var x=0.0;
        //    var y=0.0;
        //    var sumX2=0.0;
        //    var sumXY=0.0;
        //    var count = 0.0;
        //    foreach(var item in bookingList.GroupBy(c=>c.PricePerNight))
        //    {
        //        count++;
        //        x=item.Key;
        //        y=item.Select(c=>c.LengthOfStay).Sum();
        //        sumX += x ;
        //        sumY += y;
        //        sumX2 += x * x;
        //        sumXY += y * x;
        //    }

        //    var denominator = count * sumX2 - sumX * sumX;

        //    if (Math.Abs(denominator) < 1e-12)
        //    {
        //        return MinimalValue;
        //    }
        //    var result = (count*sumXY - sumX*sumY) / denominator;
        //    return (result >= MinimalValue) ? MinimalValue : result;
            
        //}
        public double CalculateMatLab()
        {
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            foreach (var item in bookingList.GroupBy(c => c.PricePerNight))
            {
                xList.Add(item.Key);
                yList.Add(item.Select(c => c.LengthOfStay).Sum());                
            }
            if (xList.Count()<2)
                return MinimalValue;
            MWNumericArray _x = new MWNumericArray(xList.Count(), 1, xList.ToArray());
            MWNumericArray _y = new MWNumericArray(yList.Count(), 1, yList.ToArray());
            
            FittingClass f = new FittingClass();
            var res = f.Fitting(_x, _y);

            double b = ((MWNumericArray)res).ToScalarDouble();
            return (b >= MinimalValue) ? MinimalValue : b;
        }
        //Manual Canculate

        public double Calculate()
        {
            var sumX = 0.0;
            var sumY = 0.0;
            var x = 0.0;
            var y = 0.0;
            var sumX2 = 0.0;
            var sumXY = 0.0;
            var count = 0.0;
            foreach (var item in bookingList.GroupBy(c => c.PricePerNight))
            {
                count++;
                x = item.Key;
                y = item.Select(c => c.LengthOfStay).Sum();               
                sumX += x;
                sumY += y;
                sumX2 += x * x;
                sumXY += y * x;
            }
          
            double denominator = count * sumX2 - sumX * sumX;
            if (Math.Abs(denominator) < 1e-12)
            {
                return MinimalValue;
            }
            var result = (count * sumXY - sumX * sumY) / denominator;

            return (result >= MinimalValue) ? MinimalValue : result;
        }

        //public double Calculate()
        //{
        //    Increment();


        //    var sumX = 0.0;
        //    var sumY = 0.0;
        //    var x = 0.0;
        //    var y = 0.0;
        //    var sumX2 = 0.0;
        //    var sumXY = 0.0;
        //    var count = 0.0;
        //    foreach (var item in bookingList.GroupBy(c => c.PricePerNight))
        //    {
        //        count++;
        //        x = item.Key;
        //        y = item.Select(c => c.LengthOfStay).Sum();
        //        sumX += x;
        //        sumY += y;
        //        sumX2 += x * x;
        //        sumXY += y * x;
        //    }

        //    var denominator = count * sumX2 - sumX * sumX;

        //    if (Math.Abs(denominator) < 1e-12)
        //    {
        //        return MinimalValue;
        //    }
        //    var result = (count * sumXY - sumX * sumY) / denominator;
        //    var return1 = (result >= MinimalValue) ? MinimalValue : result;
        //    var denominator1 = _sum1 * _sumX2 - _sumX * _sumX;

        //    if (Math.Abs(denominator1) < 1e-12)
        //    {
        //        return MinimalValue;
        //    }
        //    var result1 = (_sum1 * _sumXy - _sumX * _sumY) / denominator1;
        //    var return2 = (result1 >= MinimalValue) ? MinimalValue : result1;
        //    return return1;
        //}


        //private void Increment()
        //{
        //    _sum1 += 1;
        //    if (currentList.Count() > 0)
        //    {
        //        _y=currentList.Select(c => c.LengthOfStay).Sum();
        //        _x=currentList.Select(c => c.PricePerNight * c.LengthOfStay).Sum() / _y;
        //        _sumY += _y;
        //        _sumX += _x;
        //        _sumX2 += _x * _x;
        //        _sumXy += _x * _y;
        //    }
        //}
    }
}