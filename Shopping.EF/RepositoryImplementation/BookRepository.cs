using Shopping.Core.IRepository;
using Shopping.Core.Models.BookModule;
using Shopping.EF.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.EF.RepositoryImplementation
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        
    }
}
