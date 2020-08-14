using System.Collections.Generic;

namespace HotelGlob.RMS.Data.Models
{
    public class HotelSettings : Entity
    {
        public string Name { get; set; }
        public string BookingName { get; set; }
        public double BookingRating { get; set; }
        public int PlanningHorizon { get; set; }
        public int HistoryPeriod { get; set; }
        public string Settings { get; set; }
        public int CountryId { get; set; }
        public bool IsRmsEnalbed { get; set; }
        public bool IsNeedRecalc { get; set; }
        public bool IsSettingsBlocked { get; set; }
        public int HotelId { get; set; }

        public virtual ICollection<Calculation> Calculations { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Log> Logs { get; set; }
    }
}
