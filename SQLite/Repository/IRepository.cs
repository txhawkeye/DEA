using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DEA.SQLite.Repository
{
    public interface IRepository<T>
    {
        Task InsertAsync(T entity);
        Task DeleteAsync(T entity);
        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
    }
}
