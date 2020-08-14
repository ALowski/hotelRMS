namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoomTypesChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MealTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UID = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoomTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RoomTypeCode = c.String(),
                        HotelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hotels", t => t.HotelId, cascadeDelete: true)
                .Index(t => t.HotelId);
            
            AddColumn("dbo.Hotels", "IsSettingsBlocked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoomTypes", "HotelId", "dbo.Hotels");
            DropIndex("dbo.RoomTypes", new[] { "HotelId" });
            DropColumn("dbo.Hotels", "IsSettingsBlocked");
            DropTable("dbo.RoomTypes");
            DropTable("dbo.MealTypes");
        }
    }
}
