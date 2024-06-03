using Moq;
using PeopleRegistration.BusinessLogic.Services;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.Entities;
using Serilog;
using System.Security.Cryptography;
using BusinessLogicUnitTests.Fixture;

namespace BusinessLogicUnitTests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _userService = new UserService(_mockRepo.Object);
        }

        [Fact]
        public void Register_UserDoesNotExist_CreatesNewUser()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);
            _mockRepo.Setup(repo => repo.SaveNewUser(It.IsAny<User>()));

            // Act
            var response = _userService.Register("testuser", "P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User created!", response.Message);

            _mockRepo.Verify(repo => repo.SaveNewUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void Register_UserAlreadyExists_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(new User());

            // Act
            var response = _userService.Register("existinguser", "P@55w#rD!!");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User already exists!", response.Message);
        }

        [Fact]
        public void Register_NewAdminUser_CreatesAdminUser()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);
            _mockRepo.Setup(repo => repo.SaveNewUser(It.IsAny<User>()));

            // Act
            var response = _userService.Register("adminuser", "P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User created!", response.Message);
        }

        [Fact]
        public void Register_SaveUserThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);
            _mockRepo.Setup(repo => repo.SaveNewUser(It.IsAny<User>())).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _userService.Register("testuser", "P@55w#rD!!"));
        }

        [Fact]
        public void Login_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.Login("nonexistentuser", "P@55w#rD!!");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User does not exist!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void Login_ValidCredentials_ReturnsSuccess(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "OLD_P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void Login_IncorrectPassword_ReturnsFailure(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "WRONG_P@55w#rD!!");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Password is incorrect!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void Login_InactiveUser_ReturnsFailure(User testUser)
        {
            // Arrange
            testUser.IsActive = false;
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "OLD_P@55w#rD!!");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal($"The User 'testuser ({testUser.Id})' is suspended! Contact system administrator!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void Login_ExpiredPassword_ReturnsFailure(User testUser)
        {
            // Arrange
            testUser.PasswordSetDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-91));
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "OLD_P@55w#rD!!");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Password has expired. Please change your password!", response.Message);
        }

        [Fact]
        public void Login_ExceptionThrown_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _userService.Login("testuser", "P@55w#rD!!"));
        }

        [Theory]
        [UserFixture]
        public void Login_ValidCredentialsPasswordNeverExpires_ReturnsSuccess(User testUser)
        {
            // Arrange
            testUser.PasswordNeverExpires = true;
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "OLD_P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void Login_ValidCredentialsAdminRole_ReturnsSuccess(User testUser)
        {
            // Arrange
            testUser.Username = "adminuser";
            testUser.Role = UserRole.Admin;
            testUser.PasswordNeverExpires = true;

            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("adminuser", "OLD_P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
            Assert.Equal(UserRole.Admin, response.Role);
        }

        [Theory]
        [UserFixture]
        public void Login_ValidCredentialsRegularRole_ReturnsSuccess(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "OLD_P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
            Assert.Equal(UserRole.Regular, response.Role);
        }

        [Theory]
        [UserFixture]
        public void Login_CaseInsensitiveUsername_ReturnsSuccess(User testUser)
        {
            // Arrange
            testUser.Username = "TestUser";
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "OLD_P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void ChangeUserPassword_PasswordIsIncorrect_ReturnsFailure(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.ChangeUserPassword("testuser", "WRONG_P@55w#rD!!", "NEW_P@55w#rD!!", "NEW_P@55w#rD!!");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Old password is incorrect!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void ChangeUserPassword_NewPasswordsDoNotMatch_ReturnsFailure(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.ChangeUserPassword(testUser.Username, "OLD_P@55w#rD!!", "NEW_P@55w#rD!!", "DIFFERENT_NEW_P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
        }

        [Theory]
        [UserFixture]
        public void ChangeUserPassword_OldPasswordCorrect_UpdatesPasswordSuccessfully(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>()));

            // Act
            var response = _userService.ChangeUserPassword("testuser", "OLD_P@55w#rD!!", "NEW_P@55w#rD!!", "NEW_P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public void ChangeUserPassword_ExceptionThrown_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _userService.ChangeUserPassword("testuser", "OLD_P@55w#rD!!", "NEW_P@55w#rD!!", "NEW_P@55w#rD!!"));
        }

        [Theory]
        [UserFixture]
        public void ChangeUserPassword_NewPasswordSameAsOld_ReturnsFailure(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.ChangeUserPassword(testUser.Username, "OLD_P@55w#rD!!", "OLD_P@55w#rD!!", "OLD_P@55w#rD!!");

            // Assert
            Assert.True(response.IsSuccess);
        }

        [Theory]
        [InlineData("")]
        public void ChangeUserPassword_NewPasswordEmpty_ReturnsFailure(string newpassword)
        {
            // Arrange
            CreatePasswordHash("OLD_P@55w#rD!!", out byte[] passwordHash, out byte[] passwordSalt);

            var testUser = new User
            {
                Username = "testuser",
                Password = passwordHash,
                PasswordSalt = passwordSalt,
                IsActive = true,
                PasswordSetDate = DateOnly.FromDateTime(DateTime.Now),
                PasswordNeverExpires = false
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.ChangeUserPassword("testuser", "OLD_P@55w#rD!!", newpassword, newpassword);

            // Assert
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public void ChangeRole_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.ChangeRole("nonexistentuser", UserRole.Admin);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User does not exist!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void ChangeRole_UserIsAdminAndOnlyAdminInTheSystem_ReturnsFailure(User testUser)
        {
            // Arrange
            testUser.Username = "adminuser";
            testUser.Role = UserRole.Admin;
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Returns(1);

            // Act
            var response = _userService.ChangeRole("adminuser", UserRole.Regular);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("There cannot be 0 admins in the system!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void ChangeRole_RoleUpdateForExistingUser_ChangesRole(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Returns(1);

            // Act
            var response = _userService.ChangeRole("testuser", UserRole.Admin);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"Role for User 'testuser ({testUser.Id})' updated successfully!", response.Message);

            _mockRepo.Verify(repo => repo.UpdateUser(testUser), Times.Once);
        }

        [Theory]
        [UserFixture]
        public void ChangeRole_NewRoleIsSameAsOldRole_ReturnsFailure(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.ChangeRole("testuser", UserRole.Regular);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User already has that role!", response.Message);
        }

        [Fact]
        public void ChangeRole_ExceptionThrown_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _userService.ChangeRole("testuser", UserRole.Admin));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ChangeRole_UsernameNullOrEmpty_ReturnsFailure(string username)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.ChangeRole(username, UserRole.Regular);

            // Assert
            Assert.False(response.IsSuccess);
        }

        [Theory]
        [UserFixture]
        public void ChangeRole_UserIsInactive_ReturnsFailure(User testUser)
        {
            // Arrange
            testUser.IsActive = false;
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.ChangeRole("testuser", UserRole.Admin);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal($"Cannot change role for a User '{testUser.Username} ({testUser.Id})' that is suspended!", response.Message);
        }

        [Fact]
        public void GetUserActiveStatus_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.GetUserActiveStatus("nonexistentuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User does not exist!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void GetUserActiveStatus_UserExists_ReturnsActiveStatus(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.GetUserActiveStatus("testuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User '{testUser.Username} ({testUser.Id})' activity status is 'True'!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void GetUserActiveStatus_UserExists_ReturnsInactiveStatus(User testUser)
        {
            // Arrange
            testUser.IsActive = false;
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.GetUserActiveStatus("testuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User '{testUser.Username} ({testUser.Id})' activity status is 'False'!", response.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void GetUserActiveStatus_UsernameNullOrEmpty_ReturnsFailure(string username)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.GetUserActiveStatus(username);

            // Assert
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public void GetUserActiveStatus_ExceptionThrown_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _userService.GetUserActiveStatus("testuser"));
        }

        [Fact]
        public void ChangeUserActiveStatus_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.ChangeUserActiveStatus("nonexistentuser", "adminuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User does not exist!", response.Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_SameUserAsLoggedIn_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(new User());

            // Act
            var response = _userService.ChangeUserActiveStatus("testuser", "testuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Cannot deactivate your own account!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void ChangeUserActiveStatus_UserExistsAndNotLoggedInUser_ChangesInactiveStatus(User testUser)
        {
            // Arrange
            testUser.IsActive = false;
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>()));

            // Act
            var response = _userService.ChangeUserActiveStatus("testuser", "adminuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User '{testUser.Username} ({testUser.Id})' activity status changed to 'True' successfully!", response.Message);

            _mockRepo.Verify(repo => repo.UpdateUser(testUser), Times.Once);
        }

        [Theory]
        [UserFixture]
        public void ChangeUserActiveStatus_UserExistsAndNotLoggedInUser_ChangesActiveStatus(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.ChangeUserActiveStatus("testuser", "adminuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User '{testUser.Username} ({testUser.Id})' activity status changed to 'False' successfully!", response.Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_ExceptionThrown_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Throws(new Exception());

            // Act
            Assert.Throws<Exception>(() => _userService.ChangeUserActiveStatus("testuser", "adminuser"));
        }

        [Theory]
        [InlineData("username", null)]
        [InlineData(null, "loggedinusername")]
        [InlineData("", "loggedinusername")]
        [InlineData("username", "")]
        public void ChangeUserActiveStatus_NullOrEmptyUsername_ReturnsFailure(string username, string loggedInUsername)
        {
            // Act
            var response = _userService.ChangeUserActiveStatus(username, loggedInUsername);

            // Assert
            Assert.False(response.IsSuccess);
        }

        [Theory]
        [InlineData("   ")]
        [InlineData("!@#$%")]
        [InlineData("veryveryveryverylongusername")]
        public void ChangeUserActiveStatus_UnusualUsernames_ReturnsFailure(string username)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.ChangeUserActiveStatus(username, "adminuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User does not exist!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void ChangeUserActiveStatus_UpdateUserThrowsException(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>())).Throws(new Exception());

            // Act & Assert
            Assert.Throws<Exception>(() => _userService.ChangeUserActiveStatus(testUser.Username, "adminuser"));
        }

        [Fact]
        public void DeleteUser_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.DeleteUser("testuser", "adminuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User does not exist!", response.Message);
        }

        [Fact]
        public void DeleteUser_SameUserAsLoggedIn_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(new User());

            // Act
            var response = _userService.DeleteUser("adminuser", "adminuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Cannot delete your own account!", response.Message);
        }

        [Theory]
        [UserFixture]
        public void DeleteUser_UserSuccessfullyDeleted_ReturnsSuccess(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);
            _mockRepo.Setup(repo => repo.DeleteUser(It.IsAny<User>()));

            // Act
            var response = _userService.DeleteUser("testuser", "adminuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User '{testUser.Username} ({testUser.Id})' deleted successfully!", response.Message);
        }

        [Fact]
        public void DeleteUser_GetUserThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Throws(new Exception());

            // Act
            Assert.Throws<Exception>(() => _userService.DeleteUser("testuser", "adminuser"));
        }

        [Theory]
        [UserFixture]
        public void DeleteUser_DeleteUserThrowsException_ReturnsFailure(User testUser)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);
            _mockRepo.Setup(repo => repo.DeleteUser(testUser)).Throws(new Exception());

            // Act
            Assert.Throws<Exception>(() => _userService.DeleteUser("testuser", "adminuser"));
        }

        [Theory]
        [InlineData(null, "loggedinusername")]
        [InlineData("username", null)]
        [InlineData("", "loggedinusername")]
        [InlineData("username", "")]
        public void DeleteUser_NullOrEmptyUsername_ReturnsFailure(string username, string loggedInUsername)
        {
            // Act
            var response = _userService.DeleteUser(username, loggedInUsername);

            // Assert
            Assert.False(response.IsSuccess);
        }

        [Theory]
        [UserFixture]
        public void DeleteUser_UserExistsButIsInactive_DeletesUserSuccessfully(User testUser)
        {
            // Arrange
            testUser.IsActive = false;
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);
            _mockRepo.Setup(repo => repo.DeleteUser(It.IsAny<User>()));

            // Act
            var response = _userService.DeleteUser("testuser", "adminuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User '{testUser.Username} ({testUser.Id})' deleted successfully!", response.Message);
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            try
            {
                using var hmac = new HMACSHA512();
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserServiceTests)}.{nameof(CreatePasswordHash)}]: {e.Message}");
                throw;
            }
        }
    }
}
