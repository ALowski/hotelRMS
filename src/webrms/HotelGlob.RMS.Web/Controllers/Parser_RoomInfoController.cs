using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HotelGlob.RMS.Data;
using HotelGlob.RMS.Data.Models;
using Hotels.Config;

namespace HotelGlob.RMS.Web.Controllers
{
    public class Parser_RoomInfoController : Controller
    {
        private RmsDbContext db = new RmsDbContext();

        // GET: Parser_RoomInfo
        public async Task<ActionResult> Index(int hotelId)
        {
            var parser_RoomInfos = db.Parser_RoomInfos.Where(c => c.HotelId == hotelId).Include(p => p.Hotel);
            ViewBag.HotelId = hotelId;
            HotelSettings hotel = await db.HotelSettings.FindAsync(hotelId);
            DefoConfiguration conf = new DefoConfiguration(hotel.Settings);
            ViewBag.Config = conf;
            return View(await parser_RoomInfos.ToListAsync());
        }

        // GET: Parser_RoomInfo/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parser_RoomInfo parser_RoomInfo = await db.Parser_RoomInfos.FindAsync(id);
            if (parser_RoomInfo == null)
            {
                return HttpNotFound();
            }
            return View(parser_RoomInfo);
        }

        // GET: Parser_RoomInfo/Create
        public ActionResult Create(int hotelId)
        {
            ViewBag.HotelId = hotelId;
            HotelSettings hotel = db.HotelSettings.Find(hotelId);
            DefoConfiguration conf = new DefoConfiguration(hotel.Settings);
            var roomTypes = db.RoomTypes.Where(c => c.HotelId == hotelId).ToList();
            ViewBag.RoomTypes = new SelectList(roomTypes, "Id", "Name");

            if (conf?.ConfigurationRoot?.MealTypes?.MealTypeDescriptions != null)
            {
                var mealIds = conf?.ConfigurationRoot?.MealTypes?.MealTypeDescriptions.Select(c => c.Number);
                if (mealIds.Any())
                {
                    var mealTypes = db.MealTypes.Where(c => mealIds.Contains(c.Id)).ToList();
                    ViewBag.MealTypes = new SelectList(mealTypes, "Id", "Name");
                }
            }
                //ViewBag.MealTypes = conf?.ConfigurationRoot?.MealTypes?.MealTypeDescriptions.ToDictionary(c => c.Number.ToString(), c => c.DisplayName);
            return View();
        }

        // POST: Parser_RoomInfo/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,RoomTypeId,MealTypeId,HotelId,RoomName,PeopleNum,Cancelation,Prepayment,City,HotelName,MealName")] Parser_RoomInfo parser_RoomInfo)
        {
            if (ModelState.IsValid)
            {
                db.Parser_RoomInfos.Add(parser_RoomInfo);
                await db.SaveChangesAsync();
                return RedirectToAction("Index" , new { hotelId = parser_RoomInfo.HotelId });
            }

            ViewBag.HotelId = parser_RoomInfo.HotelId;
            HotelSettings hotel = db.HotelSettings.Find(parser_RoomInfo.HotelId);
            DefoConfiguration conf = new DefoConfiguration(hotel.Settings);
            var roomTypes = db.RoomTypes.Where(c => c.HotelId == parser_RoomInfo.HotelId).ToList();
            ViewBag.RoomTypes = new SelectList(roomTypes, "Id", "Name");

            if (conf?.ConfigurationRoot?.MealTypes?.MealTypeDescriptions != null)
            {
                var mealIds = conf?.ConfigurationRoot?.MealTypes?.MealTypeDescriptions.Select(c => c.Number);
                if (mealIds.Any())
                {
                    var mealTypes = db.MealTypes.Where(c => mealIds.Contains(c.Id)).ToList();
                    ViewBag.MealTypes = new SelectList(mealTypes, "Id", "Name");
                }
            }
            return View(parser_RoomInfo);
        }

        // GET: Parser_RoomInfo/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parser_RoomInfo parser_RoomInfo = await db.Parser_RoomInfos.FindAsync(id);
            if (parser_RoomInfo == null)
            {
                return HttpNotFound();
            }
            DefoConfiguration conf = new DefoConfiguration(parser_RoomInfo.Hotel.Settings);
            var roomTypes = db.RoomTypes.Where(c => c.HotelId == parser_RoomInfo.HotelId).ToList();
            ViewBag.RoomTypes = new SelectList(roomTypes, "Id", "Name");

            if (conf?.ConfigurationRoot?.MealTypes?.MealTypeDescriptions != null)
            {
                var mealIds = conf?.ConfigurationRoot?.MealTypes?.MealTypeDescriptions.Select(c => c.Number);
                if (mealIds.Any())
                {
                    var mealTypes = db.MealTypes.Where(c => mealIds.Contains(c.Id)).ToList();
                    ViewBag.MealTypes = new SelectList(mealTypes, "Id", "Name");
                }
            }
            return View(parser_RoomInfo);
        }

        // POST: Parser_RoomInfo/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,RoomTypeId,MealTypeId,HotelId,RoomName,PeopleNum,Cancelation,Prepayment,City,HotelName,MealName")] Parser_RoomInfo parser_RoomInfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(parser_RoomInfo).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { hotelId = parser_RoomInfo.HotelId });
            }
            HotelSettings hotel = db.HotelSettings.Find(parser_RoomInfo.HotelId);
            DefoConfiguration conf = new DefoConfiguration(hotel.Settings);
            var roomTypes = db.RoomTypes.Where(c => c.HotelId == parser_RoomInfo.HotelId).ToList();
            ViewBag.RoomTypes = new SelectList(roomTypes, "Id", "Name");

            if (conf?.ConfigurationRoot?.MealTypes?.MealTypeDescriptions != null)
            {
                var mealIds = conf?.ConfigurationRoot?.MealTypes?.MealTypeDescriptions.Select(c => c.Number);
                if (mealIds.Any())
                {
                    var mealTypes = db.MealTypes.Where(c => mealIds.Contains(c.Id)).ToList();
                    ViewBag.MealTypes = new SelectList(mealTypes, "Id", "Name");
                }
            }
            return View(parser_RoomInfo);
        }

        // GET: Parser_RoomInfo/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Parser_RoomInfo parser_RoomInfo = await db.Parser_RoomInfos.FindAsync(id);
            if (parser_RoomInfo == null)
            {
                return HttpNotFound();
            }
            return View(parser_RoomInfo);
        }

        // POST: Parser_RoomInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Parser_RoomInfo parser_RoomInfo = await db.Parser_RoomInfos.FindAsync(id);
            var hotelId = parser_RoomInfo.HotelId;
            db.Parser_RoomInfos.Remove(parser_RoomInfo);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { hotelId = hotelId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
