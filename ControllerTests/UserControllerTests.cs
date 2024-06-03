using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.API.Controllers;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.Entities;
using Microsoft.AspNetCore.Http;
using ControllerUnitTests.Fixture;

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

        [Theory]
        [UserControllerTestsFixture]
        public void Register_ReturnsBadRequest_WhenUserExists(UserDto testUser)
        {
            // Arrange
            ResponseDto response = new ResponseDto(false, "User already exists");
            _mockUserService.Setup(s => s.Register(testUser.Username, testUser.Password)).Returns(response);

            // Act
            var result = _controller.Register(testUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Theory]
        [UserControllerTestsFixture]
        public void Register_ReturnsResponse_WhenSuccessful(UserDto testUser)
        {
            // Arrange
            ResponseDto response = new ResponseDto(true, "User created!");
            _mockUserService.Setup(s => s.Register(testUser.Username, testUser.Password)).Returns(response);

            // Act
            var result = _controller.Register(testUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Theory]
        [UserControllerTestsFixture]
        public void Register_ThrowsException_WhenErrorOccurs(UserDto testUser)
        {
            // Arrange
            _mockUserService.Setup(s => s.Register(testUser.Username, testUser.Password)).Throws(new Exception("DB Connection Error"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _controller.Register(testUser));
            Assert.Equal("DB Connection Error", ex.Message);
        }

        [Theory]
        [UserControllerTestsFixture]
        public void Login_ReturnsBadRequest_WhenUserDoesNotExist(Login testUser)
        {
            // Arrange
            ResponseDto response = new ResponseDto(false, "User does not exist");
            _mockUserService.Setup(s => s.Login(testUser.Username, testUser.Password)).Returns(response);

            // Act
            var result = _controller.Login(testUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Theory]
        [UserControllerTestsFixture]
        public void Login_ReturnsBadRequest_WhenPasswordIsEmpty(Login testUser)
        {
            // Arrange
            testUser.Password = "";
            var mockService = new Mock<IUserService>();
            mockService.Setup(service => service.Login(testUser.Username, testUser.Password)).Returns(new ResponseDto(false, "Password is incorrect!"));

            var mockJwtService = new Mock<IJwtService>();

            var _controller = new UserController(mockService.Object, mockJwtService.Object);

            // Act
            var result = _controller.Login(testUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Theory]
        [UserControllerTestsFixture]
        public void Login_ReturnsBadRequest_WhenPasswordIsIncorrect(Login testUser)
        {
            // Arrange
            ResponseDto response = new ResponseDto(false, "Password is incorrect!");
            _mockUserService.Setup(s => s.Login(testUser.Username, testUser.Password)).Returns(response);

            // Act
            var result = _controller.Login(testUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Theory]
        [UserControllerTestsFixture]
        public void Login_ReturnsJwtToken_WhenSuccessful(Login testUser)
        {
            // Arrange
            ResponseDto userServiceResponse = new ResponseDto(true, "User logged in!", UserRole.Regular);
            string jwtToken = "jwtToken";
            _mockUserService.Setup(s => s.Login(testUser.Username, testUser.Password)).Returns(userServiceResponse);
            _mockJwtService.Setup(s => s.GetJwtToken(testUser.Username, UserRole.Regular)).Returns(jwtToken);

            // Act
            var result = _controller.Login(testUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(jwtToken, okResult.Value);
        }

        [Theory]
        [UserControllerTestsFixture]
        public void Login_ThrowsException_WhenErrorOccurs(Login testUser)
        {
            // Arrange
            _mockUserService.Setup(s => s.Login(testUser.Username, testUser.Password)).Throws(new Exception("DB Connection Error"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _controller.Login(testUser));
            Assert.Equal("DB Connection Error", ex.Message);
        }

        [Fact]
        public void ChangePassword_ReturnsBadRequest_WhenOldPasswordIsIncorrect()
        {
            // Arrange
            string username = "testuser";

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ChangePassword request = new ChangePassword { OldPassword = "OLD_P@55w#rD!!", NewPassword = "P@55w#rD!!", NewPasswordAgain = "DIFFERENT_P@55w#rD!!" };
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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ChangePassword request = new ChangePassword { OldPassword = "OLD_P@55w#rD!!", NewPassword = "NEW_P@55w#rD!!", NewPasswordAgain = "DIFFERENT_NEW_P@55w#rD!!" };
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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ChangePassword request = new ChangePassword { OldPassword = "OLD_P@55w#rD!!", NewPassword = "NEW_P@55w#rD!!", NewPasswordAgain = "NEW_P@55w#rD!!" };
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
        public void ChangeRole_ReturnsBadRequest_WhenAdminUserTriesToBecomeRegular()
        {
            // Arrange
            string username = "admin";
            UserRole newRole = UserRole.Regular;
            ResponseDto response = new ResponseDto(false, "An admin cannot downgrade themselves to a regular user!");
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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ResponseDto response = new ResponseDto(false, "Cannot deactivate your own account!");
            _mockUserService.Setup(s => s.ChangeUserActiveStatus(username, username)).Returns(response);

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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

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

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, username)
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ResponseDto response = new ResponseDto(false, "Cannot delete your own account!");
            _mockUserService.Setup(s => s.DeleteUser(username, username)).Returns(response);

            // Act
            var result = _controller.DeleteUser(username);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }
    }
}
