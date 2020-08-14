using System;
using System.Collections.Generic;

namespace HotelGlob.RMS.Data.Models
{
    public class Calculation : Entity
    {
        public DateTime CalculatedOn { get; set; }
        public DateTime PredictionDate { get; set; }
        public string Settings { get; set; }

        public int HotelId { get; set; }
        public virtual HotelSettings Hotel { get; set; }

        public virtual ICollection<Prediction> Predictions { get; set; }
    }
}
