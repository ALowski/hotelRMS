using HotelGlob.RMS.Data.Models;
using Hotels.Config.ConfigModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class ConfigurationViewModel
    {
        public List<SeasonViewModel> Seasons { get; set; }
        public WeekdaysViewModel Weekdays { get; set; }
        public RoomTypesViewModel RoomTypes { get; set; }
        public MealTypesViewModel MealTypes { get; set; }
        public CategoriesViewModel Categories { get; set; }

        public static ConfigurationViewModel Map(ConfigurationRoot config, IEnumerable<RoomType> roomTypes)
        {
            return new ConfigurationViewModel { Seasons = SeasonViewModel.Map(config.Seasons),Weekdays=WeekdaysViewModel.Map(config.Weekdays),RoomTypes= RoomTypesViewModel.Map(config.RoomTypes, roomTypes), MealTypes = MealTypesViewModel.Map(config.MealTypes) , Categories =CategoriesViewModel.Map(config.Categories)};
        }
        public static ConfigurationRoot Map(ConfigurationViewModel config, IEnumerable<RoomType> roomTypes)
        {
            return new ConfigurationRoot { Seasons = SeasonViewModel.Map(config.Seasons),Weekdays=WeekdaysViewModel.Map(config.Weekdays), RoomTypes = RoomTypesViewModel.Map(config.RoomTypes, roomTypes), MealTypes=MealTypesViewModel.Map(config.MealTypes), Categories = CategoriesViewModel.Map(config.Categories) };
        }
    }
}