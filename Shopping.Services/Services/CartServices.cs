using AutoMapper;
using Shopping.Core.Dtos;
using Shopping.Core.Dtos.CartsDtos.RequestDtos;
using Shopping.Core.Dtos.CartsDtos.ResponseDtos;
using Shopping.Core.IRepository;
using Shopping.Core.Models.BookModule;
using Shopping.Core.Models.CartModule;
using Shopping.Services.IServices;


namespace Shopping.Services.Services
{
    public class CartServices : ICartServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public CartServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<GeneralResponse<List<CartBooksDto>>> ListOfBooksInCart(int id)
        {
            try
            {
                if(!await _unitOfWork.CartRepository.AnyAsync( check: i => i.Id == id))
                {
                    return new GeneralResponse<List<CartBooksDto>>
                    {
                        IsSuccess = false,
                        Message = "No cart found with this Id!",
                    };
                }
                var books = await _unitOfWork.CartBooksRepository.Where(c => c.cartId == id);

                return new GeneralResponse<List<CartBooksDto>>
                {
                    IsSuccess = true,
                    Message = "Books Listed Successfully.",
                    Data = _mapper.Map<List<CartBooksDto>>(books)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<CartBooksDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }

        }
        public async Task<GeneralResponse<CartDto>> AddToCart(int userId,int cartId, int bookId, AddDto dto)
        {
            try 
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if(user == null)
                {
                    return new GeneralResponse<CartDto>
                    {
                        IsSuccess = false,
                        Message = "No user found with this Id."
                    };
                }

                var cart = await _unitOfWork.CartRepository.SingleOrDefaultAsync(u => u.UserId == userId);
                if (cart.Id != cartId)
                {
                    return new GeneralResponse<CartDto>
                    {
                        IsSuccess = false,
                        Message = "Wrong Cart Id."
                    };
                }

                var book = await _unitOfWork.BookRepository.GetByIdAsync(bookId);
                if (book == null)
                {
                    return new GeneralResponse<CartDto>
                    {
                        IsSuccess = false,
                        Message = "No book found with this Id."
                    };
                }
                if(await _unitOfWork.CartBooksRepository.FilterAndAnyAsync(filter: n => n.cartId == cartId, check: i => i.bookId == bookId))
                {
                    return new GeneralResponse<CartDto>
                    {
                        IsSuccess = false,
                        Message = "this book is already added in your cart."
                    };
                }    
                if (dto.wantedCopies > book.NumOfCopies || dto.wantedCopies <= 0)
                {
                    return new GeneralResponse<CartDto>
                    {
                        IsSuccess = false,
                        Message = "the number of wanted copies isnot available."
                    };
                }
                var BookPrice = book.Price * dto.wantedCopies;
                
                

                var cartBook = new CartBooks
                {
                    bookId = book.Id,
                    cartId = cart.Id, 
                    WantedCopies = dto.wantedCopies,
                    Price = BookPrice
                };

                await _unitOfWork.CartBooksRepository.AddAsync(cartBook);
                await _unitOfWork.Complete();

                cart.TotalPrice = cart.TotalPrice + BookPrice;
                _unitOfWork.CartRepository.Update(cart);
                await _unitOfWork.Complete();

                var data = _mapper.Map<CartDto>(book);
                data.WantedCopies = dto.wantedCopies;
                data.Price = BookPrice;

                return new GeneralResponse<CartDto>
                {
                    IsSuccess = true,
                    Message = "The Book is added to the cart successfully.",
                    Data = data
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<CartDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }


        public async Task<GeneralResponse<CartBooksDto>> DeleteFromCart(int cartId, int bookId)
        {
            try 
            {
                var cart = await _unitOfWork.CartRepository.GetByIdAsync(cartId);
                if (cart == null)
                {
                    return new GeneralResponse<CartBooksDto>
                    {
                        IsSuccess = false,
                        Message = "No Cart found with this Id."
                    };
                }

               var check = await _unitOfWork.CartBooksRepository.GetSpecificItem(n => n.cartId == cartId, i => i.bookId == bookId);
                if (check == null)
                {
                    return new GeneralResponse<CartBooksDto>
                    {
                        IsSuccess = false,
                        Message = "No book found in your cart with this Id."
                    };
                }
                
                cart.TotalPrice = cart.TotalPrice - check.Price;
                _unitOfWork.CartBooksRepository.Remove(check);
                await _unitOfWork.Complete();

                return new GeneralResponse<CartBooksDto>
                {
                    IsSuccess = true,
                    Message = "The Book is deleted from the cart successfully.",
                    Data = _mapper.Map<CartBooksDto>(check)
                };

            }
            catch (Exception ex)
            {
                return new GeneralResponse<CartBooksDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }


        }

        public async Task<GeneralResponse<CartBooksDto>> EditOnNumOfCopies(int cartId, int bookId, AddDto dto)
        {
            try
            {
                var cart = await _unitOfWork.CartRepository.GetByIdAsync(cartId);
                if (cart == null)
                {
                    return new GeneralResponse<CartBooksDto>
                    {
                        IsSuccess = false,
                        Message = "No Cart found with this Id."
                    };
                }

                var check = await _unitOfWork.CartBooksRepository.GetSpecificItem(n => n.cartId == cartId, i => i.bookId == bookId);
                if (check == null)
                {
                    return new GeneralResponse<CartBooksDto>
                    {
                        IsSuccess = false,
                        Message = "No book found in your cart with this Id."
                    };
                }

                var book = await _unitOfWork.BookRepository.GetByIdAsync(bookId);
                if(dto.wantedCopies > book.NumOfCopies || dto.wantedCopies <= 0)
                {
                    return new GeneralResponse<CartBooksDto>
                    {
                        IsSuccess = false,
                        Message = "Num of wanted copies isnot available."
                    };
                }

                check.WantedCopies = dto.wantedCopies;
                cart.TotalPrice -= check.Price;

                check.Price = dto.wantedCopies * book.Price;
                cart.TotalPrice += check.Price;

                _unitOfWork.CartRepository.Update(cart);
                _unitOfWork.CartBooksRepository.Update(check);
                await _unitOfWork.Complete();

                return new GeneralResponse<CartBooksDto>
                {
                    IsSuccess = true,
                    Message = "The number of wanted copies is changed successfully.",
                    Data = _mapper.Map<CartBooksDto>(check)
                };

            }
            catch (Exception ex)
            {
                return new GeneralResponse<CartBooksDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }

        }


        public async Task<GeneralResponse<List<CartBooksDto>>> Buying(int userId, int cartId)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return new GeneralResponse<List<CartBooksDto>>
                    {
                        IsSuccess = false,
                        Message = "No user found with this Id."
                    };
                }

