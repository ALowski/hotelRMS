using Hangfire;
using HotelGlob.RMS.Data;
using Microsoft.Owin;
using Owin;
using System.Linq;
using HotelGlob.RMS.HangfireWeb.Tasks;

//using Hangfire;

[assembly: OwinStartup(typeof(HotelGlob.RMS.HangfireWeb.Startup))]
namespace HotelGlob.RMS.HangfireWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            GlobalConfiguration.Configuration.UseSqlServerStorage("HangfireConnection");

            app.UseHangfireDashboard();
            var options = new BackgroundJobServerOptions { WorkerCount = 1 };
            app.UseHangfireServer(options);

            RunHotelsJob();
        }

        public void RunHotelsJob()
        {
            using (var context = new RmsDbContext())
            {
                var hotels = context.HotelSettings.Where(h => h.IsRmsEnalbed).ToList();
                foreach (var hotel in hotels)
                {
                    RecurringJob.AddOrUpdate<CalculateHotelPredictionTask>(CalculateHotelPredictionTask.GetJobName(hotel.Id), t => t.Run(hotel.Id), Cron.Minutely);
                    RecurringJob.AddOrUpdate<BookingParserHotelTask>(BookingParserHotelTask.GetJobName(hotel.Id), t => t.Run(hotel.Id), Cron.Daily);
                }
            }
        }
    }
}
