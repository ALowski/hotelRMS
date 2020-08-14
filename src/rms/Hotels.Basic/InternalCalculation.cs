using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotels.Basic
{
    public class InternalCalculation
    {
        public DateTime PredictionDate { get; set; }
        public IEnumerable<InternalPrediction> Predictions { get; set; }
    }
}
