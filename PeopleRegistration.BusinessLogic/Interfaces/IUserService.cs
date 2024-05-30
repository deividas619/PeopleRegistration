using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.BusinessLogic.Interfaces
{
    public interface IUserService
    {
        ResponseDto Register(string username, string password);
        ResponseDto Login(string username, string password);
        ResponseDto ChangeUserPassword(string username, string oldPassword, string newPassword, string newPasswordAgain);
        ResponseDto ChangeRole(string username, UserRole newRole);
        ResponseDto SuspendUser(string username);
        ResponseDto DeleteUser(string username);
    }
}
