using System;
using System.Collections.Generic;
using System.Linq;

namespace Hotels.Basic
{
    public static class DataSourceMerger
    {
        public static IEnumerable<Booking> Merge(bool newestOrdersFirst, params IEnumerable<Booking>[] sources)
        {
            IEnumerator<Booking>[] iters = sources.Select(x => x.GetEnumerator()).ToArray();
            SortedSet<Tuple<int, Booking>> order = new SortedSet<Tuple<int, Booking>>(new BookingComparer(newestOrdersFirst));
            for (int i = 0; i < sources.Length; ++i)
            {
                if (iters[i].MoveNext()) order.Add(Tuple.Create(i, iters[i].Current));
            }
            while (order.Count > 0)
            {
                var next = order.Min;
                yield return next.Item2;
                if (iters[next.Item1].MoveNext()) order.Add(Tuple.Create(next.Item1, iters[next.Item1].Current));
            }
        }

        private class BookingComparer : IComparer<Tuple<int, Booking>>
        {
            private readonly bool nof;

            public BookingComparer(bool newestOrdersFirst)
            {
                nof = newestOrdersFirst;
            }

            public int Compare(Tuple<int, Booking> x, Tuple<int, Booking> y)
            {
                if (x.Item2.CheckIn == y.Item2.CheckIn) return 0;
                return (nof == (x.Item2.CheckIn > y.Item2.CheckIn)) ? -1 : 1;
            }
        }
    }
}
