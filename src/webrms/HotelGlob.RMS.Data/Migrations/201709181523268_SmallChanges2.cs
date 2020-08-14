namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SmallChanges2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes");
            DropForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes");
            DropIndex("dbo.Reservations", new[] { "MealTypeId" });
            DropIndex("dbo.Predictions", new[] { "MealTypeId" });
            AddColumn("dbo.Parser_RoomData", "StayLength", c => c.Int(nullable: false));
            AlterColumn("dbo.Reservations", "MealTypeId", c => c.Int());
            AlterColumn("dbo.Predictions", "MealTypeId", c => c.Int());
            CreateIndex("dbo.Reservations", "MealTypeId");
            CreateIndex("dbo.Predictions", "MealTypeId");
            AddForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes", "Id");
            AddForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes");
            DropForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes");
            DropIndex("dbo.Predictions", new[] { "MealTypeId" });
            DropIndex("dbo.Reservations", new[] { "MealTypeId" });
            AlterColumn("dbo.Predictions", "MealTypeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Reservations", "MealTypeId", c => c.Int(nullable: false));
            DropColumn("dbo.Parser_RoomData", "StayLength");
            CreateIndex("dbo.Predictions", "MealTypeId");
            CreateIndex("dbo.Reservations", "MealTypeId");
            AddForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes", "Id", cascadeDelete: true);
        }
    }
}
