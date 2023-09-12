using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;
using Shopping.Core.Dtos;
using Shopping.Core.Dtos.RequestDtos;
using Shopping.Core.Dtos.ResponseDtos;
using Shopping.Core.Helpers;
using Shopping.Core.IRepository;
using Shopping.Core.Models.AuthModule;
using Shopping.Core.Models.CartModule;
using Shopping.Services.IServices;

namespace Shopping.Services.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public AuthServices(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<GeneralResponse<UserDto>> SignUp(SignUpDto dto)
        {
            try
            {
                if (await _unitOfWork.UserRepository.AnyAsync(check: x => x.UserName == dto.UserName))
                {
                    return new GeneralResponse<UserDto>
                    {
                        IsSuccess = false,
                        Message = "This username is already taken."
                    };
                }
                if (await _unitOfWork.UserRepository.AnyAsync(check: x => x.Email == dto.Email))
                {
                    return new GeneralResponse<UserDto>
                    {
                        IsSuccess = false,
                        Message = "User is already Exist."
                    };
                }
                var cart = new Cart();
                var user = _mapper.Map<User>(dto);
                user.Password = HashingService.HashPassword(dto.Password);
                user.Cart = cart;

                await _unitOfWork.CartRepository.AddAsync(cart);
                await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.Complete();

                return new GeneralResponse<UserDto>
                {
                    IsSuccess = true,
                    Message = "Signed Up Successfully",
                    Data = _mapper.Map<UserDto>(dto)
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<UserDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong",
                    Error = ex
                };
            }

        }

        public async Task<GeneralResponse<UserDto>> LogIn(LogInDto dto)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.SingleOrDefaultAsync(x => x.Email == dto.Email && x.Password == HashingService.HashPassword(dto.Password));

                if (user == null)
                {
                    return new GeneralResponse<UserDto>
                    {
                        IsSuccess = false,
                        Message = "Email or Password is wrong."
                    };
                }
                return new GeneralResponse<UserDto>
                {
                    IsSuccess = true,
                    Message = "Logged In Successfully",
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<UserDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong",
                    Error = ex
                };
            }
        }

        public async Task<GeneralResponse<UserDto>> ResetPassword(ResetPasswordDto dto)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.SingleOrDefaultAsync(x => x.Email == dto.Email && x.Password == HashingService.HashPassword(dto.Password));

                if (!await _unitOfWork.UserRepository.AnyAsync(check: x => x.Email == dto.Email))
                {
                    return new GeneralResponse<UserDto>
                    {
                        IsSuccess = false,
                        Message = "No User Exist with this Email!"
                    };
                }

                if (user == null)
                {
                    return new GeneralResponse<UserDto>
                    {
                        IsSuccess = false,
                        Message = "Wrong Password."
                    };
                }

                user.Password = HashingService.HashPassword(dto.NewPassword);

                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.Complete();

                return new GeneralResponse<UserDto>
                {
                    IsSuccess = true,
                    Message = "Password is resetted successfully",
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<UserDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong",
                    Error = ex
                };
            }

        }

        public async Task<GeneralResponse<UserDto>> ForgetPassword(ForgetPasswordDto dto)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.SingleOrDefaultAsync(x => x.Email == dto.Email && x.UserName == dto.UserName);
                if (!await _unitOfWork.UserRepository.AnyAsync(check: x => x.UserName == dto.UserName))
                {
                    return new GeneralResponse<UserDto>
                    {
                        IsSuccess = false,
                        Message = "Wrong UserName!"
                    };
                }
                if (user == null)
                {
                    return new GeneralResponse<UserDto>
                    {
                        IsSuccess = false,
                        Message = "No User Exist with this Email!"
                    };
                }

                string DefaultPassword = _configuration.GetValue<string>("DefaultPassword");

                user.Password = HashingService.HashPassword(DefaultPassword);
                _unitOfWork.UserRepository.Update(user);
                await _unitOfWork.Complete();

                return new GeneralResponse<UserDto>
                {
                    IsSuccess = true,
                    Message = "User has a temperory Default Password",
                };
            }
            catch (Exception ex)
            {
                return new GeneralResponse<UserDto>
                {
                    IsSuccess = false,
                    Message = "Something went wrong",
                    Error = ex
                };
            }

        }





    }
}
