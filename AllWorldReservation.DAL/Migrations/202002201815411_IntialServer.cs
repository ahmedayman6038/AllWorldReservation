namespace AllWorldReservation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IntialServer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hotels", "PlaceId", c => c.Int());
            CreateIndex("dbo.Hotels", "PlaceId");
            AddForeignKey("dbo.Hotels", "PlaceId", "dbo.Places", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Hotels", "PlaceId", "dbo.Places");
            DropIndex("dbo.Hotels", new[] { "PlaceId" });
            DropColumn("dbo.Hotels", "PlaceId");
        }
    }
}
