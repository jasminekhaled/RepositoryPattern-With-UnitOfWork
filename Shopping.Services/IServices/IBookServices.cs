using Shopping.Core.Dtos;
using Shopping.Core.Dtos.BookDtos.RequestDtos;
using Shopping.Core.Dtos.BookDtos.ResponseDtos;

namespace Shopping.Services.IServices
{
    public interface IBookServices
    {
        Task<GeneralResponse<List<CategoryDto>>> GetAllCategories();
        Task<GeneralResponse<List<BookDto>>> GetAllBooks();
        Task<GeneralResponse<CategoryDto>> AddCategory(AddCategoryDto dto);
        Task<GeneralResponse<CategoryDto>> DeleteCategory(int id);
        Task<GeneralResponse<BookDto>> AddBook(BookRequestDto dto);
        Task<GeneralResponse<BookDto>> DeleteBook(int id);
        Task<GeneralResponse<BookDto>> BookDetails(int id);
        Task<GeneralResponse<BookDto>> EditBook(int id, EditRequestDto dto);
        Task<GeneralResponse<BookDto>> BuyABook(int bookId, int userId, BuyABookRequestDto dto);
        

    }
}
