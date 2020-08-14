using System;

namespace Hotels.Core.DaytimeExtrapolators
{
    public sealed class DiscreteDaytimeExtrapolator : IDaytimeExtrapolator
    {
        private const int SecondsInDay = 24 * 60 * 60;
        private const int SecondsInTenMinutes = 10 * 60;

        private readonly int[] _stats;
        private int _count;


        public DiscreteDaytimeExtrapolator()
        {
            _stats = new int[SecondsInDay / SecondsInTenMinutes];
            _count = 0;
        }


        public void AddBooking(DateTime bookingTime)
        {
            _count++;
            _stats[GetIndex(bookingTime)]++;
        }


        public void Calculate()
        {
            if (_count <= 0)
            {
                return;
            }

            for (var i = 1; i < _stats.Length; ++i)
            {
                _stats[i] += _stats[i - 1];
            }

            var firstValueIndex = 0;

            while (_stats[firstValueIndex] == 0)
            {
                firstValueIndex++;
            }

            for (var i = 0; i < firstValueIndex; ++i)
            {
                _stats[i] = _stats[firstValueIndex];
            }
        }


        public double GetDayPart(DateTime currentTime)
        {
            return _stats[GetIndex(currentTime)] / (double)_count;
        }



        private static int GetIndex(DateTime dateTime)
        {
            return (int)((long)dateTime.TimeOfDay.TotalSeconds % SecondsInDay) / SecondsInTenMinutes;
        }
    }
}
