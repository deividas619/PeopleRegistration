using Moq;
using PeopleRegistration.BusinessLogic.Services;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.Entities;
using System.Text;

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
        public void Register_UsernameDoesNotMeetRequirements_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.Register("abc", "password"); // assuming "abc" doesn't meet username requirements

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Username does not meet requirements!", response.Message);
        }

        [Fact]
        public void Register_UserDoesNotExist_CreatesNewUser()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);
            _mockRepo.Setup(repo => repo.SaveNewUser(It.IsAny<User>()));

            // Act
            var response = _userService.Register("newuser", "newpassword");

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
            var response = _userService.Register("existinguser", "testpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User already exists!", response.Message);
        }

        [Fact]
        public void Register_PasswordDoesNotMeetRequirements_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.Register("newuser", "123"); // assuming "123" doesn't meet password requirements

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Password does not meet requirements!", response.Message);
        }

        [Fact]
        public void Register_NewAdminUser_CreatesAdminUser()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);
            _mockRepo.Setup(repo => repo.SaveNewUser(It.IsAny<User>()));

            // Act
            var response = _userService.Register("admin", "password");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User created!", response.Message);
            _mockRepo.Verify(repo => repo.SaveNewUser(It.Is<User>(u => u.Role == UserRole.Admin)), Times.Once);
        }

        [Fact]
        public void Register_SaveUserThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);
            _mockRepo.Setup(repo => repo.SaveNewUser(It.IsAny<User>())).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _userService.Register("newuser", "newpassword"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Register_UsernameNullOrEmpty_ReturnsFailure(string username)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.Register(username, "password");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Username cannot be null or empty!", response.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Register_PasswordNullOrEmpty_ReturnsFailure(string password)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.Register("newuser", password);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Password cannot be null or empty!", response.Message);
        }

        [Fact]
        public void Login_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.Login("nonexistentuser", "testpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User does not exist!", response.Message);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsSuccess()
        {
            // Arrange
            var testUser = new User
            {
                Username = "testuser",
                Password = Encoding.UTF8.GetBytes("mockhashedpassword"),
                PasswordSalt = Encoding.UTF8.GetBytes("mocksalt"),
                IsActive = true,
                PasswordSetDate = DateOnly.FromDateTime(DateTime.Now),
                PasswordNeverExpires = false
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "testpassword");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
        }

        [Fact]
        public void Login_IncorrectPassword_ReturnsFailure()
        {
            // Arrange
            var testUser = new User
            {
                Username = "testuser",
                Password = Encoding.UTF8.GetBytes("mockhashedpassword"),
                PasswordSalt = Encoding.UTF8.GetBytes("mocksalt"),
                IsActive = true
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "wrongpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Password is incorrect!", response.Message);
        }

        [Fact]
        public void Login_InactiveUser_ReturnsFailure()
        {
            // Arrange
            var testUser = new User
            {
                Username = "testuser",
                Password = Encoding.UTF8.GetBytes("mockhashedpassword"),
                PasswordSalt = Encoding.UTF8.GetBytes("mocksalt"),
                IsActive = false
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "testpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal($"The User 'testuser ({testUser.Id})' is suspended! Contact system administrator!", response.Message);
        }

        [Fact]
        public void Login_ExpiredPassword_ReturnsFailure()
        {
            // Arrange
            var testUser = new User
            {
                Username = "testuser",
                Password = Encoding.UTF8.GetBytes("mockhashedpassword"),
                PasswordSalt = Encoding.UTF8.GetBytes("mocksalt"),
                IsActive = true,
                PasswordSetDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-91)),
                PasswordNeverExpires = false
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "testpassword");

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
            Assert.Throws<Exception>(() => _userService.Login("testuser", "testpassword"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Login_UsernameNullOrEmpty_ReturnsFailure(string username)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.Login(username, "password");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Username cannot be null or empty!", response.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Login_PasswordNullOrEmpty_ReturnsFailure(string password)
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.Login("username", password);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Password cannot be null or empty!", response.Message);
        }

        [Fact]
        public void Login_ValidCredentialsPasswordNeverExpires_ReturnsSuccess()
        {
            // Arrange
            var testUser = new User
            {
                Username = "testuser",
                Password = Encoding.UTF8.GetBytes("mockhashedpassword"),
                PasswordSalt = Encoding.UTF8.GetBytes("mocksalt"),
                IsActive = true,
                PasswordSetDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-91)),
                PasswordNeverExpires = true
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "testpassword");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
        }

        [Fact]
        public void Login_ValidCredentialsAdminRole_ReturnsSuccess()
        {
            // Arrange
            var testUser = new User
            {
                Username = "adminuser",
                Password = Encoding.UTF8.GetBytes("mockhashedpassword"),
                PasswordSalt = Encoding.UTF8.GetBytes("mocksalt"),
                IsActive = true,
                Role = UserRole.Admin
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("adminuser", "adminpassword");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
            Assert.Equal(UserRole.Admin, response.Role);
        }

        [Fact]
        public void Login_ValidCredentialsRegularRole_ReturnsSuccess()
        {
            // Arrange
            var testUser = new User
            {
                Username = "regularuser",
                Password = Encoding.UTF8.GetBytes("mockhashedpassword"),
                PasswordSalt = Encoding.UTF8.GetBytes("mocksalt"),
                IsActive = true,
                Role = UserRole.Regular
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("regularuser", "regularpassword");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
            Assert.Equal(UserRole.Regular, response.Role);
        }

        [Fact]
        public void Login_CaseInsensitiveUsername_ReturnsSuccess()
        {
            // Arrange
            var testUser = new User
            {
                Username = "TestUser",
                Password = Encoding.UTF8.GetBytes("mockhashedpassword"),
                PasswordSalt = Encoding.UTF8.GetBytes("mocksalt"),
                IsActive = true,
                PasswordSetDate = DateOnly.FromDateTime(DateTime.Now),
                PasswordNeverExpires = false
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(testUser);

            // Act
            var response = _userService.Login("testuser", "testpassword");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User logged in!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_PasswordIsIncorrect_ReturnsFailure()
        {
            // Arrange
            var user = new User { /* Set properties here */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("testuser", "incorrectpassword", "newpassword", "newpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Old password is incorrect!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_NewPasswordDoesNotMeetRequirements_ReturnsFailure()
        {
            // Arrange
            var user = new User { /* Set properties here */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("existinguser", "oldpassword", "123", "123"); // assuming "123" doesn't meet password requirements

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("New password does not meet requirements!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_NewPasswordsDoNotMatch_ReturnsFailure()
        {
            // Arrange
            var user = new User { /* Set properties here like Username, Password etc. */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("existinguser", "oldpassword", "newpassword", "differentnewpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("New passwords do not match!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_OldPasswordCorrectButNewPasswordsDoNotMatch_ReturnsFailure()
        {
            // Arrange
            var user = new User { /* Set properties here */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("testuser", "correctoldpassword", "newpassword", "differentnewpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("New passwords do not match!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_OldPasswordCorrect_UpdatesPasswordSuccessfully()
        {
            // Arrange
            var user = new User { /* Set properties here */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>()));

            // Act
            var response = _userService.ChangeUserPassword("testuser", "correctpassword", "newpassword", "newpassword");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("Password updated!", response.Message);
            _mockRepo.Verify(repo => repo.UpdateUser(user), Times.Once);
        }

        [Fact]
        public void ChangeUserPassword_NewPasswordAndConfirmationDoNotMatch_ReturnsFailure()
        {
            // Arrange
            var user = new User { /* Set properties here */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("testuser", "testpassword", "newpassword", "differentpassword");

            // Assert
            // Replace "Passwords do not match!" with the appropriate error message from your service
            Assert.False(response.IsSuccess);
            Assert.Equal("Passwords do not match!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_OldPasswordIncorrect_ReturnsFailure()
        {
            // Arrange
            var user = new User { /* Set properties here like Username, Password etc. */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("existinguser", "incorrectoldpassword", "newpassword", "newpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Incorrect old password!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.ChangeUserPassword("nonexistentuser", "oldpassword", "newpassword", "newpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User does not exist!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_ExceptionThrown_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Throws<Exception>();

            // Act & Assert
            Assert.Throws<Exception>(() => _userService.ChangeUserPassword("testuser", "oldpassword", "newpassword", "newpassword"));
        }

        [Fact]
        public void ChangeUserPassword_NewPasswordSameAsOld_ReturnsFailure()
        {
            // Arrange
            var user = new User { /* Set properties here */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("testuser", "oldpassword", "oldpassword", "oldpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("New password cannot be the same as the old password!", response.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ChangeUserPassword_NewPasswordNullOrEmpty_ReturnsFailure(string newpassword)
        {
            // Arrange
            var user = new User { /* Set properties here */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("username", "oldpassword", newpassword, newpassword);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("New password cannot be null or empty!", response.Message);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ChangeUserPassword_OldPasswordNullOrEmpty_ReturnsFailure(string oldpassword)
        {
            // Arrange
            var user = new User { /* Set properties here */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("username", oldpassword, "newpassword", "newpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Old password cannot be null or empty!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_UserIsInactive_ReturnsFailure()
        {
            // Arrange
            var user = new User
            {
                IsActive = false
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("username", "oldpassword", "newpassword", "newpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Inactive users cannot change their password!", response.Message);
        }

        [Fact]
        public void ChangeUserPassword_UserIsActive_ReturnsFailure()
        {
            // Arrange
            var user = new User
            {
                IsActive = true
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserPassword("username", "oldpassword", "newpassword", "newpassword");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Locked users cannot change their password!", response.Message);
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

        [Fact]
        public void ChangeRole_UserExistsButNewRoleIsInvalid_ReturnsFailure()
        {
            // Arrange
            var user = new User { Role = UserRole.Regular };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            UserRole invalidRole = (UserRole)999; // Assuming 999 is not a valid UserRole
            var response = _userService.ChangeRole("existinguser", invalidRole);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Invalid role!", response.Message);
        }

        [Fact]
        public void ChangeRole_UserIsAdminAndOnlyAdminInTheSystem_ReturnsFailure()
        {
            // Arrange
            var user = new User { Role = UserRole.Admin };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Returns(1);

            // Act
            var response = _userService.ChangeRole("existingadmin", UserRole.Regular);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Cannot change the role as this is the only admin in the system!", response.Message);
        }

        [Fact]
        public void ChangeRole_UserExistsButThereCannotBeZeroAdmins_ReturnsFailure()
        {
            // Arrange
            var user = new User { Role = UserRole.Admin };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Returns(1);

            // Act
            var response = _userService.ChangeRole("existinguser", UserRole.Regular);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("There cannot be 0 admins in the system!", response.Message);
        }

        [Fact]
        public void ChangeRole_UserExistsAndNewRoleDifferentFromCurrent_ChangesRole()
        {
            // Arrange
            var user = new User { Username = "existinguser", Role = UserRole.Regular };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Returns(1);

            // Act
            var response = _userService.ChangeRole("existinguser", UserRole.Admin);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"Role for User 'existinguser' updated successfully!", response.Message);

            // Verify that the UpdateUser method was called
            _mockRepo.Verify(repo => repo.UpdateUser(user), Times.Once);
        }

        [Fact]
        public void ChangeRole_NewRoleIsSameAsOldRole_ReturnsFailure()
        {
            // Arrange
            var user = new User { Role = UserRole.Admin }; // assuming initial role is Admin
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeRole("testuser", UserRole.Admin); // attempting to change role to Admin

            // Assert
            // Replace "User already has that role!" with the appropriate error message from your service
            Assert.False(response.IsSuccess);
            Assert.Equal("User already has that role!", response.Message);
        }

        [Fact]
        public void ChangeRole_UserExistsAndNewRoleDifferent_ChangesRoleSuccessfully()
        {
            // Arrange
            var user = new User { Role = UserRole.Regular };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Returns(1);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>()));

            // Act
            var response = _userService.ChangeRole("existinguser", UserRole.Admin);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"Role for User 'existinguser' updated successfully!", response.Message);
            _mockRepo.Verify(repo => repo.UpdateUser(user), Times.Once);
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
            Assert.Equal("Username cannot be null or empty!", response.Message);
        }

        [Fact]
        public void ChangeRole_UserIsInactive_ReturnsFailure()
        {
            // Arrange
            var user = new User
            {
                IsActive = false
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeRole("username", UserRole.Admin);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Inactive users cannot change their role!", response.Message);
        }

        [Fact]
        public void ChangeRole_UserIsActive_ReturnsFailure()
        {
            // Arrange
            var user = new User
            {
                IsActive = true
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeRole("username", UserRole.Admin);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Locked users cannot change their role!", response.Message);
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

        [Fact]
        public void GetUserActiveStatus_UserExists_ReturnsSuccess()
        {
            // Arrange
            var user = new User { IsActive = true };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.GetUserActiveStatus("existinguser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User is active.", response.Message);
        }

        [Fact]
        public void GetUserActiveStatus_UserExists_ReturnsActiveStatus()
        {
            // Arrange
            var user = new User { Username = "existinguser", IsActive = true };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.GetUserActiveStatus("existinguser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User 'existinguser' activity status is 'True'!", response.Message);
        }

        [Fact]
        public void GetUserActiveStatus_UserExistsButIsInactive_ReturnsInactive()
        {
            // Arrange
            var user = new User { IsActive = false };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.GetUserActiveStatus("existinguser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User is inactive.", response.Message);
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
            Assert.Equal("Username cannot be null or empty!", response.Message);
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
        public void GetUserActiveStatus_UserIsActive_ReturnsUserLockedStatus()
        {
            // Arrange
            var user = new User
            {
                IsActive = true
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.GetUserActiveStatus("username");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User is locked.", response.Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.ChangeUserActiveStatus("nonexistentuser", "loggedinuser");

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
            var response = _userService.ChangeUserActiveStatus("sameuser", "sameuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Cannot deactivate your own account!", response.Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_UserIsAlreadyInactive_ReturnsFailure()
        {
            // Arrange
            var user = new User { IsActive = false };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserActiveStatus("existinguser", "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User is already inactive!", response.Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_UserIsAlreadyActive_ReturnsFailure()
        {
            // Arrange
            var user = new User { IsActive = true };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserActiveStatus("existinguser", "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User is already active!", response.Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_UserExistsAndIsNotLoggedInUser_UpdatesActiveStatusSuccessfully()
        {
            // Arrange
            var user = new User { IsActive = true };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>()));

            // Act
            var response = _userService.ChangeUserActiveStatus("existinguser", "loggedinuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User 'existinguser' activity status changed to 'False' successfully!", response.Message);
            _mockRepo.Verify(repo => repo.UpdateUser(user), Times.Once);
        }

        [Fact]
        public void ChangeUserActiveStatus_UserExistsAndNotLoggedInUser_ChangesActiveStatus()
        {
            // Arrange
            var user = new User { IsActive = true };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserActiveStatus("existinguser", "loggedinuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User 'existinguser' activity status changed to 'False' successfully!", response.Message);

            // Verify that the UpdateUser method was called
            _mockRepo.Verify(repo => repo.UpdateUser(user), Times.Once);
        }

        [Fact]
        public void ChangeUserActiveStatus_ExceptionThrown_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Throws(new Exception());

            // Act
            Assert.Throws<Exception>(() => _userService.ChangeUserActiveStatus("existinguser", "loggedinuser"));
        }

        [Fact]
        public void ChangeUserActiveStatus_UserDoesNotExist_UpdateUserNotCalled()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            _userService.ChangeUserActiveStatus("nonexistentuser", "loggedinuser");

            // Assert
            _mockRepo.Verify(repo => repo.UpdateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void ChangeUserActiveStatus_UserExistsAndIsInactive_UpdatesActiveStatusSuccessfully()
        {
            // Arrange
            var user = new User { IsActive = false };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>()));

            // Act
            var response = _userService.ChangeUserActiveStatus("existinguser", "loggedinuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User 'existinguser' activity status changed to 'True' successfully!", response.Message);
            _mockRepo.Verify(repo => repo.UpdateUser(user), Times.Once);
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
            Assert.Equal("Username cannot be null or empty!", response.Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_RepositoryUpdateFails_ReturnsFailure()
        {
            // Arrange
            var user = new User { IsActive = true };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>())).Throws(new Exception());

            // Act
            Assert.Throws<Exception>(() => _userService.ChangeUserActiveStatus("existinguser", "loggedinuser"));
        }

        [Fact]
        public void ChangeUserActiveStatus_UserAlreadyInactive_DeactivatesSuccessfully()
        {
            // Arrange
            var user = new User { IsActive = false };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>()));

            // Act
            var response = _userService.ChangeUserActiveStatus("existinguser", "loggedinuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User 'existinguser' activity status remains 'False'", response.Message);
            _mockRepo.Verify(repo => repo.UpdateUser(user), Times.Never);
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
            var response = _userService.ChangeUserActiveStatus(username, "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User does not exist!", response.Message);
        }

        [Fact]
        public void ChangeUserActiveStatus_UpdateUserThrowsException_UpdateUserNotCalled()
        {
            // Arrange
            var user = new User { IsActive = true };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.UpdateUser(It.IsAny<User>())).Throws(new Exception());

            // Act
            Assert.Throws<Exception>(() => _userService.ChangeUserActiveStatus("existinguser", "loggedinuser"));

            // Assert
            _mockRepo.Verify(repo => repo.UpdateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void ChangeUserActiveStatus_DeactivateAdminUser_ReturnsFailure()
        {
            // Arrange
            var user = new User { IsActive = true, Role = UserRole.Admin };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.ChangeUserActiveStatus("adminuser", "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Cannot deactivate an admin user!", response.Message);
        }

        [Fact]
        public void DeleteUser_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            var response = _userService.DeleteUser("nonexistentuser", "loggedinuser");

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
            var response = _userService.DeleteUser("sameuser", "sameuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Cannot delete your own account!", response.Message);
        }

        [Fact]
        public void DeleteUser_UserSuccessfullyDeleted_ReturnsSuccess()
        {
            // Arrange
            var user = new User { /* Set properties here */ };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.DeleteUser(It.IsAny<User>()));

            // Act
            var response = _userService.DeleteUser("testuser", "loggedinuser");

            // Assert
            // Replace "User 'testuser' deleted successfully!" with the appropriate success message from your service
            Assert.True(response.IsSuccess);
            Assert.Equal("User 'testuser' deleted successfully!", response.Message);
        }

        [Fact]
        public void DeleteUser_UserExistsAndNotLoggedInUser_DeletesUser()
        {
            // Arrange
            var user = new User { Username = "existinguser" };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.DeleteUser("existinguser", "loggedinuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal($"User 'existinguser' deleted successfully!", response.Message);

            // Verify that the DeleteUser method was called
            _mockRepo.Verify(repo => repo.DeleteUser(user), Times.Once);
        }

        [Fact]
        public void DeleteUser_UserExistsButIsLoggedInUser_ReturnsFailure()
        {
            // Arrange
            var user = new User { Username = "existinguser" };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.DeleteUser("existinguser", "existinguser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Cannot delete your own account!", response.Message);
        }

        [Fact]
        public void DeleteUser_UserExistsButIsLastAdmin_ReturnsFailure()
        {
            // Arrange
            var user = new User { Username = "existingadmin", Role = UserRole.Admin };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Returns(1);

            // Act
            var response = _userService.DeleteUser("existingadmin", "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Cannot delete user as it's the last admin in the system!", response.Message);
        }

        [Fact]
        public void DeleteUser_UserExistsAndIsNotLastAdmin_DeletesUserSuccessfully()
        {
            // Arrange
            var user = new User { Username = "existingadmin", Role = UserRole.Admin };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Returns(2); // there's another admin
            _mockRepo.Setup(repo => repo.DeleteUser(It.IsAny<User>()));

            // Act
            var response = _userService.DeleteUser("existingadmin", "loggedinuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User 'existingadmin' deleted successfully!", response.Message);
            _mockRepo.Verify(repo => repo.DeleteUser(user), Times.Once);
        }

        [Fact]
        public void DeleteUser_GetUserThrowsException_ReturnsFailure()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Throws(new Exception());

            // Act
            Assert.Throws<Exception>(() => _userService.DeleteUser("username", "loggedinuser"));
        }

        [Fact]
        public void DeleteUser_DeleteUserThrowsException_ReturnsFailure()
        {
            // Arrange
            var user = new User();
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.DeleteUser(user)).Throws(new Exception());

            // Act
            Assert.Throws<Exception>(() => _userService.DeleteUser("username", "loggedinuser"));
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
            Assert.Equal("Username cannot be null or empty!", response.Message);
        }

        [Fact]
        public void DeleteUser_UserDoesNotExist_DeleteUserNotCalled()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns<User>(null);

            // Act
            _userService.DeleteUser("nonexistentuser", "loggedinuser");

            // Assert
            _mockRepo.Verify(repo => repo.DeleteUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void DeleteUser_TryingToDeleteLastAdmin_DeleteUserNotCalled()
        {
            // Arrange
            var user = new User { Username = "existingadmin", Role = UserRole.Admin };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Returns(1);

            // Act
            _userService.DeleteUser("existingadmin", "loggedinuser");

            // Assert
            _mockRepo.Verify(repo => repo.DeleteUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void DeleteUser_GetRoleCountThrowsException_ReturnsFailure()
        {
            // Arrange
            var user = new User { Username = "existingadmin", Role = UserRole.Admin };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.GetRoleCount(UserRole.Admin)).Throws(new Exception());

            // Act
            Assert.Throws<Exception>(() => _userService.DeleteUser("existingadmin", "loggedinuser"));
        }

        [Fact]
        public void DeleteUser_UserExistsButIsInactive_DeletesUserSuccessfully()
        {
            // Arrange
            var user = new User { Username = "existinguser", IsActive = false };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.DeleteUser(It.IsAny<User>()));

            // Act
            var response = _userService.DeleteUser("existinguser", "loggedinuser");

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal("User 'existinguser' deleted successfully!", response.Message);
            _mockRepo.Verify(repo => repo.DeleteUser(user), Times.Once);
        }

        /*[Fact]
        public void DeleteUser_DeleteUserFails_ReturnsFailure()
        {
            // Arrange
            var user = new User();
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.DeleteUser(It.IsAny<User>())).Returns(false); // DeleteUser returns false indicating failure

            // Act
            var response = _userService.DeleteUser("username", "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User could not be deleted!", response.Message);
        }*/

        [Fact]
        public void DeleteUser_UserHasAssociatedEntities_ReturnsFailure()
        {
            // Arrange
            var user = new User
            {
                PersonInformation = new List<PersonInformation> { new PersonInformation() }
            };
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);

            // Act
            var response = _userService.DeleteUser("username", "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("User has associated information and cannot be deleted!", response.Message);
        }

        [Fact]
        public void DeleteUser_NonExistentLoggedInUser_ReturnsFailure()
        {
            // Arrange
            var user = new User();
            _mockRepo.Setup(repo => repo.GetUser("username")).Returns(user);
            _mockRepo.Setup(repo => repo.GetUser("nonexistentloggedinuser")).Returns<User>(null);

            // Act
            var response = _userService.DeleteUser("username", "nonexistentloggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Logged in user does not exist!", response.Message);
        }

        /*[Fact]
        public void DeleteUser_DeleteUserReturnsFalse_ReturnsFailure()
        {
            // Arrange
            var user = new User();
            _mockRepo.Setup(repo => repo.GetUser(It.IsAny<string>())).Returns(user);
            _mockRepo.Setup(repo => repo.DeleteUser(user)).Returns(false); // DeleteUser returns false

            // Act
            var response = _userService.DeleteUser("username", "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Failed to delete the user!", response.Message);
        }*/

        [Fact]
        public void DeleteUser_NonAdminTriesToDeleteUser_ReturnsFailure()
        {
            // Arrange
            var user = new User();
            var loggedInUser = new User { Role = UserRole.Regular }; // Non-admin user
            _mockRepo.Setup(repo => repo.GetUser("username")).Returns(user);
            _mockRepo.Setup(repo => repo.GetUser("loggedinuser")).Returns(loggedInUser);

            // Act
            var response = _userService.DeleteUser("username", "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("You do not have permission to delete users!", response.Message);
        }

        [Fact]
        public void DeleteUser_UserTriesToDeleteAdmin_ReturnsFailure()
        {
            // Arrange
            var user = new User { Role = UserRole.Admin };
            var loggedInUser = new User { Role = UserRole.Regular };
            _mockRepo.Setup(repo => repo.GetUser("username")).Returns(user);
            _mockRepo.Setup(repo => repo.GetUser("loggedinuser")).Returns(loggedInUser);

            // Act
            var response = _userService.DeleteUser("username", "loggedinuser");

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("You do not have permission to delete this user!", response.Message);
        }

        [Fact]
        public void DeleteUser_LoggedInUserDoesNotExist_DeleteUserNotCalled()
        {
            // Arrange
            var user = new User();
            _mockRepo.Setup(repo => repo.GetUser("username")).Returns(user);
            _mockRepo.Setup(repo => repo.GetUser("nonexistentloggedinuser")).Returns<User>(null);

            // Act
            _userService.DeleteUser("username", "nonexistentloggedinuser");

            // Assert
            _mockRepo.Verify(repo => repo.DeleteUser(It.IsAny<User>()), Times.Never);
        }
    }
}
