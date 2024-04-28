using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SportsStore.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T: class
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        /// <summary>
        ///  метод Get принимает фильтр в виде выражения, которое определяет, 
        ///  какие элементы нужно вернуть из коллекции типа T, 
        ///  удовлетворяющие указанному условию
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false);
        void Add(T entity);
        void Remove(T entity);

        /// <summary>
        /// удаление коллекции данных типа T
        /// </summary>
        /// <param name="entities"></param>
        void RemoveRange(IEnumerable<T> entities);  
    }
}
