using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotelGlob.RMS.Data.Models;
using HotelGlob.RMS.Data;
using Hotels.Basic;
using System.Data.Entity;
using HotelGlob.RMS.Adapter;

namespace HotelGlob.RMS.BL
{
    public class GroupCalculationBL
    {
        public GroupCalcInput GetCalcInputForIndex(RmsAdapter rmsAdapter, Dictionary<int,string> roomsNumber, Dictionary<int, string> mealsNumber)
        {
            var result = rmsAdapter.GetGroupCalculationResult();
            GroupCalcInput gc = new GroupCalcInput();
            gc.Start = DateTime.Today;
            gc.End = DateTime.Today;
            gc.Inputs = new List<GroupCalcDayInput> { GetNewInput(DateTime.Today, roomsNumber, mealsNumber, result) };
            return gc;
        }
        public  GroupCalcDayInput GetNewInput(DateTime date, Dictionary<int, string> roomTypes, Dictionary<int, string> mealTypes, IEnumerable<GroupCalculationResult> info)
        {
            var input = new GroupCalcDayInput { Date = date, RoomTypes = new Dictionary<string, RoomInfo>(), MealTypes = new Dictionary<string, MealInfo>() };
            var dayResults = info.FirstOrDefault();
            foreach (int i in roomTypes.Keys)
            {
                var roomInfo = dayResults.Results.FirstOrDefault(c => c.RoomType == i);
                input.RoomTypes.Add(i.ToString(),  new RoomInfo { Value = 0, Name= roomTypes[i], EmptyRooms = roomInfo == null ? 0 : Convert.ToInt32(roomInfo.EmptyRooms), PlanEmptyRooms = roomInfo == null ? 0 : Convert.ToInt32(roomInfo.PlanEmptyRooms) });
            }
            foreach (int i in mealTypes.Keys)
                input.MealTypes.Add(i.ToString(), new MealInfo { Name = mealTypes[i], Value = 0 });
            return input;
        }
        public async Task<RmsAdapter> InitRmsAdapter(DateTime start, DateTime end, int hotelId, RmsDbContext db)
        {
            return await
                Task.Run(() =>
                {
                    HotelSettings hotel = db.HotelSettings.Find(hotelId);

                    if (hotel == null || !hotel.IsRmsEnalbed)
                    {
                        return null;
                    }
                    var calculations = new List<Calculation>();
                    for (var s = start.Date; s <= end.Date; s = s.AddDays(1))
                    {
                        calculations.Add(db.Calculations.Where(c => c.HotelId == hotelId && DbFunctions.TruncateTime(c.PredictionDate) == s).OrderByDescending(c => c.CalculatedOn).FirstOrDefault());
                    }
                    IEnumerable<Reservation> reservations = db.Reservations.Where(r => r.HotelId == hotelId && (r.CheckInDate <= end.Date && DbFunctions.AddDays(r.CheckInDate, r.DaysCount - 1) >= start.Date)).ToList();
                    IEnumerable<Reservation> groupReservations = db.Reservations.Where(r => r.HotelId == hotelId && r.ReservationType == ReservationType.Group).ToList();
                    var rmsAdapter = new RmsAdapter();
                    rmsAdapter.RunGroupCalculation(start, end, reservations, groupReservations, calculations, hotel.Settings);                    
                    return rmsAdapter;
                });
        }
        public GroupCalcInput GetInputPartialView(GroupCalcInput viewModel, RmsAdapter rmsAdapter)
        {
            var roomInfoResult = rmsAdapter.GetGroupCalculationResult();
            if (viewModel.Inputs != null && viewModel.Inputs.Any() && viewModel.Inputs[0].RoomTypes.Any())
            {
                var roomTotal = viewModel.Inputs[0].RoomTypes.ToDictionary(c => Convert.ToInt32(c.Key), c => c.Value.Name);
                var mealTotal = new Dictionary<int,string>();
                if (viewModel.Inputs[0].MealTypes != null && viewModel.Inputs[0].MealTypes.Any())
                    mealTotal = viewModel.Inputs[0].MealTypes.ToDictionary(c => Convert.ToInt32(c.Key), c => c.Value.Name);
                for (DateTime i = viewModel.Start; i <= viewModel.End; i = i.AddDays(1))
                    if (!viewModel.Inputs.Any(c => c.Date == i))
                        viewModel.Inputs.Add(GetNewInput(i, roomTotal, mealTotal, roomInfoResult.Where(c => c.Date == i)));
                viewModel.Inputs.RemoveAll(c => c.Date < viewModel.Start || viewModel.End < c.Date);
            }
            viewModel.Inputs = viewModel.Inputs.OrderBy(c => c.Date).ToList();
            return viewModel;
        }
    }
}
