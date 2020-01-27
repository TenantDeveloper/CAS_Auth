using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Business_Logic
{
   public interface IRepository<T>
         where T: class , new ()
    {
        void Insert(T entity);
        void Insert(IList<T> entities);
        void Update(T entity);
        void Delete(T entity);
        IQueryable<T> GetAll();
        void Save();
    }
}
