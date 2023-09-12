using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.Core.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        public IBookRepository BookRepository { get; }
        public IBookUsersRepository BookUsersRepository { get; }
        public IBookCategoriesRepository BookCategoriesRepository { get; }
        public IUserRepository UserRepository { get; }
        public ICartRepository CartRepository { get; }
        public ICartBooksRepository CartBooksRepository { get; }
        public ICategoryRepository CategoryRepository { get; }
        Task<int> Complete();
        void Dispose();
    }
}
