using System;
using System.Linq;
using System.Linq.Expressions;

namespace Imposto.Core.Data
{
    public interface IRepository<T>
    {
        T GetById(int id);
        T[] GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> filter);
        void Save(T entity);
        void Delete(T entity);
    }
}
