using HotelGlob.RMS.Data;
using HotelGlob.RMS.Data.Models;
using HotelGlob.RMS.Web.Models.Configuration;
using Hotels.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace HotelGlob.RMS.Web.Controllers
{
    public class ConfigurationController : Controller
    {
        private RmsDbContext db = new RmsDbContext();
        // GET: Configuration
        public async Task<ActionResult> Index(int hotelId)
        {
            ViewBag.HotelId = hotelId;
            HotelSettings hotel =await db.HotelSettings.FindAsync(hotelId);
            var roomTypes = db.RoomTypes.Where(c=>c.HotelId==hotelId).ToList();
            var mealTypes = db.MealTypes.ToList();
            DefoConfiguration conf = new DefoConfiguration(hotel.Settings);
            var model = ConfigurationViewModel.Map(conf.ConfigurationRoot, roomTypes);
            model.RoomTypes.IsEditable = !hotel.IsSettingsBlocked;
            ViewBag.DbMealTypes = new SelectList(mealTypes, "Id", "Name");
            ViewBag.DbRoomTypes = new SelectList(roomTypes, "RoomTypeCode", "Name");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(int hotelId, ConfigurationViewModel config)
        {
            var roomTypes = db.RoomTypes.Where(c => c.HotelId == hotelId).ToList();
            if (ModelState.IsValid)
            {
                if (config.RoomTypes.IsEditable)
                {
                    var newRoomTypes = config.RoomTypes.RoomTypes.Where(c => c.Number == 0).ToList();
                    if (newRoomTypes.Any())
                        db.RoomTypes.AddRange(newRoomTypes.Select(c => new RoomType { Name = c.Name, RoomTypeCode = c.RoomTypeCode, HotelId = hotelId }));
                    db.RoomTypes.RemoveRange(roomTypes.Where(c =>! config.RoomTypes.RoomTypes.Any(s => s.Number == c.Id)));
                    foreach (var item in config.RoomTypes.RoomTypes.Where(c => c.Number > 0).ToList())
                    {
                        var entity = roomTypes.FirstOrDefault(c => c.Id == item.Number);
                        if (entity != null)
                        {
                            entity.RoomTypeCode = item.RoomTypeCode;
                            entity.Name = item.Name;
                            db.Entry(entity).State = EntityState.Modified;
                        }
                    }
                    await db.SaveChangesAsync();
                    roomTypes = db.RoomTypes.Where(c => c.HotelId == hotelId).ToList();
                }
                
                HotelSettings hotel = await db.HotelSettings.FindAsync(hotelId);
                if (config.MealTypes.UseInDynamicCalculation)
                    config.RoomTypes.RoomTypeCoefs.Clear();
                hotel.Settings = JsonConvert.SerializeObject(ConfigurationViewModel.Map(config, roomTypes), Formatting.Indented);
                db.Entry(hotel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                
                return RedirectToAction("Index","HotelSettings");
            }
            ViewBag.HotelId = hotelId;             
            var mealTypes = db.MealTypes.ToList();
            ViewBag.DbMealTypes = new SelectList(mealTypes, "Id", "Name");
            ViewBag.DbRoomTypes = new SelectList(roomTypes, "RoomTypeCode", "Name");
            return View(config);
        }
    }
}