namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Calculations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CalculatedOn = c.DateTime(nullable: false),
                        PredictionDate = c.DateTime(nullable: false),
                        Settings = c.String(),
                        HotelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hotels", t => t.HotelId, cascadeDelete: true)
                .Index(t => t.HotelId);
            
            CreateTable(
                "dbo.Hotels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Settings = c.String(),
                        IsRmsEnalbed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        Body = c.String(),
                        LogType = c.Int(nullable: false),
                        HotelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hotels", t => t.HotelId, cascadeDelete: true)
                .Index(t => t.HotelId);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedOn = c.DateTime(nullable: false),
                        OrderDate = c.DateTime(nullable: false),
                        CheckInDate = c.DateTime(nullable: false),
                        DaysCount = c.Int(nullable: false),
                        PricePerNight = c.Double(nullable: false),
                        RoomType = c.Int(nullable: false),
                        ReservationStatus = c.Int(nullable: false),
                        MealType = c.Int(nullable: false),
                        HotelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hotels", t => t.HotelId, cascadeDelete: false)
                .Index(t => t.HotelId);
            
            CreateTable(
                "dbo.Predictions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoomType = c.Int(nullable: false),
                        CategoryType = c.Int(nullable: false),
                        MealType = c.Int(nullable: false),
                        ExpectedLoad = c.Int(nullable: false),
                        PriceCushion = c.Double(nullable: false),
                        Price = c.Double(nullable: false),
                        CalculationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Calculations", t => t.CalculationId, cascadeDelete: false)
                .Index(t => t.CalculationId);
            
            CreateTable(
                "dbo.Inflations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Coef = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Predictions", "CalculationId", "dbo.Calculations");
            DropForeignKey("dbo.Reservations", "HotelId", "dbo.Hotels");
            DropForeignKey("dbo.Logs", "HotelId", "dbo.Hotels");
            DropForeignKey("dbo.Calculations", "HotelId", "dbo.Hotels");
            DropIndex("dbo.Predictions", new[] { "CalculationId" });
            DropIndex("dbo.Reservations", new[] { "HotelId" });
            DropIndex("dbo.Logs", new[] { "HotelId" });
            DropIndex("dbo.Calculations", new[] { "HotelId" });
            DropTable("dbo.Inflations");
            DropTable("dbo.Predictions");
            DropTable("dbo.Reservations");
            DropTable("dbo.Logs");
            DropTable("dbo.Hotels");
            DropTable("dbo.Calculations");
        }
    }
}
