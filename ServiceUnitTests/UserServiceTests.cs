using Moq;
using PersonRegistration.BusinessLogic.Services;
using PersonRegistration.Database.Interfaces;
using PersonRegistration.Shared.Entities;

namespace BusinessLogicUnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public void Login_WithValidCredentials_ReturnsSuccessResponse()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";

            _userService.CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = passwordHash,
                PasswordSalt = passwordSalt,
            };

            _mockUserRepository.Setup(repo => repo.GetUser(username)).Returns(user);

            // Act
            var response = _userService.Login(username, password);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ReturnsErrorResponse()
        {
            // Arrange
            var Username = "testuser";
            var password = "testpassword";

            _mockUserRepository.Setup(repo => repo.GetUser(Username)).Returns((User)null);

            // Act
            var response = _userService.Login(Username, password);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Username does not exist!", response.Message);
        }

        [Fact]
        public void Register_WithNewUsername_ReturnsSuccessResponse()
        {
            // Arrange
            var Username = "newuser";
            var password = "newpassword";

            _mockUserRepository.Setup(repo => repo.GetUser(Username)).Returns((User)null);

            // Act
            var response = _userService.Register(Username, password);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User created!", response.Message);
            _mockUserRepository.Verify(repo => repo.SaveNewUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void Register_WithExistingUsername_ReturnsErrorResponse()
        {
            // Arrange
            var existingUsername = "existinguser";
            var password = "existingpassword";
            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Username = existingUsername,
                Password = [120, 227, 65, 14, 24],
                PasswordSalt = [68, 13, 2, 215, 33]
            };

            _mockUserRepository.Setup(repo => repo.GetUser(existingUsername)).Returns(existingUser);

            // Act
            var response = _userService.Register(existingUsername, password);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User already exists!", response.Message);
            _mockUserRepository.Verify(repo => repo.SaveNewUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void ChangeUserPassword_WithValidPasswords_ReturnsSuccessResponse()
        {
            // Arrange
            var username = "testuser";
            var oldPassword = "oldpassword";
            var newPassword = "newpassword";
            var newPasswordAgain = "newpassword";

            _userService.CreatePasswordHash(oldPassword, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = passwordHash,
                PasswordSalt = passwordSalt,
            };

            _mockUserRepository.Setup(repo => repo.GetUser(username)).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword(username, oldPassword, newPassword, newPasswordAgain);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("Password updated!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_WithInvalidOldPassword_ReturnsErrorResponse()
        {
            // Arrange
            var username = "testuser";
            var oldPassword = "wrongoldpassword";
            var newPassword = "newpassword";
            var newPasswordAgain = "newpassword";

            _userService.CreatePasswordHash("correctoldpassword", out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = passwordHash,
                PasswordSalt = passwordSalt,
            };

            _mockUserRepository.Setup(repo => repo.GetUser(username)).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword(username, oldPassword, newPassword, newPasswordAgain);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Old password is incorrect!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_WithNonMatchingNewPasswords_ReturnsErrorResponse()
        {
            // Arrange
            var username = "testuser";
            var oldPassword = "oldpassword";
            var newPassword = "newpassword";
            var newPasswordAgain = "differentnewpassword";

            _userService.CreatePasswordHash(oldPassword, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                Password = passwordHash,
                PasswordSalt = passwordSalt,
            };

            _mockUserRepository.Setup(repo => repo.GetUser(username)).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword(username, oldPassword, newPassword, newPasswordAgain);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Passwords are different!", response.Message);
        }
    }
}
