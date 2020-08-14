using System;
using AutoMapper;
using HotelGlob.RMS.Api.Models;
using HotelGlob.RMS.Data;
using HotelGlob.RMS.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;

namespace HotelGlob.RMS.Api.Controllers
{
    public class InflationsController : ApiController
    {
        /// <summary>
        /// Post inflations
        /// </summary>
        public async Task<IHttpActionResult> Post( [FromBody]InflationViewModel[] InflationViewModels)
        {
            using (var context = new RmsDbContext())
            {
                var inflations = Mapper.Map<List<Inflation>>(InflationViewModels);
                inflations.ForEach(s => context.Inflations.RemoveRange( context.Inflations.Where(c => c.CountryId == s.CountryId && c.Date == s.Date)));

                while (inflations.Any())
                {
                    context.Inflations.AddRange(inflations.Take(250));
                    try
                    {
                        await context.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        inflations = inflations.Skip(250).ToList();
                    }
                }

            }

            return Ok();
        }
    }
}