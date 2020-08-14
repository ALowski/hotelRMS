using HotelGlob.RMS.Data;
using HotelGlob.RMS.Data.Models;
using Hotels.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Hotels.Config.ConfigModel;
using Newtonsoft.Json;
using HotelGlob.RMS.Adapter;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using HotelGlob.RMS.BL;

namespace HotelGlob.RMS.Web.Controllers
{
    public class GroupCalculationController : Controller
    {
        private RmsDbContext _db = new RmsDbContext();
        private GroupCalculationBL _BLManager = new GroupCalculationBL();
        // GET: GroupCalculation
        public async Task<ActionResult> Index(int id)
        {
            HotelSettings hotel = await _db.HotelSettings.FindAsync(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            var configurationRoot = JsonConvert.DeserializeObject<ConfigurationRoot>(hotel.Settings);
           
            var rmsAdapter = await _BLManager.InitRmsAdapter(DateTime.Today, DateTime.Today, id,_db);
            var roomTypes = _db.RoomTypes.Where(c => c.HotelId == id).ToList();
            var mealTypes = _db.MealTypes.ToList();
            Session["rmsAdapter"] = rmsAdapter;
            var input = _BLManager.GetCalcInputForIndex(rmsAdapter, configurationRoot.RoomTypes.RoomTypeDescriptions.ToDictionary(c=>c.Number, c=>roomTypes.FirstOrDefault(s=>s.Id==c.Number)?.Name), configurationRoot.MealTypes.MealTypeDescriptions.ToDictionary(c=>c.Number, c=>mealTypes.FirstOrDefault(s => s.Id == c.Number)?.Name));
            input.HotelId = id;
            return View(input);
        }
        
        [HttpPost]
        public async Task<ActionResult> GetOutputPartialView(GroupCalcInput viewModel)
        {
            IEnumerable<GroupCalculationResult>  result = await
                Task.Run(() =>
            {
                var rmsAdapter = (RmsAdapter)Session["rmsAdapter"];
                if (rmsAdapter!= null)
                    return  rmsAdapter.RunGroupCalculationPriceCalculation(viewModel);
                return null;
            });
            return PartialView("GroupCalcOutputList", result);
        }
        [HttpPost]
        public async Task<ActionResult> GetInputPartialView(GroupCalcInput viewModel)
        {
            var rmsAdapter = await _BLManager.InitRmsAdapter(viewModel.Start, viewModel.End, viewModel.HotelId, _db);
            Session["rmsAdapter"] = rmsAdapter;
            ModelState.Clear();
            return PartialView("GroupCalcInputList", _BLManager.GetInputPartialView(viewModel, rmsAdapter));
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