namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HotelChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hotels", "IsNeedRecalc", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Hotels", "IsNeedRecalc");
        }
    }
}
