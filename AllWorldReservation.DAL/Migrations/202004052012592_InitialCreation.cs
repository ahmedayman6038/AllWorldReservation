namespace AllWorldReservation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Places", "PhotoId", "dbo.Photos");
            DropIndex("dbo.Places", new[] { "PhotoId" });
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Address = c.String(maxLength: 150),
                        City = c.String(maxLength: 50),
                        PostCode = c.String(maxLength: 10),
                        CountryId = c.Int(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .Index(t => t.CountryId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 2),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        TotalAmount = c.Single(nullable: false),
                        HotelId = c.Int(),
                        Guests = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Hotels", t => t.HotelId)
                .Index(t => t.HotelId);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PayType = c.Int(nullable: false),
                        ReservationType = c.Int(nullable: false),
                        ItemId = c.Int(nullable: false),
                        Email = c.String(nullable: false, maxLength: 50),
                        TelNo1 = c.String(nullable: false, maxLength: 50),
                        TelNo2 = c.String(maxLength: 50),
                        Address1 = c.String(nullable: false, maxLength: 150),
                        Address2 = c.String(maxLength: 150),
                        City = c.String(nullable: false, maxLength: 50),
                        PostCode = c.String(maxLength: 10),
                        CountryId = c.Int(),
                        Paied = c.Boolean(nullable: false),
                        OrderId = c.String(nullable: false, maxLength: 50),
                        TotalAmount = c.Single(nullable: false),
                        UserId = c.String(maxLength: 128),
                        ReservationFrom = c.DateTime(nullable: false),
                        ReservationTo = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.CountryId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Guests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 5),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        Surname = c.String(nullable: false, maxLength: 50),
                        Type = c.Int(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        ReservationId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Reservations", t => t.ReservationId)
                .Index(t => t.ReservationId);
            
            CreateTable(
                "dbo.Tours",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        Price = c.Single(nullable: false),
                        Duration = c.Int(nullable: false),
                        AvalibleFrom = c.DateTime(),
                        AvalibleTo = c.DateTime(),
                        CreatedDate = c.DateTime(nullable: false),
                        PhotoId = c.Int(),
                        PlaceId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId)
                .ForeignKey("dbo.Places", t => t.PlaceId)
                .Index(t => t.PhotoId)
                .Index(t => t.PlaceId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.ReservationRooms",
                c => new
                    {
                        Reservation_Id = c.Int(nullable: false),
                        Room_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Reservation_Id, t.Room_Id })
                .ForeignKey("dbo.Reservations", t => t.Reservation_Id, cascadeDelete: true)
                .ForeignKey("dbo.Rooms", t => t.Room_Id, cascadeDelete: true)
                .Index(t => t.Reservation_Id)
                .Index(t => t.Room_Id);
            
            AddColumn("dbo.Posts", "UserId", c => c.String(maxLength: 128));
            AddColumn("dbo.Hotels", "Address", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.Hotels", "AvalibleFrom", c => c.DateTime());
            AddColumn("dbo.Hotels", "AvalibleTo", c => c.DateTime());
            AddColumn("dbo.Places", "Code", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.Places", "CountryId", c => c.Int());
            AddColumn("dbo.Mails", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Posts", "UserId");
            CreateIndex("dbo.Places", "Code", unique: true);
            CreateIndex("dbo.Places", "CountryId");
            CreateIndex("dbo.Mails", "UserId");
            AddForeignKey("dbo.Places", "CountryId", "dbo.Countries", "Id");
            AddForeignKey("dbo.Mails", "UserId", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Posts", "UserId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.Hotels", "Location");
            DropColumn("dbo.Places", "Description");
            DropColumn("dbo.Places", "Price");
            DropColumn("dbo.Places", "Location");
            DropColumn("dbo.Places", "Duration");
            DropColumn("dbo.Places", "CreatedDate");
            DropColumn("dbo.Places", "PhotoId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Places", "PhotoId", c => c.Int());
            AddColumn("dbo.Places", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Places", "Duration", c => c.Int(nullable: false));
            AddColumn("dbo.Places", "Location", c => c.String(nullable: false));
            AddColumn("dbo.Places", "Price", c => c.Single(nullable: false));
            AddColumn("dbo.Places", "Description", c => c.String());
            AddColumn("dbo.Hotels", "Location", c => c.String(nullable: false));
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Posts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Mails", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Tours", "PlaceId", "dbo.Places");
            DropForeignKey("dbo.Tours", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.Reservations", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ReservationRooms", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.ReservationRooms", "Reservation_Id", "dbo.Reservations");
            DropForeignKey("dbo.Guests", "ReservationId", "dbo.Reservations");
            DropForeignKey("dbo.Reservations", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.Rooms", "HotelId", "dbo.Hotels");
            DropForeignKey("dbo.Places", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ReservationRooms", new[] { "Room_Id" });
            DropIndex("dbo.ReservationRooms", new[] { "Reservation_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Mails", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Tours", new[] { "PlaceId" });
            DropIndex("dbo.Tours", new[] { "PhotoId" });
            DropIndex("dbo.Guests", new[] { "ReservationId" });
            DropIndex("dbo.Reservations", new[] { "UserId" });
            DropIndex("dbo.Reservations", new[] { "CountryId" });
            DropIndex("dbo.Rooms", new[] { "HotelId" });
            DropIndex("dbo.Places", new[] { "CountryId" });
            DropIndex("dbo.Places", new[] { "Code" });
            DropIndex("dbo.Countries", new[] { "Code" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "CountryId" });
            DropIndex("dbo.Posts", new[] { "UserId" });
            DropColumn("dbo.Mails", "UserId");
            DropColumn("dbo.Places", "CountryId");
            DropColumn("dbo.Places", "Code");
            DropColumn("dbo.Hotels", "AvalibleTo");
            DropColumn("dbo.Hotels", "AvalibleFrom");
            DropColumn("dbo.Hotels", "Address");
            DropColumn("dbo.Posts", "UserId");
            DropTable("dbo.ReservationRooms");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Tours");
            DropTable("dbo.Guests");
            DropTable("dbo.Reservations");
            DropTable("dbo.Rooms");
            DropTable("dbo.Countries");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            CreateIndex("dbo.Places", "PhotoId");
            AddForeignKey("dbo.Places", "PhotoId", "dbo.Photos", "Id");
        }
    }
}
