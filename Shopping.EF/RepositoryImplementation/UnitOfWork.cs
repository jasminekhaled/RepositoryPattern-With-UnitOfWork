using Shopping.Core.IRepository;
using Shopping.EF.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.EF.RepositoryImplementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBookRepository BookRepository { get; set; }

        public IBookUsersRepository BookUsersRepository { get; set; }

        public IBookCategoriesRepository BookCategoriesRepository { get; set; }

        public IUserRepository UserRepository { get; set; }

        public ICartRepository CartRepository { get; set; }

        public ICartBooksRepository CartBooksRepository { get; set; }

        public ICategoryRepository CategoryRepository { get; set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            BookRepository = new BookRepository(context);
            UserRepository = new UserRepository(context);
            BookCategoriesRepository = new BookCategoriesRepository(context);
            BookUsersRepository = new BookUsersRepository(context);
            CartBooksRepository = new CartBooksRepository(context);
            CartRepository = new CartRepository(context);
            CategoryRepository = new CategoryRepository(context);
            
        }


        public Task<int> Complete()
        {
            return _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        
    }
}
