using PersonRegistration.Shared.DTOs;

namespace PersonRegistration.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        ResponseDto Register(string username, string password);
        ResponseDto Login(string username, string password);
        ResponseDto ChangeUserPassword(string username, string oldPassword, string newPassword, string newPasswordAgain);
    }
}
