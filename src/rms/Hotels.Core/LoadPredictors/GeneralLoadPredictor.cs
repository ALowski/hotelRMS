using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hotels.Basic;

namespace Hotels.Core.LoadPredictors
{
    public class GeneralLoadPredictor: ILoadPredictor
    {
        private HoltLoadPredicrtor hlp = new HoltLoadPredicrtor();
        private MovingAverageLoadPredictor malp = new MovingAverageLoadPredictor();
        private SameDayLoadPredictor sdlp = new SameDayLoadPredictor();

        private double[] _price;
        private double[] _forecast;
        private int _planningHorizont;
        
        public void Initialize(DateTime today, bool[] pastDays, bool[] futureDays, int planningHorizont)
        {
            _planningHorizont = planningHorizont;
            _forecast = new double[planningHorizont];
            hlp.Initialize(today, pastDays, futureDays, planningHorizont);
            malp.Initialize(today,pastDays,futureDays,planningHorizont);
            if(planningHorizont>90)
                sdlp.Initialize(today, pastDays, futureDays, planningHorizont);
        }


        public void AddBooking(Booking booking)
        {
            hlp.AddBooking(booking);
            malp.AddBooking(booking);
            if (_planningHorizont > 90)
                sdlp.AddBooking(booking);
        }


        public double[] Calculate(double[] forecast, double[] price)
        {
            var res = hlp.Calculate(forecast, price);
            if (res == null)
                price=malp.Calculate(forecast, price);
            if (_planningHorizont > 90)
                _price = sdlp.Calculate(forecast, price);
            _forecast = forecast;
            _price = price;
            return _price;
        }

        public double[] GetForecast()
        {
            return _forecast;
        }
        public double[] GetPrice()
        {
            return _price;
        }
    }
}
