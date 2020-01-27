using Infrastructure.Business_Logic;
using Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessManagement.Repo
{
    public class Repository<T> : IRepository<T>
         where T : class, new()
    {

        private readonly ApplicationDbContext _dbContext;
        private DbSet<T> _set;
        public void Delete(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
                Entities.Remove(entity);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public Repository(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
            if (_dbContext.Database.EnsureCreated())
                _dbContext.Database.Migrate();
        }
        private DbSet<T> Entities
        {
            get
            {
                if (_set == null)
                {
                    _set = _dbContext.Set<T>();
                }
                return _set;
            }
        }
        public IQueryable<T> GetAll()
        {
            return Entities;
        }

        public void Insert(T entity)
        {
            try
            {
                if(entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }
                Entities.Add(entity);
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public void Insert(IList<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            var ce = _dbContext.Entry<T>(entity);
            if(ce != null)
            {
                if (ce.State == EntityState.Detached)
                {
                    _set.Attach(entity);
                }
                ce.State = EntityState.Modified;
            }
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
