using System;
using System.Collections.Generic;

namespace Hotels.Basic
{
    public interface IDataProvider
    {
        IEnumerable<Booking> GetBookings(DateTime start, DateTime today, int seasonType, int weekdayType, int roomType, int categoryInfo, int mealType);
        IEnumerable<InternalEvent> GetEvents(DateTime date);
        IEnumerable<Booking> GetGroupBookings(DateTime start, DateTime today, int seasonType, int roomType);
        IEnumerable<Booking> GetBookings(DateTime start, DateTime today, int seasonType, int weekdayType, int roomType, int categoryInfo, int stayStart, int stayEnd, int mealType);
        IEnumerable<Booking> GetBookingsForPlan(DateTime end, DateTime today, int roomType, int seasonType, int weekdayType, int categoryInfo, int mealType);
        IEnumerable<Booking> GetGroupBookingsForPlan(DateTime end, DateTime today, int roomType, int seasonType);
        IEnumerable<Booking> GetBookingsForPlan(DateTime end, DateTime today, int roomType, int seasonType, int weekdayType, int categoryInfo, int stayStart, int stayEnd, int mealType);
        double GetInflationCoef(DateTime start, DateTime end);
        int GetBookingsCount(DateTime start, DateTime today, int seasonType, int weekdayType, int roomType, int categoryInfo);

        IEnumerable<InternalParserRoomData> GetParserRoomData(DateTime date, int roomType, int mealType, int LStayStart, int LStayEnd);
    }
}
