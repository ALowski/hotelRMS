using HotelGlob.RMS.Data;
using HotelGlob.RMS.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelGlob.RMS.BL
{
    public class DBManager
    {
        public async Task<bool> InsertReservations(IEnumerable<Reservation> reservations,int hotelId)
        {
            using (var context = new RmsDbContext())
            {
                HotelSettings hotel = await context.HotelSettings.FindAsync(hotelId);

                if (hotel == null)
                {
                    return false;
                }

                DateTime createdOn = DateTime.Now;
                reservations.ToList().ForEach(f =>
                {
                    f.HotelId = hotelId;
                    f.CreatedOn = createdOn;
                });
                while (reservations.Any())
                {
                    context.Reservations.AddRange(reservations.Take(250));
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    finally
                    {
                        reservations = reservations.Skip(250).ToList();
                    }
                }
                hotel.IsNeedRecalc = true;
                return true;
            }
        }
        public async Task<bool> InsertEvents(IEnumerable<Event> events)
        {
            using (var context = new RmsDbContext())
            {
                if (events.Any())
                {
                    context.Events.AddRange(events);
                    HotelSettings hotel = await context.HotelSettings.FindAsync(events.FirstOrDefault().HotelId);
                    hotel.IsNeedRecalc = true;
                    await context.SaveChangesAsync();
                }
                return true;
            }
        }
        public async Task<bool> UpdateEvents(IEnumerable<Event> events)
        {
            using (var context = new RmsDbContext())
            {
                if (events.Any())
                {
                    foreach(var item in events)
                        context.Entry(item).State = EntityState.Modified;
                    HotelSettings hotel = await context.HotelSettings.FindAsync(events.FirstOrDefault().HotelId);
                    hotel.IsNeedRecalc = true;
                    await context.SaveChangesAsync();
                }
                return true;
            }
        }
        public async Task<bool> DeleteEvents(IEnumerable<int> ids)
        {
            using (var context = new RmsDbContext())
            {
                var events = await context.Events.Where(c=>ids.Contains(c.Id)).ToListAsync();
                if (events.Any())
                {
                    HotelSettings hotel = await context.HotelSettings.FindAsync(events.FirstOrDefault().HotelId);
                    context.Events.RemoveRange(events);
                    hotel.IsNeedRecalc = true;
                    await context.SaveChangesAsync();
                }
                return true;
            }
        }
        
    }
}
