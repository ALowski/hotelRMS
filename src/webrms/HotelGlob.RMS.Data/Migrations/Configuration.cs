using System;
using HotelGlob.RMS.Data.Models;
using System.Data.Entity.Migrations;

namespace HotelGlob.RMS.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<RmsDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RmsDbContext context)
        {
            //    //  This method will be called after migrating to the latest version.

            //    //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //    //  to avoid creating duplicate seed data. E.g.
            //    //
            //    context.Hotels.AddOrUpdate(
            //      h => h.Name,
            //      new Hotel { Name = "Hotel 1",IsRmsEnalbed=true, CountryId=1 },
            //      new Hotel { Name = "Hotel 2" },
            //      new Hotel { Name = "Hotel 3" },
            //      new Hotel { Name = "Hotel 4" },
            //      new Hotel { Name = "Hotel 5" },
            //      new Hotel { Name = "Hotel 6" }, 
            //      new Hotel { Name = "Hotel 7" },
            //      new Hotel { Name = "Hotel 8" },
            //      new Hotel { Name = "Hotel 9" }, 
            //      new Hotel { Name = "Hotel 10" },
            //      new Hotel { Name = "Hotel 11" },
            //      new Hotel { Name = "Hotel 12" }
            //    );

            //    context.Inflations.AddOrUpdate(
            //        i => i.Date,
            //        new Inflation {Date = new DateTime(2014, 12, 01), Coef = 102.24, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 01, 01), Coef = 104.24, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 02, 01), Coef = 98.56, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 03, 01), Coef = 100.75, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 04, 01), Coef = 101.24, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 05, 01), Coef = 100.32, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 06, 01), Coef = 98.99, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 07, 01), Coef = 100.7, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 08, 01), Coef = 98.32, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 09, 01), Coef = 105.48, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 10, 01), Coef = 100.43, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 11, 01), Coef = 100.21, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2015, 12, 01), Coef = 100.08, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 01, 01), Coef = 99.31, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 02, 01), Coef = 101.74, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 03, 01), Coef = 99.98, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 04, 01), Coef = 101.25, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 05, 01), Coef = 99.68, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 06, 01), Coef = 101.12, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 07, 01), Coef = 99.27, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 08, 01), Coef = 99.79, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 09, 01), Coef = 101.97, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 10, 01), Coef = 100.6, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 11, 01), Coef = 100.4, CountryId = 1 },
            //        new Inflation {Date = new DateTime(2016, 12, 01), Coef = 99.5, CountryId = 1 });

            //    context.SaveChanges();
            }
        }
}
