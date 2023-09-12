using Shopping.Core.Dtos;
using Shopping.Core.Dtos.RequestDtos;
using Shopping.Core.Dtos.ResponseDtos;

namespace Shopping.Services.IServices
{
    public interface IAuthServices
    {
        Task<GeneralResponse<UserDto>> SignUp(SignUpDto dto);
        Task<GeneralResponse<UserDto>> LogIn(LogInDto dto);
        Task<GeneralResponse<UserDto>> ResetPassword(ResetPasswordDto dto);
        Task<GeneralResponse<UserDto>> ForgetPassword(ForgetPasswordDto dto);


    }
}