                var cart = await _unitOfWork.CartRepository.SingleOrDefaultAsync(u => u.UserId == userId);
                if (cart.Id != cartId)
                {
                    return new GeneralResponse<List<CartBooksDto>>
                    {
                        IsSuccess = false,
                        Message = "Wrong Cart Id."
                    };
                }

                

                var cartbooks = await _unitOfWork.CartBooksRepository.Where(c => c.cartId == cartId);
                if (cartbooks == null)
                {
                    return new GeneralResponse<List<CartBooksDto>>
                    {
                        IsSuccess = false,
                        Message = "The Cart is Empty."
                    };
                }
                foreach (var cartbook in cartbooks)
                {
                    var book = await _unitOfWork.BookRepository.GetByIdAsync(cartbook.bookId);
                    if (book.NumOfCopies == 0)
                    {
                        var TheCart = await _unitOfWork.CartRepository.GetByIdAsync(cartbook.cartId);
                        TheCart.TotalPrice = TheCart.TotalPrice - cartbook.Price;
                        _unitOfWork.CartBooksRepository.Remove(cartbook);
                        await _unitOfWork.Complete();
                        return new GeneralResponse<List<CartBooksDto>>
                        {
                            IsSuccess = false,
                            Message = $"The book with this Id => {cartbook.bookId} is sold out."
                        };

                    }
                    if (book.NumOfCopies < cartbook.WantedCopies)
                    {
                        return new GeneralResponse<List<CartBooksDto>>
                        {
                            IsSuccess = false,
                            Message = $"The number of copies for the book with this Id => {cartbook.bookId} isnot available."
                        };

                    }
                }

                var bookUsers = cartbooks.Select(s => new BookUsers
                {
                    bookId = s.bookId,
                    userId = userId,
                    Date = DateTime.Now,
                    NumOfBoughtCopies = s.WantedCopies,
                    price = s.Price
                }).ToList();
                await _unitOfWork.BookUsersRepository.AddRangeAsync(bookUsers);

                foreach (var cartbook in cartbooks)
                {
                    var book = await _unitOfWork.BookRepository.GetByIdAsync(cartbook.bookId);
                    book.NumOfCopies -=  cartbook.WantedCopies;
                    book.NumOfSoldCopies += cartbook.WantedCopies;
                    _unitOfWork.BookRepository.Update(book);
                    await _unitOfWork.Complete();
                }
                

                cart.TotalPrice = 0;
                _unitOfWork.CartRepository.Update(cart);
                var data = _mapper.Map<List<CartBooksDto>>(cartbooks);
                _unitOfWork.CartBooksRepository.RemoveRange(cartbooks);
                await _unitOfWork.Complete();

                return new GeneralResponse<List<CartBooksDto>>
                {
                    IsSuccess = true,
                    Message = "The Process had been done successfully.",
                    Data = data
                };

            }
            catch (Exception ex)
            {
                return new GeneralResponse<List<CartBooksDto>>
                {
                    IsSuccess = false,
                    Message = "Something went wrong.",
                    Error = ex
                };
            }
        }

        
    }
}
