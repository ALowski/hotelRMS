namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "ReservationType", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "GrouplId", c => c.Int());
            AddColumn("dbo.Reservations", "EventId", c => c.Int());
            AddColumn("dbo.Predictions", "NoShows", c => c.Double(nullable: false));
            AddColumn("dbo.Predictions", "Cancelation", c => c.Double(nullable: false));
            DropColumn("dbo.Predictions", "PriceCushion");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Predictions", "PriceCushion", c => c.Double(nullable: false));
            DropColumn("dbo.Predictions", "Cancelation");
            DropColumn("dbo.Predictions", "NoShows");
            DropColumn("dbo.Reservations", "EventId");
            DropColumn("dbo.Reservations", "GrouplId");
            DropColumn("dbo.Reservations", "ReservationType");
        }
    }
}
