using System;
using System.Collections.Generic;
using System.Linq;
using Hotels.Basic;

namespace Hotels.Data
{
    public class SqlServerDataProvider : IDataProvider
    {
        public IEnumerable<Booking> GetBookings(DateTime start, DateTime today, int seasonType, int weekdayType, int roomType, int mealType, int categoryInfo)
        {
            return null;
        }
        public IEnumerable<InternalEvent> GetEvents(DateTime date)
        {
            return null;
        }
        public IEnumerable<InternalParserRoomData> GetParserRoomData(DateTime date, int roomType, int mealType, int LStayStart, int LStayEnd)
        {
            return null;
        }
        public IEnumerable<Booking> GetBookingsForPlan(DateTime end, DateTime today, int roomType, int seasonType, int weekdayType, int categoryInfo, int stayStart, int stayEnd, int mealType)
        {
            return null;
        }

        public IEnumerable<Booking> GetBookings(DateTime start, DateTime today, int seasonType, int weekdayType, int roomType, int categoryInfo, int stayStart, int stayEnd, int mealType)
        {
            return null;
        }
        public IEnumerable<Booking> GetBookingsForPlan(DateTime end, DateTime today, int roomType, int seasonType, int weekdayType, int categoryInfo, int mealType)
        {
            return null;
        }
        public int GetBookingsCount(DateTime start, DateTime today, int seasonType, int weekdayType, int roomType, int categoryInfo)
        {
            return -1;
        }
        public double GetInflationCoef(DateTime start, DateTime end)
        {
            return 1;
        }

        public IEnumerable<Booking> GetGroupBookings(DateTime start, DateTime today, int seasonType, int roomType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Booking> GetGroupBookingsForPlan(DateTime end, DateTime today, int roomType, int seasonType)
        {
            throw new NotImplementedException();
        }
    }
}
