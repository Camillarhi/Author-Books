using AuthorsAPI.Context;
using AuthorsAPI.IRepository;
using AuthorsAPI.Model;

namespace AuthorsAPI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IGenericRepository<BookModel> _books;
        private IGenericRepository<AuthorModel> _authors;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<BookModel> Books => _books ??= new GenericRepository<BookModel>(_context);

        public IGenericRepository<AuthorModel> Authors => _authors ??= new GenericRepository<AuthorModel>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
