using System;
using System.Collections.Generic;
using System.Linq;
using Hotels.Basic;

namespace Hotels.Core.LoadPredictors
{
    public sealed class MovingAverageLoadPredictor : ILoadPredictor
    {
        private double _priceTemp;
        private double[] _price;
        private int _priceDay=-1;
        private double[] _forecast;
        private DateTime _today;
        private Queue<PastDay> _pastDays;
        private bool[] _futureDays;
        private int _count = 0;

        public void Initialize(DateTime today, bool[] pastDays, bool[] futureDays,int planningHorizont)
        {
            _priceTemp = -1d;
            _today = today;
            //_forecast = new double[planningHorizont];
            _pastDays = new Queue<PastDay>();
            _futureDays = futureDays;
                        
            var start = pastDays.Length - 1;

            while (start >= 0 && _count<30)
            {
                if (pastDays[start])
                {
                    ++_count;
                }
                --start;
            }

            for (++start; start < pastDays.Length; ++start)
            {
                _pastDays.Enqueue(new PastDay { Value = pastDays[start], Stats = 0 });
            }
        }


        public void AddBooking(Booking booking)
        {
            var dayNumber = Math.Max(-1, (_today-booking.CheckIn).Days);
            if (dayNumber > -1 && dayNumber <= _pastDays.Count)
            {
                if(_priceDay<0)
                    _priceDay = dayNumber;
                if (dayNumber <= _priceDay)
                {
                    if (dayNumber < _priceDay||(dayNumber == _priceDay && _priceTemp < booking.PricePerNight))
                    {
                        _priceTemp = booking.PricePerNight;
                    }
                    _priceDay = dayNumber;
                }
                var pastDay = _pastDays.Reverse().ElementAt(dayNumber-1);
                pastDay.Stats += booking.LengthOfStay;
            }
        }


        public double[] Calculate(double[] forecast, double[] price)
        {
            _price = price;
            double total=0.0;
            int length = 8;
            double percent=0.35;
            var values=_pastDays.Reverse().Where(c=>c.Value).ToList();
            if (((double)values.Take(length).Count(c => c.Stats > 0) / length) < percent)
            {
                length=15;
                if(((double)values.Take(length).Count(c=>c.Stats>0)/length)<percent)
                    length=30;
            }
            for(int i=0;i<length;i++)
                total+=values.ElementAt(i).Stats*(length-i);
            var value = 2 * total / (length * (length + 1));
            //var total = _pastDays.Reverse()..Sum(ps => ps.Value ? ps.Stats : 0);
            //var value = (double)total / _count;
            //double dif = 0;
            for (var i = 0; i < _futureDays.Count() && i < 90; ++i)
            {
                if (_futureDays[i])
                {
                    forecast[i] = value;
                    price[i] = _priceTemp;
                //        + dif;
                //    dif = forecast[i] - (int)forecast[i];
                //    forecast[i] = (int)forecast[i];                                     
                }                
            }
            _forecast = forecast;
            _price = price;
            return _price;
        }

        public double[] GetForecast()
        {
            return _forecast;
        }
        public double[] GetPrice()
        {
            return _price;
        }
    }
}