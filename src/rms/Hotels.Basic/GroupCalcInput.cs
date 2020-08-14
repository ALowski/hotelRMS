using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels.Basic
{
    public class GroupCalcInput
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Start { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime End { get; set; }
        public int HotelId { get; set; }
        public List<GroupCalcDayInput> Inputs { get; set; }
        public string GetInputsJSON()
        {
            return JsonConvert.SerializeObject(Inputs);
        }
    }
    public struct GroupCalcDayInput
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        public Dictionary<string, RoomInfo> RoomTypes { get; set; }
        public Dictionary<string, MealInfo> MealTypes { get; set; }
    }
    public struct RoomInfo
    {
        public int Value{get;set;}
        public int EmptyRooms {get;set;}
        public int PlanEmptyRooms { get; set; }
        public string Name { get; set; }
    }
    public struct MealInfo
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }
}
