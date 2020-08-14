namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventChanges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HotelId = c.Int(nullable: false),
                        Name = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        Coef = c.Double(nullable: false),
                        PriceCoef = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hotels", t => t.HotelId, cascadeDelete: true)
                .Index(t => t.HotelId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "HotelId", "dbo.Hotels");
            DropIndex("dbo.Events", new[] { "HotelId" });
            DropTable("dbo.Events");
        }
    }
}
