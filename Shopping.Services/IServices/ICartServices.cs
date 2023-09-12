using Shopping.Core.Dtos;
using Shopping.Core.Dtos.CartsDtos.RequestDtos;
using Shopping.Core.Dtos.CartsDtos.ResponseDtos;

namespace Shopping.Services.IServices
{
    public interface ICartServices
    {
        Task<GeneralResponse<CartDto>> AddToCart(int userId, int cartId, int bookId, AddDto dto);
        Task<GeneralResponse<CartBooksDto>> DeleteFromCart(int cartId, int bookId);
        Task<GeneralResponse<List<CartBooksDto>>> ListOfBooksInCart(int id);
        Task<GeneralResponse<CartBooksDto>> EditOnNumOfCopies(int cartId, int bookId, AddDto dto);
        Task<GeneralResponse<List<CartBooksDto>>> Buying(int userId, int cartId);
    }
}
