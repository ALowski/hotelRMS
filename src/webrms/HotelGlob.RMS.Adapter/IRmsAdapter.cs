using HotelGlob.RMS.Data.Models;
using Hotels.Basic;
using System;
using System.Collections.Generic;

namespace HotelGlob.RMS.Adapter
{
    public interface IRmsAdapter
    {
        IEnumerable<Calculation> Run(DateTime startDate, int planningHorizon, int pastPeriod,
            IEnumerable<Reservation> reservations, IEnumerable<Inflation> inflations, IEnumerable<Event> events, IEnumerable<Parser_RoomData> parserRoomData,
            string settings,double rating);
        IEnumerable<GroupCalculationResult> RunGroupCalculationPriceCalculation(GroupCalcInput input);
        void RunGroupCalculation(DateTime start, DateTime end, IEnumerable<Reservation> reservations, IEnumerable<Reservation> groupReservations, IEnumerable<Calculation> predictions,
            string settings);
        IEnumerable<GroupCalculationResult> GetGroupCalculationResult();
    }
}
