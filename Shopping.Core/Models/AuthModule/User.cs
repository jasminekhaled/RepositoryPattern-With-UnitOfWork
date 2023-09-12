using Shopping.Core.Models.BookModule;
using Shopping.Core.Models.CartModule;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Core.Models.AuthModule
{
    public class User
    {
        public int id { get; set; }

        [MaxLength(length: 30)]
        [MinLength(length: 5)]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [MinLength(length: 10)]
        public string Password { get; set; }
        public List<BookUsers> bookUsers { get; set; }
        public Cart Cart { get; set; }

    }
}
