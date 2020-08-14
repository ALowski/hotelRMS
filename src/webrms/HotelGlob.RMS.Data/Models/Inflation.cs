using System;

namespace HotelGlob.RMS.Data.Models
{
    public class Inflation
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double Coef { get; set; }
        public int CountryId { get; set; }
    }
}
