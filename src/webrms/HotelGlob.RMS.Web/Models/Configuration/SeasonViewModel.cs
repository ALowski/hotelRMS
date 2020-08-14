using Hotels.Config.ConfigModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelGlob.RMS.Web.Models.Configuration
{
    public class SeasonViewModel
    {
        public int Number { get; set; }
        public string Description { get; set; }
        [Display(Name = "Start Week")]
        public int Start { get; set; }
        [Display(Name = "Finish Week")]
        public int Finish { get; set; }
        [Display(Name = "Price Reduction")]
        public double Reduction { get; set; }
        public static List<SeasonViewModel> Map(Seasons seasons)
        {
            if(seasons.SeasonDescriptions ==null|| !seasons.SeasonDescriptions.Any())
                return new List<SeasonViewModel>();
            return seasons.SeasonDescriptions.Select(c => new SeasonViewModel { Number=c.Number,Description=c.Description,Finish=c.Finish, Start=c.Start,Reduction= ((seasons.PriceReductions?.FirstOrDefault(s=>s.Number==c.Number)==null)?1: seasons.PriceReductions.FirstOrDefault(s => s.Number == c.Number).Amount) }).ToList();
        }
        public static Seasons Map( List<SeasonViewModel> seasons)
        {
            var result = new Seasons();
            if (seasons == null || !seasons.Any())
                return result;
            var ids= seasons.Select(c => c.Number).Distinct().OrderBy(c=>c).ToList();
            result.SeasonDescriptions = seasons.Select(c=> new SeasonDescription { Number = ids.FindIndex(s => s == c.Number), Description = c.Description, Finish = c.Finish, Start = c.Start }).OrderBy(c => c.Number).ToList();
            result.PriceReductions = seasons.Where(c=>c.Reduction!=1).Select(c => new PriceReduction { Number = c.Number, Amount=c.Reduction});
            result.Total = seasons.Count();
            return result;
        }
    }
}