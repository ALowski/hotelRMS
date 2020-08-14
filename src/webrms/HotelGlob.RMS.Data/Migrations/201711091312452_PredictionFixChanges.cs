namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PredictionFixChanges : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Predictions", "ExpectedLoad");
            AddColumn("dbo.Predictions", "ExpectedLoad", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Predictions", "ExpectedLoad");
            AddColumn("dbo.Predictions", "ExpectedLoad", c => c.Int(nullable: false));
        }
    }
}
