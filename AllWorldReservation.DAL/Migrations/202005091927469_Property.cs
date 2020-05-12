namespace AllWorldReservation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Property : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Properties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        PriceUSD = c.Single(nullable: false),
                        PriceEGP = c.Single(nullable: false),
                        Address = c.String(nullable: false, maxLength: 150),
                        AvalibleFrom = c.DateTime(),
                        AvalibleTo = c.DateTime(),
                        CreatedDate = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                        Approved = c.Boolean(nullable: false),
                        PhotoId = c.Int(),
                        PlaceId = c.Int(),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId)
                .ForeignKey("dbo.Places", t => t.PlaceId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.PhotoId)
                .Index(t => t.PlaceId)
                .Index(t => t.UserId);
            
            AlterColumn("dbo.Settings", "CompanyName", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Properties", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Properties", "PlaceId", "dbo.Places");
            DropForeignKey("dbo.Properties", "PhotoId", "dbo.Photos");
            DropIndex("dbo.Properties", new[] { "UserId" });
            DropIndex("dbo.Properties", new[] { "PlaceId" });
            DropIndex("dbo.Properties", new[] { "PhotoId" });
            AlterColumn("dbo.Settings", "CompanyName", c => c.String(nullable: false, maxLength: 50));
            DropTable("dbo.Properties");
        }
    }
}
