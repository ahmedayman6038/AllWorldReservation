namespace AllWorldReservation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Posts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                        Content = c.String(),
                        CategoryId = c.Int(),
                        PhotoId = c.Int(),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .ForeignKey("dbo.Photos", t => t.PhotoId)
                .Index(t => t.CategoryId)
                .Index(t => t.PhotoId);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        UploadDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Hotels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false),
                        Price = c.Single(nullable: false),
                        Rating = c.Single(nullable: false),
                        Location = c.String(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        PhotoId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId)
                .Index(t => t.PhotoId);
            
            CreateTable(
                "dbo.Mails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 50),
                        Subject = c.String(nullable: false, maxLength: 50),
                        Message = c.String(nullable: false),
                        MailType = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Places",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(nullable: false),
                        Price = c.Single(nullable: false),
                        Rating = c.Single(nullable: false),
                        Location = c.String(nullable: false),
                        Duration = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        PhotoId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId)
                .Index(t => t.PhotoId);
            
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompanyName = c.String(nullable: false, maxLength: 50),
                        CompanyEmail = c.String(maxLength: 50),
                        CompanyAddress = c.String(maxLength: 100),
                        CompanyPhone = c.String(maxLength: 50),
                        CompanyTelephone = c.String(maxLength: 50),
                        CompanyFax = c.String(maxLength: 50),
                        AboutCompany = c.String(maxLength: 300),
                        FacebookUrl = c.String(maxLength: 200),
                        TwitterUrl = c.String(maxLength: 200),
                        InstagramUrl = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Places", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.Hotels", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.Posts", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.Posts", "CategoryId", "dbo.Categories");
            DropIndex("dbo.Places", new[] { "PhotoId" });
            DropIndex("dbo.Hotels", new[] { "PhotoId" });
            DropIndex("dbo.Posts", new[] { "PhotoId" });
            DropIndex("dbo.Posts", new[] { "CategoryId" });
            DropTable("dbo.Settings");
            DropTable("dbo.Places");
            DropTable("dbo.Mails");
            DropTable("dbo.Hotels");
            DropTable("dbo.Photos");
            DropTable("dbo.Posts");
            DropTable("dbo.Categories");
        }
    }
}
