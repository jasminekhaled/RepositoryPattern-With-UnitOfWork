using AutoMapper;
using Shopping.Core.Dtos;
using Shopping.Core.Dtos.StatisticsDtos.ResponseDtos;
using Shopping.Core.IRepository;
using Shopping.Services.IServices;

namespace Shopping.Services.Services
{
    public class StatisticsServices : IStatisticsServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public StatisticsServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<GeneralResponse<List<TopRatedDto>>> TopRatedBooks()
        {
            try
            {
                var books = await _unitOfWork.BookRepository.OrderByDescendingTheTop5(r => r.Rate, 5);
                var Data = _mapper.Map<List<TopRatedDto>>(books);
                foreach (var book in books)
                {
                    var names = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(i => i.bookId == book.Id, s => s.Category.Name);
                    var data = Data.FirstOrDefault(i => i.id == book.Id);
                    data.Categories = names.ToList();
                }
                return new GeneralResponse<List<TopRatedDto>>
                {
                    IsSuccess = true,
                    Message = "Top Rated Books Listed Successfully.",
                    Data = Data
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<TopRatedDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }

        }

        public async Task<GeneralResponse<List<MostSoldBooksDto>>> MostSoldBooksInGeneral()
        {
            try
            {
                var books = await _unitOfWork.BookRepository.OrderByDescendingTheTop5(s => s.NumOfSoldCopies, 5);
                var Data = _mapper.Map<List<MostSoldBooksDto>>(books);
                foreach (var book in books)
                {
                    var names = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(i => i.bookId == book.Id, s => s.Category.Name);
                    var data = Data.FirstOrDefault(i => i.id == book.Id);
                    data.Categories = names.ToList();
                }
                return new GeneralResponse<List<MostSoldBooksDto>>
                {
                    IsSuccess = true,
                    Message = "Most Sold Books Listed Successfully.",
                    Data = Data
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<MostSoldBooksDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }

        public async Task<GeneralResponse<List<MostSoldBooksDto>>> MostSoldBooksInPeriod(DateTime startDate, DateTime endDate)
        {
            try
            {
                var info = await _unitOfWork.BookUsersRepository.GetSelectedAsync(d => d.Date >= startDate && d.Date < endDate,
                    g => g.bookId,
                    s => new
                    {
                        bookId = s.Key,
                        NumOfBoughtCopies = s.Sum(g => g.NumOfBoughtCopies)
                    },
                    g => g.NumOfBoughtCopies,
                    5);
                    

                var ids = info.Select(s => s.bookId).ToList();
                var copies = info.Select(s => s.NumOfBoughtCopies).ToList();

                var books = await _unitOfWork.BookRepository
                    .Where(b => ids.Contains(b.Id));


                var Data = _mapper.Map<List<MostSoldBooksDto>>(books);
                foreach (var book in books)
                {
                    var names = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(i => i.bookId == book.Id, s => s.Category.Name);
                    var data = Data.FirstOrDefault(i => i.id == book.Id);
                    data.Categories = names.ToList();
                    data.NumOfSoldCopies = info.Where(i => i.bookId == book.Id).Select(s => s.NumOfBoughtCopies).FirstOrDefault(); 
                }
                
                return new GeneralResponse<List<MostSoldBooksDto>>
                {
                    IsSuccess = true,
                    Message = "Most Sold Books Listed Successfully.",
                    Data = Data
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<MostSoldBooksDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }

        public async Task<GeneralResponse<TopCategoryDto>> TopCategory()
        {
            try
            {
                var books = await _unitOfWork.BookRepository.OrderByDescendingTheTop5(orderBy: s => s.NumOfSoldCopies, 5);
                var booksId = books.Select(s => s.Id).ToList();
                var bookCategory = await _unitOfWork.BookCategoriesRepository.Where(b => booksId.Contains(b.bookId));

                var CategoryId = bookCategory.Select(bc => bc.categoryId).GroupBy(g => g).Select(s => new
                {
                    categoryId = s.Key,
                    count = s.Count()
                }).OrderByDescending(c => c.count).Select(i => i.categoryId).FirstOrDefault();


                var CategoryName = await _unitOfWork.CategoryRepository.GetByIdAsync(CategoryId);

                

                return new GeneralResponse<TopCategoryDto>
                {
                    IsSuccess = true,
                    Message = "Top Category is posted Successfully.",
                    Data = _mapper.Map<TopCategoryDto>(CategoryName)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<TopCategoryDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }

        public async Task<GeneralResponse<TopCategoryDto>> TopCategoryForEachUser(int userId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if(user == null)
                {
                    return new GeneralResponse<TopCategoryDto>
                    {
                        IsSuccess = false,
                        Message = "No User Found with this Id .",
                    };
                }
                var userBooks = await _unitOfWork.BookUsersRepository.Where(i => i.userId == userId);
                if (userBooks == null)
                {
                    return new GeneralResponse<TopCategoryDto>
                    {
                        IsSuccess = false,
                        Message = "The User doesnot have a favourite category yet.",
                    };
                }
                var booksId = userBooks.Select(s => s.bookId).GroupBy(bookid => bookid).Select(ss => ss.Key).ToList();
                
                var bookCategory = await _unitOfWork.BookCategoriesRepository.Where(b => booksId.Contains(b.bookId));

                var CategoryId = bookCategory.Select(bc => bc.categoryId).GroupBy(g => g).Select(s => new
                {
                    categoryId = s.Key,
                    count = s.Count()
                }).OrderByDescending(c => c.count).Select(i => i.categoryId).FirstOrDefault();
                
                    
                var CategoryName = await _unitOfWork.CategoryRepository.GetByIdAsync(CategoryId);
                return new GeneralResponse<TopCategoryDto>
                {
                    IsSuccess = true,
                    Message = "Top Category is posted Successfully.",
                    Data = _mapper.Map<TopCategoryDto>(CategoryName)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<TopCategoryDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }


        public async Task<GeneralResponse<List<TopRatedDto>>> GetBooksByCategory(int categoryId)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return new GeneralResponse<List<TopRatedDto>>
                    {
                        IsSuccess = false,
                        Message = "No Category Found with this Id .",
                    };
                }
                var booksCategory = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(i => i.categoryId == categoryId, b => b.bookId);
                var books = await _unitOfWork.BookRepository.Where(i => booksCategory.Contains(i.Id));
                var Data = _mapper.Map<List<TopRatedDto>>(books);
                foreach (var book in books)
                {
                    var names = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(i => i.bookId == book.Id, s => s.Category.Name);
                    var data = Data.FirstOrDefault(i => i.id == book.Id);
                    data.Categories = names.ToList();
                }
                return new GeneralResponse<List<TopRatedDto>>
                {
                    IsSuccess = true,
                    Message = "Books  is listed Successfully.",
                    Data = Data
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<TopRatedDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }

        public async Task<GeneralResponse<UserActivityDto>> UserActivity(int id)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return new GeneralResponse<UserActivityDto>
                    {
                        IsSuccess = false,
                        Message = "No User Found with this Id .",
                    };
                }
                var v = await _unitOfWork.BookUsersRepository.GetFirstItem(filter: i => i.userId == id);
                var check = await _unitOfWork.BookUsersRepository.GetOneAsync(filter: i => i.userId == id, select: s => s.Date, orderBy: d => d.Date);
                if(v == null)
                {
                    return new GeneralResponse<UserActivityDto>
                    {
                        IsSuccess = false,
                        Message = "No Buying Processes is done yet .",
                    };
                }
                var fromMonth = DateTime.Now.AddDays(-30);
                var isActive = (fromMonth > check) ? false : true;
                var data = new UserActivityDto
                {
                    IsActive = isActive,
                    LastBuyingProcess = check
                };
                return new GeneralResponse<UserActivityDto>
                {
                    IsSuccess = true,
                    Message = "this is the user status.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<UserActivityDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }

        public async Task<GeneralResponse<List<TotalSalesDto>>> TotalSales(DateTime startDate, DateTime endDate)
        {
            try
            {
                var bu = await _unitOfWork.BookUsersRepository.Where(d => d.Date >= startDate && d.Date < endDate);
                var data = bu.GroupBy(g => g.bookId).Select(s => new TotalSalesDto
                {
                    bookId = s.Key,
                    Copies = s.Sum(u => u.NumOfBoughtCopies),
                    price = s.Sum(m => m.price)
                })
                .ToList();
                var total = data.Select(e => e.price).Sum();

                
                return new GeneralResponse<List<TotalSalesDto>>
                {
                    IsSuccess = true,
                    Message = $"The Total Sales is {total}$ .",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<TotalSalesDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }

       
    }
}
