using System;
using System.Collections.Generic;
using System.Linq;
using Hotels.Basic;
using Hotels.Core.Tools;

namespace Hotels.Core.LoadPredictors
{
    public sealed class HoltLoadPredicrtor : ILoadPredictor
    {
        private double _priceTemp;
        private double[] _price;
        private int _priceDay=-1;
        private double[] _forecast;
        private DateTime _today;
        private List<PastDay> _pastDays;
        private bool[] _futureDays;
        private int _count = 0;

        public void Initialize(DateTime today, bool[] pastDays, bool[] futureDays, int planningHorizont)
        {
            _priceTemp = -1d;
            _today = today;
            //_forecast = new double[planningHorizont];
            _pastDays = new List<PastDay>();
            _futureDays = futureDays;

            var start = pastDays.Length - 1;

            while (start >= 0 && _count <  90)
            {
                if (pastDays[start])
                {
                    ++_count;
                }
                --start;
            }

            for (++start; start < pastDays.Length; ++start)
            {
                _pastDays.Add(new PastDay { Value = pastDays[start], Stats = 0 });
            }
        }


        public void AddBooking(Booking booking)
        {
            var dayNumber = Math.Max(-1, (_today - booking.CheckIn).Days);
            if (dayNumber > -1 && dayNumber <= _pastDays.Count)
            {
                if (_priceDay < 0)
                    _priceDay = dayNumber;
                if (dayNumber <= _priceDay)
                {
                    if (dayNumber < _priceDay || (dayNumber == _priceDay && _priceTemp < booking.PricePerNight))
                    {
                        _priceTemp = booking.PricePerNight;
                    }
                    _priceDay = dayNumber;
                }                
                var pastDay = _pastDays.ElementAt(_pastDays.Count-dayNumber);
                pastDay.Stats += booking.LengthOfStay;
            }
        }


        public double[] Calculate(double[] forecast, double[] price)
        {
            var days=_pastDays.Where(c=>c.Value).Select(c=>(double)c.Stats).ToArray();
            if ((double)days.Count(c => c > 0) / days.Count() < 0.8)
                return null;
            var result = HoltsCalculation.Calculate(Math.Min(_futureDays.Where(c => c).Count(), 90), days);
            int m = 0;
            //double dif = 0;
            for (var i = 0; i < _futureDays.Count()&&i<90; ++i)
            {
                if (_futureDays[i])
                {
                    forecast[i] = result[m];
                    price[i] = _priceTemp;//+ dif;
                    //dif = forecast[i] - (int)forecast[i];
                    //forecast[i] = (int)forecast[i];                    
                    m++;
                }
            }
            _price = price;
            _forecast = forecast;
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