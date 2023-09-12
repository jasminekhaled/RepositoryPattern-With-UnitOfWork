using Shopping.Core.IRepository;
using Shopping.Core.Models.AuthModule;
using Shopping.EF.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopping.EF.RepositoryImplementation
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {

        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }


    }
}
