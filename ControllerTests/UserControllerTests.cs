using Microsoft.AspNetCore.Mvc;
using Moq;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.API.Controllers;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.Entities;

namespace ControllerUnitTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockJwtService = new Mock<IJwtService>();
            _controller = new UserController(_mockUserService.Object, _mockJwtService.Object);
        }

        [Fact]
        public void Register_ReturnsBadRequest_WhenUserExists()
        {
            // Arrange
            UserDto request = new UserDto { Username = "testuser", Password = "password" };
            ResponseDto response = new ResponseDto(false, "User already exists");
            _mockUserService.Setup(s => s.Register(request.Username, request.Password)).Returns(response);

            // Act
            var result = _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void Register_ReturnsBadRequest_WhenRequestIsNull()
        {
            // Act
            var result = _controller.Register(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal();
        }

        [Fact]
        public void Register_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            UserDto request = new UserDto { Username = "testuser", Password = "password" };
            ResponseDto response = new ResponseDto(true, "User created!");
            _mockUserService.Setup(s => s.Register(request.Username, request.Password)).Returns(response);

            // Act
            var result = _controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void Register_ThrowsException_WhenErrorOccurs()
        {
            // Arrange
            UserDto request = new UserDto { Username = "testuser", Password = "password" };
            _mockUserService.Setup(s => s.Register(request.Username, request.Password)).Throws(new Exception("DB Connection Error"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _controller.Register(request));
            Assert.Equal("DB Connection Error", ex.Message);
        }

        [Fact]
        public void Login_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "testuser";
            string password = "password";
            ResponseDto response = new ResponseDto(false, "User does not exist");
            _mockUserService.Setup(s => s.Login(username, password)).Returns(response);

            // Act
            var result = _controller.Login(username, password);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void Login_ReturnsBadRequest_WhenUsernameIsEmpty()
        {
            // Arrange
            string username = "";
            string password = "password";

            // Act
            var result = _controller.Login(username, password);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal();
        }

        [Fact]
        public void Login_ReturnsBadRequest_WhenPasswordIsEmpty()
        {
            // Arrange
            string username = "testuser";
            string password = "";

            // Act
            var result = _controller.Login(username, password);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result.Result);
            Assert.Equal();
        }

        [Fact]
        public void Login_ReturnsBadRequest_WhenPasswordIsIncorrect()
        {
            // Arrange
            string username = "testuser";
            string password = "wrongpassword";
            ResponseDto response = new ResponseDto(false, "Password is incorrect!");
            _mockUserService.Setup(s => s.Login(username, password)).Returns(response);

            // Act
            var result = _controller.Login(username, password);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void Login_ReturnsJwtToken_WhenSuccessful()
        {
            // Arrange
            string username = "testuser";
            string password = "password";
            ResponseDto userServiceResponse = new ResponseDto(true, "User logged in!", UserRole.Regular);
            string jwtToken = "jwtToken";
            _mockUserService.Setup(s => s.Login(username, password)).Returns(userServiceResponse);
            _mockJwtService.Setup(s => s.GetJwtToken(username, UserRole.Regular)).Returns(jwtToken);

            // Act
            var result = _controller.Login(username, password);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(jwtToken, okResult.Value);
        }

        [Fact]
        public void Login_ThrowsException_WhenErrorOccurs()
        {
            // Arrange
            string username = "testuser";
            string password = "password";
            _mockUserService.Setup(s => s.Login(username, password)).Throws(new Exception("DB Connection Error"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _controller.Login(username, password));
            Assert.Equal("DB Connection Error", ex.Message);
        }

        [Fact]
        public void ChangePassword_ReturnsBadRequest_WhenOldPasswordIsIncorrect()
        {
            // Arrange
            string username = "testuser";
            ChangePassword request = new ChangePassword { OldPassword = "oldPwd", NewPassword = "newPwd", NewPasswordAgain = "newPwd" };
            ResponseDto response = new ResponseDto(false, "Old password is incorrect!");
            _mockUserService.Setup(s => s.ChangeUserPassword(username, request.OldPassword, request.NewPassword, request.NewPasswordAgain)).Returns(response);

            // Act
            var result = _controller.ChangePassword(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangePassword_ReturnsBadRequest_WhenNewPasswordsDoNotMatch()
        {
            // Arrange
            string username = "testuser";
            ChangePassword request = new ChangePassword { OldPassword = "oldPwd", NewPassword = "newPwd", NewPasswordAgain = "differentNewPwd" };
            ResponseDto response = new ResponseDto(false, "New passwords do not match!");
            _mockUserService.Setup(s => s.ChangeUserPassword(username, request.OldPassword, request.NewPassword, request.NewPasswordAgain)).Returns(response);

            // Act
            var result = _controller.ChangePassword(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangePassword_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            string username = "testuser";
            ChangePassword request = new ChangePassword { OldPassword = "oldPwd", NewPassword = "newPwd", NewPasswordAgain = "newPwd" };
            ResponseDto response = new ResponseDto(true, "Password updated!");
            _mockUserService.Setup(s => s.ChangeUserPassword(username, request.OldPassword, request.NewPassword, request.NewPasswordAgain)).Returns(response);

            // Act
            var result = _controller.ChangePassword(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void ChangeRole_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "testuser";
            UserRole newRole = UserRole.Admin;
            ResponseDto response = new ResponseDto(false, "User does not exist!");
            _mockUserService.Setup(s => s.ChangeRole(username, newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole(username, newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeRole_ReturnsBadRequest_WhenNoAdminsLeft()
        {
            // Arrange
            string username = "admin";
            UserRole newRole = UserRole.Regular;
            ResponseDto response = new ResponseDto(false, "There cannot be 0 admins in the system!");
            _mockUserService.Setup(s => s.ChangeRole(username, newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole(username, newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeRole_ReturnsBadRequest_WhenUserIsNotAdmin()
        {
            // Arrange
            string username = "testuser";
            UserRole newRole = UserRole.Admin;
            ResponseDto response = new ResponseDto(false, "Only admin can change roles!");
            _mockUserService.Setup(s => s.ChangeRole(username, newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole(username, newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeRole_ReturnsBadRequest_WhenNewRoleIsSameAsOldRole()
        {
            // Arrange
            string username = "testuser";
            UserRole newRole = UserRole.Regular;
            ResponseDto response = new ResponseDto(false, "User already has that role!");
            _mockUserService.Setup(s => s.ChangeRole(username, newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole(username, newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeRole_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            string username = "testuser";
            UserRole newRole = UserRole.Admin;
            ResponseDto response = new ResponseDto(true, $"Role for User '{username} updated successfully!");
            _mockUserService.Setup(s => s.ChangeRole(username, newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole(username, newRole);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void ChangeRole_ReturnsBadRequest_WhenRegularUserTriesToBecomeAdmin()
        {
            // Arrange
            string username = "regular";
            UserRole newRole = UserRole.Admin;
            ResponseDto response = new ResponseDto(false, "A regular user can't be made an admin!");
            _mockUserService.Setup(s => s.ChangeRole(username, newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole(username, newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeRole_ReturnsBadRequest_WhenAdminUserTriesToBecomeRegular()
        {
            // Arrange
            string username = "admin";
            UserRole newRole = UserRole.Regular;
            ResponseDto response = new ResponseDto(false, "An admin user can't downgrade to a regular user!");
            _mockUserService.Setup(s => s.ChangeRole(username, newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole(username, newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void GetUserActiveStatus_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(false, "User does not exist!");
            _mockUserService.Setup(s => s.GetUserActiveStatus(username)).Returns(response);

            // Act
            var result = _controller.GetUserActiveStatus(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void GetUserActiveStatus_ReturnsBadRequest_WhenUserIsNotAdmin()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(false, "Only admin can fetch active status!");
            _mockUserService.Setup(s => s.GetUserActiveStatus(username)).Returns(response);

            // Act
            var result = _controller.GetUserActiveStatus(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void GetUserActiveStatus_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(true, $"User '{username}' activity status is 'Active'!");
            _mockUserService.Setup(s => s.GetUserActiveStatus(username)).Returns(response);

            // Act
            var result = _controller.GetUserActiveStatus(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(false, "User does not exist!");
            _mockUserService.Setup(s => s.ChangeUserActiveStatus(username, It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.ChangeUserActiveStatus(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeUserActiveStatus_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(true, $"User '{username}' activity status changed to 'Inactive' successfully!");
            _mockUserService.Setup(s => s.ChangeUserActiveStatus(username, It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.ChangeUserActiveStatus(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_ReturnsBadRequest_WhenTryingToDeactivateOwnAccount()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(false, "Cannot deactivate your own account!");
            _mockUserService.Setup(s => s.ChangeUserActiveStatus(username, username)).Returns(response);

            // Act
            var result = _controller.ChangeUserActiveStatus(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeUserActiveStatus_ReturnsBadRequest_WhenUserIsNotAdmin()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(false, "Only admin can change active status!");
            _mockUserService.Setup(s => s.ChangeUserActiveStatus(username, It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.ChangeUserActiveStatus(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void DeleteUser_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(false, "User does not exist!");
            _mockUserService.Setup(s => s.DeleteUser(username, It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.DeleteUser(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void DeleteUser_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(true, $"User '{username}' deleted successfully!");
            _mockUserService.Setup(s => s.DeleteUser(username, It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.DeleteUser(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void DeleteUser_ReturnsBadRequest_WhenTryingToDeleteOwnAccount()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(false, "Cannot delete your own account!");
            _mockUserService.Setup(s => s.DeleteUser(username, username)).Returns(response);

            // Act
            var result = _controller.DeleteUser(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void DeleteUser_ReturnsBadRequest_WhenUserIsNotAdmin()
        {
            // Arrange
            string username = "testuser";
            ResponseDto response = new ResponseDto(false, "Only admin can delete users!");
            _mockUserService.Setup(s => s.DeleteUser(username, It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.DeleteUser(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }
    }
}
