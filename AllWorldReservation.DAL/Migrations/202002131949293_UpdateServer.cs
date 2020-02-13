namespace AllWorldReservation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateServer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "ItemId", c => c.Int(nullable: false));
            AddColumn("dbo.Photos", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.Hotels", "Stars", c => c.Int(nullable: false));
            DropColumn("dbo.Hotels", "Rating");
            DropColumn("dbo.Places", "Rating");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Places", "Rating", c => c.Single(nullable: false));
            AddColumn("dbo.Hotels", "Rating", c => c.Single(nullable: false));
            DropColumn("dbo.Hotels", "Stars");
            DropColumn("dbo.Photos", "Type");
            DropColumn("dbo.Photos", "ItemId");
        }
    }
}
