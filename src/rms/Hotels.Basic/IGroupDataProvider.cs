using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels.Basic
{
    public interface IGroupDataProvider
    {
        IEnumerable<Booking> GetPlanBookings(DateTime date, int roomType);
        IEnumerable<Booking> GetGroupBookings(int seasonType,  int roomType );
        InternalCalculation GetPredictions(DateTime date);
    }
}
