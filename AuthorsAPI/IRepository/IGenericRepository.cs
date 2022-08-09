using System.Linq.Expressions;

namespace AuthorsAPI.IRepository
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> GetAll(
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null
            );

        Task<T> Get(Expression<Func<T, bool>> expression);

        Task Insert(T entity);

        Task Delete(int id);

        void Update(T entity);  
    }
}
