using HotelGlob.RMS.Data.Models;
using System.Data.Entity;

namespace HotelGlob.RMS.Data
{
    public class RmsDbContext : DbContext
    {
        public RmsDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<HotelSettings> HotelSettings { get; set; }
        public DbSet<Calculation> Calculations { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Inflation> Inflations { get; set; }
        public DbSet<Parser_RoomInfo> Parser_RoomInfos { get; set; }
        public DbSet<Parser_RoomData> Parser_RoomDatas { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<MealType> MealTypes { get; set; }
        public DbSet<Log> Logs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<HotelSettings>().HasKey(h => h.Id);

            modelBuilder.Entity<HotelSettings>()
                .HasMany(h => h.Reservations)
                .WithRequired(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<HotelSettings>()
                .HasMany(h => h.Calculations)
                .WithRequired(r => r.Hotel)
                .HasForeignKey(r => r.HotelId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Calculation>()
                .HasMany(h => h.Predictions)
                .WithRequired(r => r.Calculation)
                .HasForeignKey(r => r.CalculationId)
                .WillCascadeOnDelete(true);
        }

        public System.Data.Entity.DbSet<HotelGlob.RMS.Data.Models.Event> Events { get; set; }
    }
}
