using System;
using AutoMapper;
using HotelGlob.RMS.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using HotelGlob.RMS.BL;
using HotelGlob.RMS.Data.Models;

namespace HotelGlob.RMS.Api.Controllers
{
    public class ReservationsController : ApiController
    {
        /// <summary>
        /// Post reservations
        /// </summary>
        /// <param name="hotelId">Hotel identifier</param>
        public async Task<IHttpActionResult> Post([FromUri]int hotelId, [FromBody]ReservationViewModel[] reservationViewModels)
        {
            var manager = new DBManager();
                var reservations = Mapper.Map<List<Reservation>>(reservationViewModels);
            var result = await manager.InsertReservations(reservations, hotelId);
            if (result)
                return Ok();
            else
                return InternalServerError();
        }
    }
}
