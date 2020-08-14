namespace HotelGlob.RMS.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeChanges2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "GroupId", c => c.Int());
            DropColumn("dbo.Reservations", "GrouplId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservations", "GrouplId", c => c.Int());
            DropColumn("dbo.Reservations", "GroupId");
        }
    }
}
