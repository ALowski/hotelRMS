using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using HotelGlob.RMS.Data;
using HotelGlob.RMS.Data.Models;

namespace HotelGlob.RMS.Web.Controllers
{
    public class HotelSettingsController : Controller
    {
        private RmsDbContext db = new RmsDbContext();

        // GET: Hotels
        public async Task<ActionResult> Index()
        {
            return View(await db.HotelSettings.ToListAsync());
        }

        // GET: Hotels/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelSettings hotel = await db.HotelSettings.FindAsync(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        // GET: Hotels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Hotels/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Settings,IsRmsEnalbed")] HotelSettings hotel)
        {
            if (ModelState.IsValid)
            {
                db.HotelSettings.Add(hotel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(hotel);
        }

        // GET: Hotels/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelSettings hotel = await db.HotelSettings.FindAsync(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Settings,IsRmsEnalbed,IsNeedRecalc")] HotelSettings hotel)
        {
            if (ModelState.IsValid)
            {
                HotelSettings hotelUp = await db.HotelSettings.FindAsync(hotel.Id);
                hotelUp.Name = hotel.Name;
                hotelUp.IsNeedRecalc = hotel.IsNeedRecalc;
                hotelUp.IsRmsEnalbed = hotel.IsRmsEnalbed;
                hotelUp.Settings = hotel.Settings;
                db.Entry(hotelUp).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(hotel);
        }

        // GET: Hotels/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HotelSettings hotel = await db.HotelSettings.FindAsync(id);
            if (hotel == null)
            {
                return HttpNotFound();
            }
            return View(hotel);
        }

        // POST: Hotels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            HotelSettings hotel = await db.HotelSettings.FindAsync(id);
            db.HotelSettings.Remove(hotel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
