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
    public class BookCategoriesRepository : BaseRepository<BookCategories>, IBookCategoriesRepository
    {
        private readonly ApplicationDbContext _context;

        public BookCategoriesRepository(ApplicationDbContext context) : base(context)
        {
        }


    }
}
