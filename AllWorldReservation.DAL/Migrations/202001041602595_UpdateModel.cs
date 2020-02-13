namespace AllWorldReservation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Hotels", "Description", c => c.String());
            AlterColumn("dbo.Places", "Description", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Places", "Description", c => c.String(nullable: false));
            AlterColumn("dbo.Hotels", "Description", c => c.String(nullable: false));
        }
    }
}
