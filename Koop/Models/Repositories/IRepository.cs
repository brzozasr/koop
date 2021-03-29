using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Koop.Models.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetDetail(Func<T, bool> predicate);
        void Add(T entity);
        void Delete(T entity);
        Task<List<T>> ExecuteSql(string query, params object[] parameters);
    }
}