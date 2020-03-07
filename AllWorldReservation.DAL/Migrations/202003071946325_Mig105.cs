namespace AllWorldReservation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mig105 : DbMigration
    {
        public override void Up()
        {
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
                        Country = c.String(nullable: false, maxLength: 50),
                        Paied = c.Boolean(nullable: false),
                        OrderId = c.String(nullable: false, maxLength: 50),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            AddColumn("dbo.Hotels", "Address", c => c.String(nullable: false, maxLength: 150));
            AddColumn("dbo.Hotels", "AvalibleFrom", c => c.DateTime());
            AddColumn("dbo.Hotels", "AvalibleTo", c => c.DateTime());
            AddColumn("dbo.Places", "Code", c => c.Int(nullable: false));
            AddColumn("dbo.Places", "Country", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.Hotels", "Location");
            DropColumn("dbo.Places", "Location");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Places", "Location", c => c.String(nullable: false));
            AddColumn("dbo.Hotels", "Location", c => c.String(nullable: false));
            DropForeignKey("dbo.ReservationRooms", "Room_Id", "dbo.Rooms");
            DropForeignKey("dbo.ReservationRooms", "Reservation_Id", "dbo.Reservations");
            DropForeignKey("dbo.Guests", "ReservationId", "dbo.Reservations");
            DropForeignKey("dbo.Rooms", "HotelId", "dbo.Hotels");
            DropIndex("dbo.ReservationRooms", new[] { "Room_Id" });
            DropIndex("dbo.ReservationRooms", new[] { "Reservation_Id" });
            DropIndex("dbo.Guests", new[] { "ReservationId" });
            DropIndex("dbo.Rooms", new[] { "HotelId" });
            DropColumn("dbo.Places", "Country");
            DropColumn("dbo.Places", "Code");
            DropColumn("dbo.Hotels", "AvalibleTo");
            DropColumn("dbo.Hotels", "AvalibleFrom");
            DropColumn("dbo.Hotels", "Address");
            DropTable("dbo.ReservationRooms");
            DropTable("dbo.Guests");
            DropTable("dbo.Reservations");
            DropTable("dbo.Rooms");
        }
    }
}
