﻿using AllWorldReservation.BL.Interfaces;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        private GenericRepository<Category> categoryRepository;
        public IGenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new GenericRepository<Category>(_context);
                }
                return categoryRepository;
            }
        }

        private GenericRepository<Mail> mailRepository;
        public IGenericRepository<Mail> MailRepository
        {
            get
            {
                if (this.mailRepository == null)
                {
                    this.mailRepository = new GenericRepository<Mail>(_context);
                }
                return mailRepository;
            }
        }

        private GenericRepository<Photo> photoRepository;
        public IGenericRepository<Photo> PhotoRepository
        {
            get
            {
                if (this.photoRepository == null)
                {
                    this.photoRepository = new GenericRepository<Photo>(_context);
                }
                return photoRepository;
            }
        }

        private GenericRepository<Post> postRepository;
        public IGenericRepository<Post> PostRepository
        {
            get
            {
                if (this.postRepository == null)
                {
                    this.postRepository = new GenericRepository<Post>(_context);
                }
                return postRepository;
            }
        }

        private GenericRepository<Setting> settingRepository;
        public IGenericRepository<Setting> SettingRepository
        {
            get
            {
                if (this.settingRepository == null)
                {
                    this.settingRepository = new GenericRepository<Setting>(_context);
                }
                return settingRepository;
            }
        }

        private GenericRepository<Place> placeRepository;
        public IGenericRepository<Place> PlaceRepository
        {
            get
            {
                if (this.placeRepository == null)
                {
                    this.placeRepository = new GenericRepository<Place>(_context);
                }
                return placeRepository;
            }
        }

        private GenericRepository<Hotel> hotelRepository;
        public IGenericRepository<Hotel> HotelRepository
        {
            get
            {
                if (this.hotelRepository == null)
                {
                    this.hotelRepository = new GenericRepository<Hotel>(_context);
                }
                return hotelRepository;
            }
        }

        private GenericRepository<Property> propertyRepository;
        public IGenericRepository<Property> PropertyRepository
        {
            get
            {
                if (this.propertyRepository == null)
                {
                    this.propertyRepository = new GenericRepository<Property>(_context);
                }
                return propertyRepository;
            }
        }

        private GenericRepository<Room> roomRepository;
        public IGenericRepository<Room> RoomRepository
        {
            get
            {
                if (this.roomRepository == null)
                {
                    this.roomRepository = new GenericRepository<Room>(_context);
                }
                return roomRepository;
            }
        }

        private GenericRepository<Guest> guestRepository;
        public IGenericRepository<Guest> GuestRepository
        {
            get
            {
                if (this.guestRepository == null)
                {
                    this.guestRepository = new GenericRepository<Guest>(_context);
                }
                return guestRepository;
            }
        }

        private GenericRepository<Reservation> reservationRepository;
        public IGenericRepository<Reservation> ReservationRepository
        {
            get
            {
                if (this.reservationRepository == null)
                {
                    this.reservationRepository = new GenericRepository<Reservation>(_context);
                }
                return reservationRepository;
            }
        }

        private GenericRepository<Tour> tourRepository;
        public IGenericRepository<Tour> TourRepository
        {
            get
            {
                if (this.tourRepository == null)
                {
                    this.tourRepository = new GenericRepository<Tour>(_context);
                }
                return tourRepository;
            }
        }

        private GenericRepository<Country> countryRepository;
        public IGenericRepository<Country> CountryRepository
        {
            get
            {
                if (this.countryRepository == null)
                {
                    this.countryRepository = new GenericRepository<Country>(_context);
                }
                return countryRepository;
            }
        }

        public bool Save()
        {
            bool result = true;
            var dbContextTransaction = _context.Database.BeginTransaction();
            try
            {
                _context.SaveChanges();
                dbContextTransaction.Commit();
            }
            catch (Exception ex)
            {
                result = false;
                dbContextTransaction.Rollback();
            }
            return result;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
