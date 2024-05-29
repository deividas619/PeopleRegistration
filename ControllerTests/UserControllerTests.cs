using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeopleRegistration.Shared.DTOs;
using System.Security.Claims;
using PeopleRegistration.API.Controllers;
using PeopleRegistration.BusinessLogic.Interfaces;

namespace ControllerUnitTests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IJwtService> _jwtService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _jwtService = new Mock<IJwtService>();
            _controller = new UserController(_mockUserService.Object, _jwtService.Object);
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsIsSuccessTrue()
        {
            // Arrange
            var userDto = new UserDto { Username = "test", Password = "tt" };
            var responseDto = new ResponseDto(isSuccess: true, message: "User logged in");
            _mockUserService.Setup(service => service.Login(userDto.Username, userDto.Password)).Returns(responseDto);

            // Act
            var result = _controller.Login(userDto.Username, userDto.Password);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(200, okObjectResult.StatusCode);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var userDto = new UserDto { Username = "testuser", Password = "testpassword" };
            var responseDto = new ResponseDto(isSuccess: false, message: "Username or password does not match");
            _mockUserService.Setup(service => service.Login(userDto.Username, userDto.Password)).Returns(responseDto);

            // Act
            var result = _controller.Login(userDto.Username, userDto.Password);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.NotNull(badRequestObjectResult.Value);
            var resultMessage = Assert.IsType<string>(badRequestObjectResult.Value);
            Assert.Equal("Username or password does not match", resultMessage);
        }

        [Theory]
        [InlineData(null, "password")]
        [InlineData("username", null)]
        [InlineData("", "password")]
        [InlineData("username", "")]
        public void Login_WithNullOrEmptyCredentials_ReturnsBadRequest(string username, string password)
        {
            // Arrange
            var responseDto = new ResponseDto(isSuccess: false, message: "Invalid credentials");
            _mockUserService.Setup(service => service.Login(username, password)).Returns(responseDto);

            // Act
            var result = _controller.Login(username, password);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public void Register_WithValidUser_ReturnsIsSuccessTrue()
        {
            // Arrange
            var userDto = new UserDto { Username = "newuser", Password = "newpassword" };
            var responseDto = new ResponseDto(isSuccess: true);
            _mockUserService.Setup(service => service.Register(userDto.Username, userDto.Password)).Returns(responseDto);

            // Act
            var result = _controller.Register(userDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
            Assert.NotNull(actionResult.Value);
            var resultDto = Assert.IsType<ResponseDto>(actionResult.Value);
            Assert.True(resultDto.IsSuccess);
        }

        [Fact]
        public void Register_WithExistingUser_ReturnsBadRequestObjectResult()
        {
            // Arrange
            var userDto = new UserDto { Username = "existinguser", Password = "existingpassword" };
            var responseDto = new ResponseDto(isSuccess: false, message: "User already exists");
            _mockUserService.Setup(service => service.Register(userDto.Username, userDto.Password)).Returns(responseDto);

            // Act
            var result = _controller.Register(userDto);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var resultMessage = Assert.IsType<string>(badRequestObjectResult.Value);
            Assert.Equal("User already exists", resultMessage);
        }

        [Theory]
        [InlineData(null, "password")]
        [InlineData("username", null)]
        [InlineData("", "password")]
        [InlineData("username", "")]
        public void Register_WithNullOrEmptyCredentials_ReturnsBadRequest(string username, string password)
        {
            // Arrange
            var userDto = new UserDto { Username = username, Password = password };
            var responseDto = new ResponseDto(isSuccess: false, message: "Invalid credentials");
            _mockUserService.Setup(service => service.Register(username, password)).Returns(responseDto);

            // Act
            var result = _controller.Register(userDto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
        public void ChangePassword_WithValidPasswords_ReturnsIsSuccessTrue()
        {
            // Arrange
            var oldPassword = "oldPassword";
            var newPassword = "newPassword";
            var newPasswordAgain = "newPassword";
            var username = "testUser";
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, username) })) };
            var responseDto = new ResponseDto(isSuccess: true);
            _mockUserService.Setup(service => service.ChangeUserPassword(username, oldPassword, newPassword, newPasswordAgain)).Returns(responseDto);

            // Act
            var result = _controller.ChangePassword(oldPassword, newPassword, newPasswordAgain);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
            Assert.True(actionResult.Value.IsSuccess);
        }

        [Fact]
        public void ChangePassword_WithInvalidPasswords_ReturnsBadRequest()
        {
            // Arrange
            var oldPassword = "oldPassword";
            var newPassword = "newPassword";
            var newPasswordAgain = "newPassword";
            var username = "testUser";
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, username) })) };
            var responseDto = new ResponseDto(isSuccess: false, message: "Invalid password");
            _mockUserService.Setup(service => service.ChangeUserPassword(username, oldPassword, newPassword, newPasswordAgain)).Returns(responseDto);

            // Act
            var result = _controller.ChangePassword(oldPassword, newPassword, newPasswordAgain);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ResponseDto>>(result);
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }
    }
}
