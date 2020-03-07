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

            CreateMap<Setting, SettingModel>();
            CreateMap<SettingModel, Setting>();

            CreateMap<Hotel, HotelModel>().ForMember(x => x.GUID, opt => opt.Ignore())
                                        .ForMember(x => x.ResultId, opt => opt.Ignore());
            CreateMap<HotelModel, Hotel>();

            CreateMap<Place, PlaceModel>();
            CreateMap<PlaceModel, Place>();

            CreateMap<Room, RoomModel>();
            CreateMap<RoomModel, Room>();

            CreateMap<Guest, GuestModel>();
            CreateMap<GuestModel, Guest>();

            CreateMap<Reservation, ReservationModel>();
            CreateMap<ReservationModel, Reservation>();
        }

    }
}
