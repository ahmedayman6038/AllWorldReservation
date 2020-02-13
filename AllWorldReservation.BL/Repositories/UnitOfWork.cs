using AllWorldReservation.BL.Interfaces;
using AllWorldReservation.DAL.Context;
using AllWorldReservation.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllWorldReservation.BL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContainer _context;

        public UnitOfWork(DbContainer context)
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
