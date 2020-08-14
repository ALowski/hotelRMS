using HotelGlob.RMS.Data;
using HotelGlob.RMS.Data.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using System;
using Hotels.Config;
using System.Collections.Generic;
using System.Text;

namespace HotelGlob.RMS.Web.Controllers
{
    public class CalculationsController : Controller
    {
        private RmsDbContext _db = new RmsDbContext();

        // GET: Calculations
        public async Task<ActionResult> Index(int id)
        {
            HotelSettings hotel = await _db.HotelSettings.FindAsync(id);

            if (hotel == null)
            {
                return HttpNotFound("The hotel not found.");
            }

            var calculations = await GetCalculations(0, id);
            DefoConfiguration conf = new DefoConfiguration(hotel.Settings);
            var total = conf.ConfigurationRoot.Categories.BookingPeriods.Any() ? conf.ConfigurationRoot.Categories.BookingPeriods.Count() : 1;
            Dictionary<int, string> stayNames = new Dictionary<int, string>();
            Dictionary<int, string> bookingNames = new Dictionary<int, string>();
            for (int i = 0; i < conf.ConfigurationRoot.Categories.Total; i++)
            {
                int l = (i / total);
                int b = i - l * total;
                var bp = conf.ConfigurationRoot.Categories.BookingPeriods?.FirstOrDefault(c => c.Number == b);
                var sp = conf.ConfigurationRoot.Categories.StayPeriods?.FirstOrDefault(c => c.Number == l);
                bookingNames.Add(i, bp?.LowerBound + "-" + bp?.UpperBound);
                stayNames.Add(i, sp?.LowerBound + "-" + sp?.UpperBound);
            }
            ViewBag.Id = id;
            ViewBag.Num = 0;
            ViewBag.StayNames = stayNames;
            ViewBag.BookingNames = bookingNames;
            return View(calculations);
        }

        private async Task<List<Calculation>> GetCalculations(int num, int id)
        {
            var from = DateTime.Now.Date.AddDays(7 * num);
            var to = DateTime.Now.Date.AddDays(7 * (num + 1));
            
            var queryResult = await _db.Calculations
                .Include(c => c.Predictions)
                .Where(c => c.HotelId == id && c.PredictionDate>= from && c.PredictionDate < to).ToListAsync();
            return await Task.Run(() =>
                    {
                           return queryResult.GroupBy(c => c.PredictionDate.Date).Select(c => c.OrderBy(s => s.CalculatedOn).LastOrDefault()).ToList();
                    });
        }
        
        public ActionResult GetCsv(int hotelId)
        {
            HotelSettings hotel = _db.HotelSettings.Find(hotelId);
            DefoConfiguration conf = new DefoConfiguration(hotel.Settings);
            var total = conf.ConfigurationRoot.Categories.BookingPeriods.Any() ? conf.ConfigurationRoot.Categories.BookingPeriods.Count() : 1;
            Dictionary<int, string> stayNames = new Dictionary<int, string>();
            Dictionary<int, string> bookingNames = new Dictionary<int, string>();
            for (int i = 0; i < conf.ConfigurationRoot.Categories.Total; i++)
            {
                int l = (i / total);
                int b = i - l * total;
                var bp = conf.ConfigurationRoot.Categories.BookingPeriods?.FirstOrDefault(c => c.Number == b);
                var sp = conf.ConfigurationRoot.Categories.StayPeriods?.FirstOrDefault(c => c.Number == l);
                bookingNames.Add(i, bp?.LowerBound + " - " + bp?.UpperBound);
                stayNames.Add(i, sp?.LowerBound + " - " + sp?.UpperBound);
            }
            var queryResult = _db.Calculations
               .Include(c => c.Predictions)
               .Where(c => c.HotelId == hotelId).ToList();
            
            var calcs=queryResult.GroupBy(c => c.PredictionDate.Date).Select(c => c.OrderBy(s => s.CalculatedOn).LastOrDefault()).ToList();
            var sb = new StringBuilder();
            sb.AppendLine(string.Join(";", new string[] { "Date","Room Type", "Meal Type", "Stay period", "Booking period", "People Number", "Price", "Expected Load", "Cancelation", "No Shows" }));
            foreach (var calc in calcs)
            {
                foreach(var pred in calc.Predictions)
                    sb.AppendLine(string.Join(";",new string[]{ calc.PredictionDate.ToShortDateString(),pred.RoomType.Name,pred.MealType.Name, stayNames[pred.CategoryType],bookingNames[pred.CategoryType],pred.PeopleNum.ToString(), pred.Price.ToString(), pred.ExpectedLoad.ToString(),pred.Cancelation.ToString(), pred.NoShows.ToString() }));
            }
            return File(Encoding.Default.GetBytes(sb.ToString()), System.Net.Mime.MediaTypeNames.Application.Octet, "ResultsCsv.csv");            
        }
        [HttpGet]
        public async Task<ActionResult> PredictionsOutputList(int num,int id)
        {
            HotelSettings hotel = await _db.HotelSettings.FindAsync(id);

            if (hotel == null)
            {
                return HttpNotFound("The hotel not found.");
            }
            var calculations = await GetCalculations(num, id);
            DefoConfiguration conf = new DefoConfiguration(hotel.Settings);
            var total = conf.ConfigurationRoot.Categories.BookingPeriods.Any() ? conf.ConfigurationRoot.Categories.BookingPeriods.Count() : 1;
            Dictionary<int, string> stayNames = new Dictionary<int, string>();
            Dictionary<int, string> bookingNames = new Dictionary<int, string>();
            for (int i = 0; i < conf.ConfigurationRoot.Categories.Total; i++)
            {
                int l = (i / total);
                int b = i - l * total;
                var bp = conf.ConfigurationRoot.Categories.BookingPeriods?.FirstOrDefault(c => c.Number == b);
                var sp = conf.ConfigurationRoot.Categories.StayPeriods?.FirstOrDefault(c => c.Number == l);
                bookingNames.Add(i, bp?.LowerBound + (bp?.UpperBound > 365 ? "+" : ("-" + bp?.UpperBound)));
                stayNames.Add(i, sp?.LowerBound + (sp?.UpperBound>365?"+":("-" + sp?.UpperBound)));
            }
            ViewBag.StayNames = stayNames;
            ViewBag.BookingNames = bookingNames;
            ViewBag.Num = num;
            ViewBag.Id = id;
            return PartialView("PredictionsOutputList", calculations);
        }

        public async Task<ActionResult> Details(int id)
        {
            var calculation = await this._db.Calculations.Include(c => c.Predictions).FirstOrDefaultAsync(c => c.Id == id);
            return View(calculation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}