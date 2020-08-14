namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HotelChanges2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Inflations", "CountryId", c => c.Int(nullable: false));
            AddColumn("dbo.HotelSettings", "CountryId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Inflations", "CountryId");
            DropColumn("dbo.HotelSettings", "CountryId");
        }
    }
}
