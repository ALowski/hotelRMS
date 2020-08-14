namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeChanges3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes");
            DropForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes");
            DropIndex("dbo.Reservations", new[] { "MealTypeId" });
            DropIndex("dbo.Predictions", new[] { "MealTypeId" });
            AddColumn("dbo.Hotels", "BookingName", c => c.String());
            AddColumn("dbo.Hotels", "BookingRating", c => c.Double(nullable: false));
            AddColumn("dbo.Hotels", "PlanningHorizon", c => c.Int(nullable: false));
            AddColumn("dbo.Hotels", "HistoryPeriod", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "RoomPrice", c => c.Double(nullable: false));
            AddColumn("dbo.Reservations", "MealPrice", c => c.Double(nullable: false));
            AddColumn("dbo.Parser_RoomData", "Rating", c => c.Double(nullable: false));
            AlterColumn("dbo.Reservations", "MealTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Predictions", "MealTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Reservations", "MealTypeId");
            CreateIndex("dbo.Predictions", "MealTypeId");
            AddForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes", "Id", cascadeDelete: true);
            DropColumn("dbo.Reservations", "PricePerNight");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservations", "PricePerNight", c => c.Double(nullable: false));
            DropForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes");
            DropForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes");
            DropIndex("dbo.Predictions", new[] { "MealTypeId" });
            DropIndex("dbo.Reservations", new[] { "MealTypeId" });
            AlterColumn("dbo.Predictions", "MealTypeId", c => c.Int());
            AlterColumn("dbo.Reservations", "MealTypeId", c => c.Int());
            DropColumn("dbo.Parser_RoomData", "Rating");
            DropColumn("dbo.Reservations", "MealPrice");
            DropColumn("dbo.Reservations", "RoomPrice");
            DropColumn("dbo.Hotels", "HistoryPeriod");
            DropColumn("dbo.Hotels", "PlanningHorizon");
            DropColumn("dbo.Hotels", "BookingRating");
            DropColumn("dbo.Hotels", "BookingName");
            CreateIndex("dbo.Predictions", "MealTypeId");
            CreateIndex("dbo.Reservations", "MealTypeId");
            AddForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes", "Id");
            AddForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes", "Id");
        }
    }
}
