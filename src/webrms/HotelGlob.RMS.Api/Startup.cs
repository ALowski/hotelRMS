using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using System.Diagnostics;
using AutoMapper;
using HotelGlob.RMS.Data.Models;
using HotelGlob.RMS.Api.Models;

[assembly: OwinStartup(typeof(HotelGlob.RMS.Api.Startup))]

namespace HotelGlob.RMS.Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);

            SwaggerConfig.Register(config);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<ReservationViewModel, Reservation>();
                cfg.CreateMap<InflationViewModel, Inflation>();
                cfg.CreateMap<string, DateTime>()
                .ConvertUsing(src => DateTime.ParseExact(src, "dd.MM.yyyy HH:mm:ss", null));
            });

                //app.Use(OwinTest);

            app.UseWebApi(config);
        }

        //public async Task OwinTest(IOwinContext context, Func<Task> next)
        //{
        //    var startTime = DateTime.UtcNow;
        //    var watch = Stopwatch.StartNew();
        //    await next();
        //    watch.Stop();
        //    var logTemplate = $"Client IP: {context.Request.RemoteIpAddress} Request path: {context.Request.Path} Request content type: {context.Request.ContentType} Start time: {startTime} Duration: {watch.ElapsedMilliseconds}";
        //    Trace.WriteLine(logTemplate);
        //}
    }
}
