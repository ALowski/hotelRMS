using HotelGlob.RMS.Data.Models;
using Hotels.Config.ConfigModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class RoomTypesViewModel
    {
        public bool IsEditable { get; set; }
        public List<RoomTypeViewModel> RoomTypes { get; set; }
        public List<RoomTypeCoefViewModel> RoomTypeCoefs { get; set; }
        public List<ChildRoomTypeViewModel> ChildRoomTypes { get; set; }
        public List<PriceConstraint> PriceConstraints {get; set;}
        public RoomTypesViewModel()
        {
            RoomTypes = new List<RoomTypeViewModel>();
            RoomTypeCoefs = new List<RoomTypeCoefViewModel>();
            ChildRoomTypes = new List<ChildRoomTypeViewModel>();
            PriceConstraints = new List<PriceConstraint>();
        }

        public static RoomTypesViewModel Map(RoomTypes roomTypes, IEnumerable<RoomType> dbRoomTypes)
        {
            var result = new RoomTypesViewModel();
            if (roomTypes == null)
                return result;
            if (roomTypes.RoomTypeDescriptions != null && roomTypes.RoomTypeDescriptions.Any())
                result.RoomTypes = roomTypes.RoomTypeDescriptions.Select(c => new RoomTypeViewModel { Description = c.Description,PeopleNum=c.PeopleNum, LowerBound = c.LowerBound, MarketBasePrice = c.MarketBasePrice, Number = c.Number, OperationalCost = c.OperationalCost, Quantity = c.Quantity, UpperBound = c.UpperBound }).ToList();
            var dict = dbRoomTypes.ToDictionary(c =>  c.Id, c => c.RoomTypeCode);
            if (roomTypes.RoomTypeCoefs != null && roomTypes.RoomTypeCoefs.Any())
                result.RoomTypeCoefs.AddRange(roomTypes.RoomTypeCoefs.Where(c=> dbRoomTypes.Any(s=>s.Id==c.Number)).Select(c => new RoomTypeCoefViewModel { RoomTypeCode = dict[c.Number], Coef = c.Coef, PeopleNum = c.PeopleNum }).ToList());

            if (roomTypes.ChildRooms != null && roomTypes.ChildRooms.Any())
                result.ChildRoomTypes = roomTypes.ChildRooms.Where(c => dbRoomTypes.Any(s => s.Id == c.Child)&& dbRoomTypes.Any(s=>s.Id==c.Parent)).Select(c => new ChildRoomTypeViewModel { Child = dict[c.Child], Quantity = c.Quantity, Parent = dict[c.Parent] }).ToList();
            if (roomTypes.PriceConstraints != null && roomTypes.PriceConstraints.Any())
                result.PriceConstraints = roomTypes.PriceConstraints.Where(c => dbRoomTypes.Any(s => s.Id == c.Less) && dbRoomTypes.Any(s => s.Id == c.More)).Select(c => new PriceConstraint { Less = dict[c.Less], More = dict[c.More] }).ToList();
           
            foreach (var roomtype in dbRoomTypes)
            {
                var index = result.RoomTypes.FindIndex(c => c.Number == roomtype.Id);
                if (index >= 0)
                {
                    result.RoomTypes[index].Name = roomtype.Name;
                    result.RoomTypes[index].RoomTypeCode = roomtype.RoomTypeCode;
                }
                else
                    result.RoomTypes.Add(new RoomTypeViewModel { Number = roomtype.Id, Name = roomtype.Name, RoomTypeCode = roomtype.RoomTypeCode });
            }
            return result;
        }
        public static RoomTypes Map(RoomTypesViewModel roomTypes, IEnumerable<RoomType> dbRoomTypes)
        {
            var result = new RoomTypes();
            
            if (roomTypes == null )
                return result;
            if (roomTypes.RoomTypes != null && roomTypes.RoomTypes.Any())
                result.RoomTypeDescriptions = roomTypes.RoomTypes.Select(c => new RoomTypeDescription { Description = c.Description, LowerBound = c.LowerBound, MarketBasePrice = c.MarketBasePrice, PeopleNum = c.PeopleNum, Number = (dbRoomTypes.FirstOrDefault(x=>x.RoomTypeCode== c.RoomTypeCode)?.Id)??0, OperationalCost = c.OperationalCost, Quantity = c.Quantity, UpperBound = c.UpperBound }).ToList();
            if (roomTypes.ChildRoomTypes != null && roomTypes.ChildRoomTypes.Any())
                result.ChildRooms = roomTypes.ChildRoomTypes.Select(c => new ChildRooms { Child = (dbRoomTypes.FirstOrDefault(x => x.RoomTypeCode == c.Child)?.Id) ?? 0, Quantity = c.Quantity, Parent = (dbRoomTypes.FirstOrDefault(x => x.RoomTypeCode == c.Parent)?.Id) ?? 0 }).ToList();
            if (roomTypes.PriceConstraints != null && roomTypes.PriceConstraints.Any())
                result.PriceConstraints = roomTypes.PriceConstraints.Select(c => new Hotels.Config.ConfigModel.PriceConstraint { Less = (dbRoomTypes.FirstOrDefault(x => x.RoomTypeCode == c.Less)?.Id) ?? 0, More = (dbRoomTypes.FirstOrDefault(x => x.RoomTypeCode == c.More)?.Id) ?? 0 }).ToList();
            if (roomTypes.RoomTypeCoefs != null && roomTypes.RoomTypeCoefs.Any())
                result.RoomTypeCoefs = roomTypes.RoomTypeCoefs.Select(c => new RoomTypeCoef { Number = (dbRoomTypes.FirstOrDefault(x => x.RoomTypeCode == c.RoomTypeCode)?.Id) ?? 0, PeopleNum = c.PeopleNum, Coef=c.Coef }).ToList();
            result.Total = result.RoomTypeDescriptions.Count();
            return result;
        }

    }
}