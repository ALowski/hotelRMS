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
using HotelGlob.RMS.BL;

namespace HotelGlob.RMS.Web.Controllers
{
    public class EventsController : Controller
    {
        private RmsDbContext db = new RmsDbContext();
        private DBManager _BLManager = new DBManager();
        public async Task<ActionResult> Index(int hotelId)
        {
            var events = db.Events.Where(c=>c.HotelId== hotelId).Include(c => c.Hotel);
            ViewBag.HotelId = hotelId;
            return View(await events.ToListAsync());
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public ActionResult Create(int hotelId)
        {
            return View(new Event { HotelId = hotelId,Start = DateTime.Today, End = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,HotelId,Name,Start,End,Coef,PriceCoef")] Event @event)
        {
            if (ModelState.IsValid)
            {
                await _BLManager.InsertEvents(new List<Event> { @event });
                return RedirectToAction("Index", new { hotelId = @event.HotelId });
            }

            //ViewBag.HotelId = new SelectList(db.Hotels, "Id", "Name", @event.HotelId);
            return View(@event);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            //ViewBag.HotelId = new SelectList(db.Hotels, "Id", "Name", @event.HotelId);
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,HotelId,Name,Start,End,Coef,PriceCoef")] Event @event)
        {
            if (ModelState.IsValid)
            {
                await _BLManager.UpdateEvents(new List<Event> { @event });
                return RedirectToAction("Index",new { hotelId= @event.HotelId });
            }
            //ViewBag.HotelId = new SelectList(db.Hotels, "Id", "Name", @event.HotelId);
            return View(@event);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = await db.Events.FindAsync(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id, int HotelId)
        {
            await _BLManager.DeleteEvents(new List<int> { id });
            return RedirectToAction("Index", new { hotelId = HotelId });
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
