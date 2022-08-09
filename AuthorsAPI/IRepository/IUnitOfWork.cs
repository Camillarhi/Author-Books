using AuthorsAPI.Model;

namespace AuthorsAPI.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<BookModel> Books { get; }

        IGenericRepository<AuthorModel> Authors { get; }

        Task Save();
    }
}
