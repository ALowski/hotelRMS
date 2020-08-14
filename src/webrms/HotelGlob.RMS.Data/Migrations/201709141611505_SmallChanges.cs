namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SmallChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "RoomTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "PeopleNum", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "MealTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Predictions", "RoomTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Predictions", "PeopleNum", c => c.Int(nullable: false));
            AddColumn("dbo.Predictions", "MealTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Reservations", "RoomTypeId");
            CreateIndex("dbo.Reservations", "MealTypeId");
            CreateIndex("dbo.Predictions", "RoomTypeId");
            CreateIndex("dbo.Predictions", "MealTypeId");
            CreateIndex("dbo.Parser_RoomInfo", "RoomTypeId");
            CreateIndex("dbo.Parser_RoomInfo", "MealTypeId");
            AddForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Reservations", "RoomTypeId", "dbo.RoomTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Predictions", "RoomTypeId", "dbo.RoomTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Parser_RoomInfo", "MealTypeId", "dbo.MealTypes", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Parser_RoomInfo", "RoomTypeId", "dbo.RoomTypes", "Id", cascadeDelete: true);
            DropColumn("dbo.Reservations", "RoomType");
            DropColumn("dbo.Reservations", "MealType");
            DropColumn("dbo.Predictions", "RoomType");
            DropColumn("dbo.Predictions", "MealType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Predictions", "MealType", c => c.Int(nullable: false));
            AddColumn("dbo.Predictions", "RoomType", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "MealType", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "RoomType", c => c.Int(nullable: false));
            DropForeignKey("dbo.Parser_RoomInfo", "RoomTypeId", "dbo.RoomTypes");
            DropForeignKey("dbo.Parser_RoomInfo", "MealTypeId", "dbo.MealTypes");
            DropForeignKey("dbo.Predictions", "RoomTypeId", "dbo.RoomTypes");
            DropForeignKey("dbo.Predictions", "MealTypeId", "dbo.MealTypes");
            DropForeignKey("dbo.Reservations", "RoomTypeId", "dbo.RoomTypes");
            DropForeignKey("dbo.Reservations", "MealTypeId", "dbo.MealTypes");
            DropIndex("dbo.Parser_RoomInfo", new[] { "MealTypeId" });
            DropIndex("dbo.Parser_RoomInfo", new[] { "RoomTypeId" });
            DropIndex("dbo.Predictions", new[] { "MealTypeId" });
            DropIndex("dbo.Predictions", new[] { "RoomTypeId" });
            DropIndex("dbo.Reservations", new[] { "MealTypeId" });
            DropIndex("dbo.Reservations", new[] { "RoomTypeId" });
            DropColumn("dbo.Predictions", "MealTypeId");
            DropColumn("dbo.Predictions", "PeopleNum");
            DropColumn("dbo.Predictions", "RoomTypeId");
            DropColumn("dbo.Reservations", "MealTypeId");
            DropColumn("dbo.Reservations", "PeopleNum");
            DropColumn("dbo.Reservations", "RoomTypeId");
        }
    }
}
