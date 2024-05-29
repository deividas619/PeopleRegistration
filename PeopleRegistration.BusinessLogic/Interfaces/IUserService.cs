using PeopleRegistration.Shared.DTOs;

namespace PeopleRegistration.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        ResponseDto Register(string username, string password);
        ResponseDto Login(string username, string password);
        ResponseDto ChangeUserPassword(string username, string oldPassword, string newPassword, string newPasswordAgain);
    }
}
