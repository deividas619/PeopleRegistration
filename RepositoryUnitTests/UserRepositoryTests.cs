using Microsoft.EntityFrameworkCore;
using Moq;
using PeopleRegistration.Database.Repositories;
using PeopleRegistration.Database;
using PeopleRegistration.Shared.Entities;
using PeopleRegistration.Shared.Enums;
using RepositoryUnitTests.Fixture;

namespace RepositoryUnitTests
{
    public class UserRepositoryTests
    {
        [Theory]
        [RepositoryTestsFixture]
        public void GetUser_ExistingUsername_ReturnsUser(User testUser)
        {
            // Arrange
            var users = new List<User>
            {
                testUser
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = userRepository.GetUser(testUser.Username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testUser.Username, result.Username);
        }

        [Fact]
        public void GetUser_NonExistingUsername_ReturnsNull()
        {
            // Arrange
            var users = new List<User>().AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = userRepository.GetUser("nonexistinguser");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUser_EmptyUsername_ThrowsArgumentException()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act & Assert
            Assert.Throws<NotSupportedException>(() => userRepository.GetUser(""));
        }

        [Fact]
        public void SaveUser_NewUser_SavesUser()
        {
            // Arrange
            var newUser = new User { Id = Guid.NewGuid(), Username = "newuser", Password = new byte[0], PasswordSalt = new byte[0] };

            var mockDbSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            userRepository.SaveNewUser(newUser);

            // Assert
            mockDbSet.Verify(dbSet => dbSet.Add(newUser), Times.Once);
            mockContext.Verify(context => context.SaveChanges(), Times.Once);
        }

        [Theory]
        [RepositoryTestsFixture]
        public void SaveNewUser_ExistingUser_ThrowsException(User testUser)
        {
            // Arrange
            var users = new List<User> { testUser }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Throws(new Exception("An error occurred while saving the entity changes"));

            var userRepository = new UserRepository(mockContext.Object);
            var newUser = new User { Id = testUser.Id, Username = testUser.Username, Password = testUser.Password, PasswordSalt = testUser.PasswordSalt };

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => userRepository.SaveNewUser(newUser));
            Assert.Contains("An error occurred while saving the entity changes", exception.Message);
        }

        [Fact]
        public void SaveNewUser_NullUser_ThrowsArgumentNullException()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => userRepository.SaveNewUser(null));
        }

        [Theory]
        [RepositoryTestsFixture]
        public void UpdateUser_ExistingUser_UpdatesUser(User testUser)
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            userRepository.UpdateUser(testUser);

            // Assert
            mockContext.Verify(context => context.Update(testUser), Times.Once);
            mockContext.Verify(context => context.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpdateUser_NullUser_ThrowsArgumentNullException()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();

            var userRepository = new UserRepository(mockContext.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => userRepository.UpdateUser(null));
        }

        [Fact]
        public void UpdateUser_NonExistingUser_ThrowsException()
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<User>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => userRepository.UpdateUser(null));
        }

        [Fact]
        public void DeleteUser_ExistingUser_DeletesUserAndRelatedData()
        {
            // Arrange
            var userToDelete = new User { Id = Guid.NewGuid(), Username = "userToDelete", Password = new byte[0], PasswordSalt = new byte[0] };

            var personInformationList = new List<PersonInformation>
            {
                new PersonInformation { Id = Guid.NewGuid(), User = userToDelete, ResidencePlace = new ResidencePlace { Id = Guid.NewGuid() } },
                new PersonInformation { Id = Guid.NewGuid(), User = userToDelete, ResidencePlace = null }
            }.AsQueryable();

            var mockPersonInformationDbSet = new Mock<DbSet<PersonInformation>>();
            mockPersonInformationDbSet.As<IQueryable<PersonInformation>>().Setup(m => m.Provider).Returns(personInformationList.Provider);
            mockPersonInformationDbSet.As<IQueryable<PersonInformation>>().Setup(m => m.Expression).Returns(personInformationList.Expression);
            mockPersonInformationDbSet.As<IQueryable<PersonInformation>>().Setup(m => m.ElementType).Returns(personInformationList.ElementType);
            mockPersonInformationDbSet.As<IQueryable<PersonInformation>>().Setup(m => m.GetEnumerator()).Returns(personInformationList.GetEnumerator());

            var mockResidencePlaceDbSet = new Mock<DbSet<ResidencePlace>>();
            mockResidencePlaceDbSet.Setup(m => m.Remove(It.IsAny<ResidencePlace>()));

            var mockUserDbSet = new Mock<DbSet<User>>();
            mockUserDbSet.Setup(m => m.Remove(It.IsAny<User>()));

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockUserDbSet.Object);
            mockContext.Setup(c => c.PeopleInformation).Returns(mockPersonInformationDbSet.Object);
            mockContext.Setup(c => c.ResidencePlaces).Returns(mockResidencePlaceDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            userRepository.DeleteUser(userToDelete);

            // Assert
            mockPersonInformationDbSet.Verify(dbSet => dbSet.Remove(It.IsAny<PersonInformation>()), Times.Exactly(2));
            mockResidencePlaceDbSet.Verify(dbSet => dbSet.Remove(It.IsAny<ResidencePlace>()), Times.Once);
            mockUserDbSet.Verify(dbSet => dbSet.Remove(userToDelete), Times.Once);
            mockContext.Verify(context => context.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteUser_NullUser_ThrowsArgumentNullException()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();

            var userRepository = new UserRepository(mockContext.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => userRepository.DeleteUser(null));
        }

        [Fact]
        public void DeleteUser_NonExistingUser_ThrowsException()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>();

            var userRepository = new UserRepository(mockContext.Object);

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => userRepository.DeleteUser(null));
        }

        [Fact]
        public void GetRoleCount_ReturnsCorrectCount()
        {
            // Arrange;
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Username = "user1", Role = UserRole.Admin },
                new User { Id = Guid.NewGuid(), Username = "user2", Role = UserRole.Admin },
                new User { Id = Guid.NewGuid(), Username = "user3", Role = UserRole.Regular }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = userRepository.GetRoleCount(UserRole.Admin);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public void GetRoleCount_NoUsersWithRole_ReturnsZero()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Username = "user1", Role = UserRole.Regular },
                new User { Id = Guid.NewGuid(), Username = "user2", Role = UserRole.Regular }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = userRepository.GetRoleCount(UserRole.Admin);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void GetRoleCount_NoUsers_ReturnsZero()
        {
            // Arrange
            var role = UserRole.Admin;
            var users = new List<User>().AsQueryable();

            var mockDbSet = new Mock<DbSet<User>>();
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockDbSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.Users).Returns(mockDbSet.Object);

            var userRepository = new UserRepository(mockContext.Object);

            // Act
            var result = userRepository.GetRoleCount(role);

            // Assert
            Assert.Equal(0, result);
        }
    }
}
