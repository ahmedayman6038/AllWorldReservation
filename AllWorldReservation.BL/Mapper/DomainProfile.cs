using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllWorldReservation.BL.Models;
using AllWorldReservation.DAL.Entities;
using AutoMapper;

namespace AllWorldReservation.BL.Mapper
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<Category, CategoryModel>();
            CreateMap<CategoryModel, Category>();

            CreateMap<Mail, MailModel>();
            CreateMap<MailModel, Mail>();

            CreateMap<Photo, PhotoModel>();
            CreateMap<PhotoModel, Photo>();

            CreateMap<Post, PostModel>();
            CreateMap<PostModel, Post>();

            CreateMap<Setting, SettingModel>()
                .ForMember(x => x.Currency, opt => opt.Ignore());
            CreateMap<SettingModel, Setting>();

            CreateMap<Hotel, HotelModel>()
                .ForMember(x => x.GUID, opt => opt.Ignore())
                .ForMember(x => x.ResultId, opt => opt.Ignore())
                .ForMember(x => x.Type, opt => opt.Ignore());
            CreateMap<HotelModel, Hotel>();

            CreateMap<Place, PlaceModel>();
            CreateMap<PlaceModel, Place>();

            CreateMap<Room, RoomModel>()
                .ForMember(x => x.Deleted, opt => opt.Ignore())
                .ForMember(x => x.RateId, opt => opt.Ignore());
            CreateMap<RoomModel, Room>();

            CreateMap<Guest, GuestModel>();
            CreateMap<GuestModel, Guest>();

            CreateMap<Reservation, ReservationModel>()
                .ForMember(x => x.CountryCode, opt => opt.Ignore())
                .ForMember(x => x.ReservedItem, opt => opt.Ignore());
            CreateMap<ReservationModel, Reservation>();

            CreateMap<Country, CountryModel>();
            CreateMap<CountryModel, Country>();

            CreateMap<Tour, TourModel>();
            CreateMap<TourModel, Tour>();

            CreateMap<ApplicationUser, UserModel>()
                .ForMember(x => x.Password, opt => opt.Ignore())
                .ForMember(x => x.Role, opt => opt.Ignore());
            CreateMap<UserModel, ApplicationUser>()
                .ForMember(x => x.AccessFailedCount, opt => opt.Ignore())
                .ForMember(x => x.Claims, opt => opt.Ignore())
                .ForMember(x => x.EmailConfirmed, opt => opt.Ignore())
                .ForMember(x => x.LockoutEnabled, opt => opt.Ignore())
                .ForMember(x => x.LockoutEndDateUtc, opt => opt.Ignore())
                .ForMember(x => x.Logins, opt => opt.Ignore())
                .ForMember(x => x.PasswordHash, opt => opt.Ignore())
                .ForMember(x => x.PhoneNumberConfirmed, opt => opt.Ignore())
                .ForMember(x => x.SecurityStamp, opt => opt.Ignore())
                .ForMember(x => x.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(x => x.Roles, opt => opt.Ignore());

            CreateMap<Property, PropertyModel>();
            CreateMap<PropertyModel, Property>();
        }
    }
}
