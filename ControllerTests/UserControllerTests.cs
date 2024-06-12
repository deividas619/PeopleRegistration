using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.API.Controllers;
using PeopleRegistration.BusinessLogic.Interfaces;
using Microsoft.AspNetCore.Http;
using ControllerUnitTests.Fixture;
using PeopleRegistration.Shared.Enums;

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
        [ControllerTestsFixture]
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
        [ControllerTestsFixture]
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
        [ControllerTestsFixture]
        public void Register_ThrowsException_WhenErrorOccurs(UserDto testUser)
        {
            // Arrange
            
            _mockUserService.Setup(s => s.Register(testUser.Username, testUser.Password)).Throws(new Exception("DB Connection Error"));

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _controller.Register(testUser));
            Assert.Equal("DB Connection Error", ex.Message);
        }

        [Theory]
        [ControllerTestsFixture]
        public void Login_ReturnsBadRequest_WhenUserDoesNotExist(LoginDto testUser)
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
        [ControllerTestsFixture]
        public void Login_ReturnsBadRequest_WhenPasswordIsEmpty(LoginDto testUser)
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
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Theory]
        [ControllerTestsFixture]
        public void Login_ReturnsBadRequest_WhenPasswordIsIncorrect(LoginDto testUser)
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
        [ControllerTestsFixture]
        public void Login_ReturnsJwtToken_WhenSuccessful(LoginDto testUser)
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
        [ControllerTestsFixture]
        public void Login_ThrowsException_WhenErrorOccurs(LoginDto testUser)
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
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ChangePasswordDto request = new ChangePasswordDto { OldPassword = "OLD_P@55w#rD!!", NewPassword = "P@55w#rD!!", NewPasswordAgain = "DIFFERENT_P@55w#rD!!" };
            ResponseDto response = new ResponseDto(false, "Old password is incorrect!");
            
            _mockUserService.Setup(s => s.ChangeUserPassword("testuser", request.OldPassword, request.NewPassword, request.NewPasswordAgain)).Returns(response);

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
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ChangePasswordDto request = new ChangePasswordDto { OldPassword = "OLD_P@55w#rD!!", NewPassword = "NEW_P@55w#rD!!", NewPasswordAgain = "DIFFERENT_NEW_P@55w#rD!!" };
            ResponseDto response = new ResponseDto(false, "New passwords do not match!");

            _mockUserService.Setup(s => s.ChangeUserPassword("testuser", request.OldPassword, request.NewPassword, request.NewPasswordAgain)).Returns(response);

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
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name,"testuser")
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ChangePasswordDto request = new ChangePasswordDto { OldPassword = "OLD_P@55w#rD!!", NewPassword = "NEW_P@55w#rD!!", NewPasswordAgain = "NEW_P@55w#rD!!" };
            ResponseDto response = new ResponseDto(true, "Password updated!");
            
            _mockUserService.Setup(s => s.ChangeUserPassword("testuser", request.OldPassword, request.NewPassword, request.NewPasswordAgain)).Returns(response);

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
            UserRole newRole = UserRole.Admin;
            ResponseDto response = new ResponseDto(false, "User does not exist!");
            
            _mockUserService.Setup(s => s.ChangeRole("testuser", newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole("testuser", newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeRole_ReturnsBadRequest_WhenNoAdminsLeft()
        {
            // Arrange
            UserRole newRole = UserRole.Regular;
            ResponseDto response = new ResponseDto(false, "There cannot be 0 admins in the system!");
            
            _mockUserService.Setup(s => s.ChangeRole("admin", newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole("admin", newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeRole_ReturnsBadRequest_WhenNewRoleIsSameAsOldRole()
        {
            // Arrange
            UserRole newRole = UserRole.Regular;
            ResponseDto response = new ResponseDto(false, "User already has that role!");
            
            _mockUserService.Setup(s => s.ChangeRole("testuser", newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole("testuser", newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeRole_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            UserRole newRole = UserRole.Admin;
            ResponseDto response = new ResponseDto(true, $"Role for User 'testuser' updated successfully!");
            
            _mockUserService.Setup(s => s.ChangeRole("testuser", newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole("testuser", newRole);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void ChangeRole_ReturnsBadRequest_WhenAdminUserTriesToBecomeRegular()
        {
            // Arrange
            UserRole newRole = UserRole.Regular;
            ResponseDto response = new ResponseDto(false, "An admin cannot downgrade themselves to a regular user!");
            
            _mockUserService.Setup(s => s.ChangeRole("admin", newRole)).Returns(response);

            // Act
            var result = _controller.ChangeRole("admin", newRole);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void GetUserActiveStatus_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            ResponseDto response = new ResponseDto(false, "User does not exist!");
            
            _mockUserService.Setup(s => s.GetUserActiveStatus("testuser")).Returns(response);

            // Act
            var result = _controller.GetUserActiveStatus("testuser");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void GetUserActiveStatus_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            ResponseDto response = new ResponseDto(true, $"User 'testuser' activity status is 'Active'!");
            
            _mockUserService.Setup(s => s.GetUserActiveStatus("testuser")).Returns(response);

            // Act
            var result = _controller.GetUserActiveStatus("testuser");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ResponseDto response = new ResponseDto(false, "User does not exist!");
            
            _mockUserService.Setup(s => s.ChangeUserActiveStatus("testuser", It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.ChangeUserActiveStatus("testuser");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void ChangeUserActiveStatus_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ResponseDto response = new ResponseDto(true, $"User 'testuser' activity status changed to 'Inactive' successfully!");
            
            _mockUserService.Setup(s => s.ChangeUserActiveStatus("testuser", It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.ChangeUserActiveStatus("testuser");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_ReturnsBadRequest_WhenTryingToDeactivateOwnAccount()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ResponseDto response = new ResponseDto(false, "Cannot deactivate your own account!");
            
            _mockUserService.Setup(s => s.ChangeUserActiveStatus("testuser", "testuser")).Returns(response);

            // Act
            var result = _controller.ChangeUserActiveStatus("testuser");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void DeleteUser_ReturnsBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ResponseDto response = new ResponseDto(false, "User does not exist!");
            
            _mockUserService.Setup(s => s.DeleteUser("testuser", It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.DeleteUser("testuser");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        [Fact]
        public void DeleteUser_ReturnsResponse_WhenSuccessful()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ResponseDto response = new ResponseDto(true, $"User 'testuser' deleted successfully!");
            
            _mockUserService.Setup(s => s.DeleteUser("testuser", It.IsAny<string>())).Returns(response);

            // Act
            var result = _controller.DeleteUser("testuser");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(response.Message, ((ResponseDto)okResult.Value).Message);
        }

        [Fact]
        public void DeleteUser_ReturnsBadRequest_WhenTryingToDeleteOwnAccount()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            ResponseDto response = new ResponseDto(false, "Cannot delete your own account!");
            
            _mockUserService.Setup(s => s.DeleteUser("testuser", "testuser")).Returns(response);

            // Act
            var result = _controller.DeleteUser("testuser");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(response.Message, badRequestResult.Value);
        }

        /*[Fact]
        public void test() //https://stackoverflow.com/questions/52842294/how-do-i-unit-test-model-validation-in-controllers-decorated-with-apicontroller
        {
            // Arrange
            var obj = new UserDto
            {

            };
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(obj, context, results, true);
        }*/
    }
}
