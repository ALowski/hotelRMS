using System;
using System.Collections.Generic;
using System.Linq;
using Hotels.Basic;
using Hotels.Core.Tools;

namespace Hotels.Core.LoadPredictors
{
    public sealed class SameDayLoadPredictor : ILoadPredictor
    {
        private double[] _price;
        private double[] _forecast;
        private DateTime _today;
        private List<PastDay> _pastDays;
        private bool[] _futureDays;
        private int _count = 0;

        public void Initialize(DateTime today, bool[] pastDays, bool[] futureDays, int planningHorizont)
        {
            _today = today;
            //_forecast = new double[planningHorizont];
            _pastDays = new List<PastDay>();
            _futureDays = futureDays;
            foreach (var item in pastDays)
            {
                _pastDays.Add(new PastDay { Value = item, Stats = 0 });
            }
        }


        public void AddBooking(Booking booking)
        {
            var dayNumber = Math.Max(-1, (_today - booking.CheckIn).Days);
            if (dayNumber > -1 && dayNumber <= _pastDays.Count)
            {
                var pastDay = _pastDays.ElementAt(_pastDays.Count - dayNumber);
                pastDay.Stats += booking.LengthOfStay;
                if (booking.PricePerNight > pastDay.Price)
                    pastDay.Price = booking.PricePerNight;
            }
        }


        public double[] Calculate(double[] forecast, double[] price)
        {
            //double dif = 0;
            for (var i = 90; i < _futureDays.Count(); ++i)
            {
                if (_futureDays[i])
                {
                    var date = _today.AddDays(i);
                    var week = date.GetWeekNumber();
                    var year =  new DateTime(date.Year-1 , 1, 1);
                    var prevDate = year.AddDays( - (int)year.DayOfWeek + 7 * (week-1) + (int)date.DayOfWeek);
                    var index = _pastDays.Count - Math.Max(-1, (_today - prevDate).Days);
                    if (_pastDays.Count() >= index && index >= 0)
                    {
                        while (!_pastDays[index].Value&&_pastDays.Count() >= index)
                            index++;
                    }
                    if (_pastDays.Count() >= index && index>=0)
                    {
                        var value = _pastDays[index].Stats;
                        forecast[i] = value;
                        price[i] = _pastDays[index].Price;
                        var lastDay = _pastDays.Last(c => c.Value);
                        if (lastDay != null)
                        {
                            var prevDay = _today.AddDays(-_pastDays.Count + _pastDays.IndexOf(lastDay));
                            while (date.DayOfWeek != prevDay.DayOfWeek)
                            {
                                prevDay = prevDay.AddDays(-1);
                            }
                            var prevIndex = _pastDays.Count - (_today.Date - prevDay.Date).Days;
                            double coef = (_pastDays[prevIndex].Value ? 1 : 0) + (_pastDays[prevIndex-7].Value ? 1 : 0) + (_pastDays[prevIndex - 14].Value ? 1 : 0) + (_pastDays[prevIndex - 21].Value ? 1 : 0);
                            forecast[i] += ((_pastDays[prevIndex].Stats + _pastDays[prevIndex - 7].Stats + _pastDays[prevIndex - 14].Stats + _pastDays[prevIndex - 21].Stats)/ coef) - value;// +dif;
                        }
                        //dif = forecast[i] - (int)forecast[i];
                        //forecast[i] = (int)forecast[i];
                    }
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