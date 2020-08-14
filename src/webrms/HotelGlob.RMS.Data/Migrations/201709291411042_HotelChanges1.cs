namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HotelChanges1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Hotels", newName: "HotelSettings");
            AddColumn("dbo.HotelSettings", "HotelId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HotelSettings", "HotelId");
            RenameTable(name: "dbo.HotelSettings", newName: "Hotels");
        }
    }
}
