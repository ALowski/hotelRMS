namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParserChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Parser_RoomData",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        Parser_RoomInfoId = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                        AverageOccupancy = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Parser_RoomInfo", t => t.Parser_RoomInfoId, cascadeDelete: true)
                .Index(t => t.Parser_RoomInfoId);
            
            CreateTable(
                "dbo.Parser_RoomInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoomTypeId = c.Int(nullable: false),
                        MealTypeId = c.Int(nullable: false),
                        HotelId = c.Int(nullable: false),
                        RoomName = c.String(),
                        PeopleNum = c.Int(nullable: false),
                        Cancelation = c.Boolean(nullable: false),
                        Prepayment = c.Boolean(nullable: false),
                        City = c.String(),
                        HotelName = c.String(),
                        MealName = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hotels", t => t.HotelId, cascadeDelete: false)
                .Index(t => t.HotelId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Parser_RoomData", "Parser_RoomInfoId", "dbo.Parser_RoomInfo");
            DropForeignKey("dbo.Parser_RoomInfo", "HotelId", "dbo.Hotels");
            DropIndex("dbo.Parser_RoomInfo", new[] { "HotelId" });
            DropIndex("dbo.Parser_RoomData", new[] { "Parser_RoomInfoId" });
            DropTable("dbo.Parser_RoomInfo");
            DropTable("dbo.Parser_RoomData");
        }
    }
}
