using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shopping.Core.Dtos;
using Shopping.Core.Dtos.BookDtos.RequestDtos;
using Shopping.Core.Dtos.BookDtos.ResponseDtos;
using Shopping.Core.IRepository;
using Shopping.Core.Models.BookModule;
using Shopping.Services.IServices;
using System.Collections;

namespace Shopping.Services.Services
{
    public class BookServices : IBookServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;


        public BookServices(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }



        public async Task<GeneralResponse<List<CategoryDto>>> GetAllCategories()
        {
            try
            {
                var list = await _unitOfWork.CategoryRepository.GetAllAsync();

                return new GeneralResponse<List<CategoryDto>>
                {
                    IsSuccess = true,
                    Message = "Categories Listed Successfully.",
                    Data = _mapper.Map<List<CategoryDto>>(list)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<CategoryDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }


        }


        public async Task<GeneralResponse<CategoryDto>> AddCategory(AddCategoryDto dto)
        {
            try
            {
                if (await _unitOfWork.CategoryRepository.AnyAsync(check: c => c.Name == dto.Name))
                {
                    return new GeneralResponse<CategoryDto>
                    {
                        IsSuccess = false,
                        Message = "This Category is already Existed.",
                    };
                }

                var category = _mapper.Map<Category>(dto);
                await _unitOfWork.CategoryRepository.AddAsync(category);
                await _unitOfWork.Complete();

                return new GeneralResponse<CategoryDto>
                {
                    IsSuccess = true,
                    Message = "New Category Is Added Successfully.",
                    Data = _mapper.Map<CategoryDto>(dto)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CategoryDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }

        }


        public async Task<GeneralResponse<CategoryDto>> DeleteCategory(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    return new GeneralResponse<CategoryDto>
                    {
                        IsSuccess = false,
                        Message = "No Category Found.",
                    };
                }
                _unitOfWork.CategoryRepository.Remove(category);
                await _unitOfWork.Complete();

                return new GeneralResponse<CategoryDto>
                {
                    IsSuccess = true,
                    Message = "The Category is deleted."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CategoryDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }

        }


        public async Task<GeneralResponse<List<BookDto>>> GetAllBookRepository()
        {
            try
            {
                var list = await _unitOfWork.BookRepository.GetAllAsync();

                return new GeneralResponse<List<BookDto>>
                {
                    IsSuccess = true,
                    Message = "BookRepository Listed Successfully.",
                    Data = _mapper.Map<List<BookDto>>(list)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<BookDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }


        public async Task<GeneralResponse<BookDto>> AddBook([FromForm] BookRequestDto dto)
        {
            try
            {
                var MaxAllowedPosterSize = _configuration.GetValue<long>("MaxAllowedPosterSize");
                List<string> AllowedExtenstions = _configuration.GetSection("AllowedExtenstions").Get<List<string>>();

                if (!AllowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "Only .jpg and .png Images Are Allowed."
                    };
                }

                if (dto.Poster.Length > MaxAllowedPosterSize)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "Max Allowed Size Is 1MB."
                    };
                }
                if (dto.NumOfCopies < 1)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "Num of copies should be at least 1."
                    };
                }
                if (dto.Price < 0)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "The Price cannot be a negative number."
                    };
                }
                if (dto.Rate < 0 || dto.Rate > 10)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "The Rate must be between 0 and 10."
                    };
                }
                if (dto.Year < 1)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "The Year must be a positive number."
                    };
                }
                foreach(var id in dto.CategoryId)
                {
                    if (!await _unitOfWork.CategoryRepository.AnyAsync(check: i => i.Id == id))
                    {
                       return new GeneralResponse<BookDto>
                       {
                            IsSuccess = false,
                            Message = "No Category exists with this Id."
                       }; 
                    }

                }
                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);

                var book = _mapper.Map<Book>(dto);
                 
                book.Poster = dataStream.ToArray();

                await _unitOfWork.BookRepository.AddAsync(book);
                
                await _unitOfWork.Complete();

                var bookCategories = dto.CategoryId.Select(c => new BookCategories
                {
                    bookId = book.Id,
                    categoryId = c
                });

                await _unitOfWork.BookCategoriesRepository.AddRangeAsync(bookCategories); 
                await _unitOfWork.Complete();


                var data = _mapper.Map<BookDto>(book);
                data.categoryId = dto.CategoryId;

                return new GeneralResponse<BookDto>
                {
                    IsSuccess = true,
                    Message = "The Book is added successfully.",
                    Data = data

                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<BookDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }

        }


        public async Task<GeneralResponse<BookDto>> DeleteBook(int id)
        {
            try
            {
                var book = await _unitOfWork.BookRepository.GetByIdAsync(id);
                if (book == null)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "No Book Found.",
                    };
                }
                _unitOfWork.BookRepository.Remove(book);
                await _unitOfWork.Complete();

                return new GeneralResponse<BookDto>
                {
                    IsSuccess = true,
                    Message = "The Book is deleted."
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<BookDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }



        public async Task<GeneralResponse<BookDto>> BookDetails(int id)
        {
            try 
            {
                var book = await _unitOfWork.BookRepository.GetByIdAsync(id);
                if(book == null)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "No book is existed with this id."
                    };
                }
                var names = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(c => c.bookId == book.Id, bc => bc.Category.Name);
                var ids = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(c => c.bookId == book.Id, i => i.categoryId);
                
                var data = _mapper.Map<BookDto>(book);
                data.categoryName = names.ToList();
                data.categoryId = ids.ToList();

                return new GeneralResponse<BookDto>
                {
                    IsSuccess = true,
                    Message = "Here are the Details.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<BookDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }



        public async Task<GeneralResponse<BookDto>> EditBook(int id, [FromForm]EditRequestDto dto)
        {
            try
            {
                var book = await _unitOfWork.BookRepository.GetByIdAsync(id);
                if (book == null)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "No book is existed with this id."
                    };
                }
                if(dto.Year != null && dto.Year < 1)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "The Year must be a positive number."
                    };
                }
                if (dto.NumOfCopies != null && dto.NumOfCopies < 1)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "Num of copies should be at least 1."
                    };
                }
                if (dto.Price != null && dto.Price < 0)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "The Price cannot be a negative number."
                    };
                }
                if (dto.Rate != null)
                {
                    if (dto.Rate < 0 || dto.Rate > 10)
                    {
                        return new GeneralResponse<BookDto>
                        {
                            IsSuccess = false,
                            Message = "The Rate must be between 0 and 10."
                        };
                    }
                }
                if(dto.Poster != null)
                {
                    var MaxAllowedPosterSize = _configuration.GetValue<long>("MaxAllowedPosterSize");
                    List<string> AllowedExtenstions = _configuration.GetSection("AllowedExtenstions").Get<List<string>>();

                    if (!AllowedExtenstions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    {
                        return new GeneralResponse<BookDto>
                        {
                            IsSuccess = false,
                            Message = "Only .jpg and .png Images Are Allowed."
                        };
                    }

                    if (dto.Poster.Length > MaxAllowedPosterSize)
                    {
                        return new GeneralResponse<BookDto>
                        {
                            IsSuccess = false,
                            Message = "Max Allowed Size Is 1MB."
                        };
                    }
                }

                if(dto.CategoryId != null)
                {
                    foreach (var iD in dto.CategoryId)
                    {
                        if (!await _unitOfWork.CategoryRepository.AnyAsync(check: i => i.Id == iD))
                        {
                            return new GeneralResponse<BookDto>
                            {
                                IsSuccess = false,
                                Message = "No Category exists with this Id."
                            };
                        }

                    }
                    var existingBookCategories = await _unitOfWork.BookCategoriesRepository
                     .Where(bc => bc.bookId == book.Id);

                    _unitOfWork.BookCategoriesRepository.RemoveRange(existingBookCategories);

                    var bookCategories = dto.CategoryId.Select(c => new BookCategories
                    {
                        bookId = book.Id,
                        categoryId = c
                    }).ToList();
 
                   await _unitOfWork.BookCategoriesRepository.AddRangeAsync(bookCategories);
                   _unitOfWork.BookRepository.Update(book);
                   await _unitOfWork.Complete(); 

                   
                }


                if(dto.Poster!=null)
                {
                    using var dataStream = new MemoryStream();
                    await dto.Poster.CopyToAsync(dataStream);
                    book.Poster = dataStream.ToArray();
                }

                book.Title = dto.Title ?? book.Title;
                book.Author = dto.Author ?? book.Author;
                book.Description = dto.Description ?? book.Description;
                book.NumOfCopies = dto.NumOfCopies ?? book.NumOfCopies;
                book.Price = dto.Price ?? book.Price;
                book.Year = dto.Year ?? book.Year;
                book.Rate = dto.Rate ?? book.Rate;

                _unitOfWork.BookRepository.Update(book);
                await _unitOfWork.Complete();

                var names = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(c => c.bookId == book.Id, bc => bc.Category.Name);

                var data = _mapper.Map<BookDto>(book);
                data.categoryName = names.ToList();
                data.categoryId = dto.CategoryId;

                return new GeneralResponse<BookDto>
                {
                    IsSuccess = true,
                    Message = "The book is editted successfully.",
                    Data = data
                };

            }
            catch (Exception ex)
            {
                return new GeneralResponse<BookDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }



        public async Task<GeneralResponse<BookDto>> BuyABook(int bookId,int userId,BuyABookRequestDto dto)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "No user Found.",
                    };
                }

                var book = await _unitOfWork.BookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "No Book Found.",
                    };
                }

                if(dto.NumOfCopies < 1 || dto.NumOfCopies > book.NumOfCopies)
                {
                    return new GeneralResponse<BookDto>
                    {
                        IsSuccess = false,
                        Message = "the number of copies that you want isnot available.",
                    };
                }

                book.NumOfCopies = book.NumOfCopies - dto.NumOfCopies;
                
                var bookUsers = new BookUsers
                {
                    bookId = book.Id,
                    userId = user.id,
                    Date = DateTime.Now,
                    NumOfBoughtCopies = dto.NumOfCopies,
                    price = book.Price * dto.NumOfCopies
                };
                await _unitOfWork.BookUsersRepository.AddAsync(bookUsers);

                book.NumOfSoldCopies += dto.NumOfCopies; 


                _unitOfWork.BookRepository.Update(book);
                await _unitOfWork.Complete();

                var names = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(c => c.bookId == book.Id, bc => bc.Category.Name);
                var ids = await _unitOfWork.BookCategoriesRepository.GetSpecificItems(c => c.bookId == book.Id, i => i.categoryId);

                var data = _mapper.Map<BookDto>(book);
                data.categoryName = names.ToList();
                data.categoryId = ids.ToList();

                return new GeneralResponse<BookDto>
                {
                    IsSuccess = true,
                    Message = "The book has been successfully purchased.",
                    Data = data
                };


            }
            catch (Exception ex)
            {
                return new GeneralResponse<BookDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }

        public  async Task<GeneralResponse<List<BookDto>>> GetAllBooks()
        {
            try
            {
                var list = await _unitOfWork.BookRepository.GetAllAsync();

                return new GeneralResponse<List<BookDto>>
                {
                    IsSuccess = true,
                    Message = "Books Listed Successfully.",
                    Data = _mapper.Map<List<BookDto>>(list)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<BookDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }
    }
}
