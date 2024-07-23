using System.Collections.Generic;
using System.Security.Principal;
using System;
using SpiritualNetwork.Entities;
using Microsoft.EntityFrameworkCore;
using SpiritualNetwork.API.AppContext;

namespace SpiritualNetwork.API.Services
{
    public interface IRepository<T> where T : IdEntity
    {
        T GetById(object id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void DeleteHard(T entity);
        void DeleteHardRange(IEnumerable<T> entity);
        void InsertRange(IEnumerable<T> entity);
        void UpdateRange(IEnumerable<T> entity);
        void DeleteRange(IEnumerable<T> entity);
        Task<T> GetByIdAsync(object id);
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task InsertRangeAsync(IEnumerable<T> entity);
        Task UpdateRangeAsync(IEnumerable<T> entity);
        Task DeleteRangeAsync(IEnumerable<T> entity);
        IQueryable<T> Table { get; }
    }

    public class Repository<T> : IRepository<T> where T : IdEntity
    {
        private readonly AppDbContext _context;
        private DbSet<T> _entities;

        public Repository(AppDbContext context)
        {
            _context = context;
        }

        public T GetById(object id)
        {
            return Entities.Find(id);
        }

        public void Insert(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                if (typeof(BaseEntity).IsAssignableFrom(entity.GetType()))
                {
                    (entity as BaseEntity).CreatedDate = DateTime.UtcNow;
                }
                Entities.Add(entity);
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public void InsertRange(IEnumerable<T> entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                List<T> list = new List<T>();
                foreach (var item in entity)
                {
                    if (typeof(BaseEntity).IsAssignableFrom(item.GetType()))
                    {
                        (item as BaseEntity).CreatedDate = DateTime.UtcNow;
                    }
                    list.Add(item);
                }

                Entities.AddRange(list);
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public void Update(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                if (typeof(BaseEntity).IsAssignableFrom(entity.GetType()))
                {
                    (entity as BaseEntity).ModifiedDate = DateTime.UtcNow;
                }
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public void UpdateRange(IEnumerable<T> entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                List<T> list = new List<T>();
                foreach (var item in entity)
                {
                    if (typeof(BaseEntity).IsAssignableFrom(item.GetType()))
                    {
                        (item as BaseEntity).ModifiedDate = DateTime.UtcNow;
                    }
                    list.Add(item);
                }
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }
        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                if (typeof(BaseEntity).IsAssignableFrom(entity.GetType()))
                {
                    (entity as BaseEntity).ModifiedDate = DateTime.UtcNow;
                    (entity as BaseEntity).IsDeleted = true;
                }
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public void DeleteHard(T entity)
        {
            try
            {
                var ent = _context.Set<T>().Find(entity.Id);

                if (ent != null)
                {
                    _context.Set<T>().Remove(entity);
                }
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }
        public void DeleteRange(IEnumerable<T> entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                List<T> list = new List<T>();
                foreach (var item in entity)
                {
                    if (typeof(BaseEntity).IsAssignableFrom(item.GetType()))
                    {
                        (item as BaseEntity).ModifiedDate = DateTime.UtcNow;
                        (item as BaseEntity).IsDeleted = true;
                    }
                    list.Add(item);
                }
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public void DeleteHardRange(IEnumerable<T> entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                List<T> list = new List<T>();
                foreach (var item in entity)
                {
                    var ent = _context.Set<T>().Find(item.Id);
                    list.Add(item);
                }
                if (list.Count() > 0)
                {
                    _context.Set<T>().RemoveRange(list);
                }
                _context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }
        public async Task<T> GetByIdAsync(object id)
        {
            var entity = await Entities.FindAsync(id);
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return entity;
        }




        public async Task InsertAsync(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                if (typeof(BaseEntity).IsAssignableFrom(entity.GetType()))
                {
                    (entity as BaseEntity).CreatedDate = DateTime.UtcNow;
                }
                Entities.Add(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public async Task InsertRangeAsync(IEnumerable<T> entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                List<T> list = new List<T>();
                foreach (var item in entity)
                {
                    if (typeof(BaseEntity).IsAssignableFrom(item.GetType()))
                    {
                        (item as BaseEntity).CreatedDate = DateTime.UtcNow;
                    }
                    list.Add(item);
                }
                Entities.AddRange(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                if (typeof(BaseEntity).IsAssignableFrom(entity.GetType()))
                {
                    (entity as BaseEntity).ModifiedDate = DateTime.UtcNow;
                }
                Entities.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                List<T> list = new List<T>();
                foreach (var item in entity)
                {
                    if (typeof(BaseEntity).IsAssignableFrom(item.GetType()))
                    {
                        (item as BaseEntity).ModifiedDate = DateTime.UtcNow;
                    }
                    list.Add(item);
                }
                Entities.UpdateRange(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }
        public async Task DeleteAsync(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                if (typeof(BaseEntity).IsAssignableFrom(entity.GetType()))
                {
                    (entity as BaseEntity).ModifiedDate = DateTime.UtcNow;
                    (entity as BaseEntity).IsDeleted = true;
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }
        public async Task DeleteRangeAsync(IEnumerable<T> entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                List<T> list = new List<T>();
                foreach (var item in entity)
                {
                    if (typeof(BaseEntity).IsAssignableFrom(item.GetType()))
                    {
                        (item as BaseEntity).ModifiedDate = DateTime.UtcNow;
                        (item as BaseEntity).IsDeleted = true;
                    }
                    list.Add(item);
                }
                Entities.RemoveRange(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                var msg = string.Empty;
                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        public virtual IQueryable<T> Table
        {
            get
            {
                return Entities;
            }
        }
        private DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                {
                    _entities = _context.Set<T>();
                }
                return _entities;
            }
        }
    }
}
