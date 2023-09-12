using Shopping.Core.Models.AuthModule;
namespace Shopping.Core.Models.CartModule
{
    public class Cart
    {
        public int Id { get; set; }
        public double TotalPrice { get; set; }
        public List<CartBooks> cartBooks { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

    }
}
