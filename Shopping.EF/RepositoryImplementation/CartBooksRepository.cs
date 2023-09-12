using Shopping.Core.IRepository;
using Shopping.Core.Models.CartModule;
using Shopping.EF.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.EF.RepositoryImplementation
{
    public class CartBooksRepository : BaseRepository<CartBooks>, ICartBooksRepository
    {
        private readonly ApplicationDbContext _context;

        public CartBooksRepository(ApplicationDbContext context) : base(context)
        {
        }


    }
}
