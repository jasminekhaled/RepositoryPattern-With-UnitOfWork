using Microsoft.EntityFrameworkCore;
using Shopping.Core.Models.AuthModule;
using Shopping.Core.Models.BookModule;
using Shopping.Core.Models.CartModule;

namespace Shopping.EF.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategories> bookCategories { get; set; }
        public DbSet<BookUsers> bookUsers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartBooks> CartBooks { get; set; }
    }
}
