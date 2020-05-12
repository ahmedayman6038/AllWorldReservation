using AllWorldReservation.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> CategoryRepository { get; }

        IGenericRepository<Mail> MailRepository { get; }

        IGenericRepository<Photo> PhotoRepository { get; }

        IGenericRepository<Post> PostRepository { get; }

        IGenericRepository<Setting> SettingRepository { get; }

        IGenericRepository<Place> PlaceRepository { get; }

        IGenericRepository<Hotel> HotelRepository { get; }

        IGenericRepository<Property> PropertyRepository { get; }

        IGenericRepository<Room> RoomRepository { get; }

        IGenericRepository<Guest> GuestRepository { get; }

        IGenericRepository<Reservation> ReservationRepository { get; }

        IGenericRepository<Tour> TourRepository { get; }

        IGenericRepository<Country> CountryRepository { get; }

        bool Save();

    }
}
