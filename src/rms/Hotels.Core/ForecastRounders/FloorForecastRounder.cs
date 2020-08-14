using System;

namespace Hotels.Core.ForecastRounders
{
    public sealed class FloorForecastRounder : IForecastRounder
    {
        public void Round(double[] forecast)
        {
            if (forecast == null || forecast.Length == 0)
            {
                return;
            }

            var sum = 0d;
            var prev = 0d;
            var begin = 0;
            var middle = 0;
            var random = new Random(Guid.NewGuid().GetHashCode());

            for (var i = 0; i < forecast.Length; ++i)
            {
                sum += forecast[i] - (int)forecast[i];
                forecast[i] = (int)forecast[i];

                if (sum + prev >= 1d)
                {
                    var index = (random.NextDouble() < prev) ? random.Next(begin, middle) : random.Next(middle, i + 1);
                    forecast[index]++;

                    prev += sum - 1;
                    sum = 0;
                    begin = middle;
                    middle = i + 1;
                }
            }
        }
    }
}
