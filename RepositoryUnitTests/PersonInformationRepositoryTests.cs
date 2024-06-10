using Microsoft.EntityFrameworkCore;
using Moq;
using PeopleRegistration.Database.Repositories;
using PeopleRegistration.Database;
using PeopleRegistration.Shared.Entities;
using RepositoryUnitTests.Fixture;

namespace RepositoryUnitTests
{
    public class PersonInformationRepositoryTests
    {
        private void SetupMockDbSet(Mock<DbSet<PersonInformation>> mockDbSet, IEnumerable<PersonInformation> entities)
        {
            mockDbSet.As<IQueryable<PersonInformation>>().Setup(m => m.Provider).Returns(entities.AsQueryable().Provider);
            mockDbSet.As<IQueryable<PersonInformation>>().Setup(m => m.Expression).Returns(entities.AsQueryable().Expression);
            mockDbSet.As<IQueryable<PersonInformation>>().Setup(m => m.ElementType).Returns(entities.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<PersonInformation>>().Setup(m => m.GetEnumerator()).Returns(entities.AsQueryable().GetEnumerator());

            mockDbSet.As<IAsyncEnumerable<PersonInformation>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(entities.ToAsyncEnumerable().GetAsyncEnumerator());
        }

        [Theory]
        [RepositoryTestsFixture]
        public async Task GetAllPeopleInformationForUser_ExistingUsername_ReturnsPeopleInformationList(User testUser)
        {
            // Arrange
            var personInformations = new List<PersonInformation>
            {
                new PersonInformation { User = new User { Username = testUser.Username } },
                new PersonInformation { User = new User { Username = testUser.Username } }
            };

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.GetAllPeopleInformationForUser(testUser.Username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Theory]
        [RepositoryTestsFixture]
        public async Task GetAllPeopleInformationForUser_NonExistingUsername_ReturnsEmptyList(User testUser)
        {
            // Arrange
            var username = "nonexistinguser";

            var personInformations = new List<PersonInformation>
            {
                new PersonInformation { User = new User { Username = testUser.Username } },
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.GetAllPeopleInformationForUser(username);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [RepositoryTestsFixture]
        public async Task GetSinglePersonInformationForUserByPersonalCode_ExistingUserAndCode_ReturnsPersonInformation(User testUser)
        {
            // Arrange
            var personalCode = "1234567890";
            var personInformation = new PersonInformation { User = new User { Username = testUser.Username }, PersonalCode = personalCode };

            var personInformations = new List<PersonInformation> { personInformation }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(testUser.Username, personalCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(personalCode, result.PersonalCode);
        }

        [Theory]
        [RepositoryTestsFixture]
        public async Task GetSinglePersonInformationForUserByPersonalCode_NonExistingUserAndCode_ReturnsNull(User testUser, PersonInformation testPersonInformation)
        {
            // Arrange
            var username = "nonexistinguser";

            var personInformations = new List<PersonInformation>
            {
                new PersonInformation { User = new User { Username = testUser.Username }, PersonalCode = "9876543210" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(username, testPersonInformation.PersonalCode);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddPersonInformationForUser_ValidRequest_ReturnsAddedPersonInformation()
        {
            // Arrange
            var personInformation = new PersonInformation();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            mockDbSet.Setup(d => d.Add(It.IsAny<PersonInformation>())).Callback<PersonInformation>((s) => personInformation = s);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.AddPersonInformationForUser(personInformation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(personInformation, result);
        }

        [Theory]
        [RepositoryTestsFixture]
        public async Task AddPersonInformationForUser_DuplicatePersonalCode_ThrowsException(PersonInformation testPersonInformation)
        {
            // Arrange
            var personInformations = new List<PersonInformation> { testPersonInformation }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateException());

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => personInformationRepository.AddPersonInformationForUser(testPersonInformation));
        }

        [Fact]
        public async Task AddPersonInformationForUser_NullRequest_ThrowsException()
        {
            // Arrange
            PersonInformation personInformation = null;

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => personInformationRepository.AddPersonInformationForUser(personInformation));
        }

        [Theory]
        [RepositoryTestsFixture]
        public async Task UpdatePersonInformationForUserByPersonalCode_ValidRequest_UpdatesAndReturnsPersonInformation(PersonInformation testPersonInformation)
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            mockDbSet.Setup(d => d.Update(It.IsAny<PersonInformation>())).Callback<PersonInformation>((s) => testPersonInformation = s);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.UpdatePersonInformationForUserByPersonalCode(testPersonInformation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testPersonInformation, result);
        }

        [Fact]
        public async Task UpdatePersonInformationForUserByPersonalCode_NullRequest_ThrowsException()
        {
            // Arrange
            PersonInformation personInformation = null;

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => personInformationRepository.UpdatePersonInformationForUserByPersonalCode(personInformation));
        }

        [Theory]
        [RepositoryTestsFixture]
        public async Task UpdatePersonInformationForUserByPersonalCode_NonExistingPersonInformation_ReturnsDefaultPersonInformation(PersonInformation testPersonInformation)
        {
            // Arrange
            var personInformations = new List<PersonInformation>().AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.UpdatePersonInformationForUserByPersonalCode(testPersonInformation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(default, result.Name);
            Assert.Equal(default, result.LastName);
            Assert.Equal(default, result.Gender);
            Assert.Equal(default, result.DateOfBirth);
            Assert.Null(result.PhoneNumber);
            Assert.Null(result.Email);
        }

        [Theory]
        [RepositoryTestsFixture]
        public async Task DeletePersonInformationForUserByPersonalCode_ExistingPersonInformation_RemovesAndReturnsPersonInformation(PersonInformation testPersonInformation)
        {
            // Arrange
            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            mockDbSet.Setup(d => d.Remove(It.IsAny<PersonInformation>())).Callback<PersonInformation>((s) => testPersonInformation = null);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.DeletePersonInformationForUserByPersonalCode(testPersonInformation);

            // Assert
            Assert.Null(testPersonInformation);
            Assert.NotNull(result);
        }

        [Theory]
        [RepositoryTestsFixture]
        public async Task DeletePersonInformationForUserByPersonalCode_NonExistingPersonInformation_CompletesWithoutException(PersonInformation testPersonInformation)
        {
            // Arrange
            var personInformations = new List<PersonInformation>().AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var ex = await Record.ExceptionAsync(() => personInformationRepository.DeletePersonInformationForUserByPersonalCode(testPersonInformation));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public async Task DeletePersonInformationForUserByPersonalCode_NullRequest_ThrowsException()
        {
            // Arrange
            PersonInformation personInformation = null;

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => personInformationRepository.DeletePersonInformationForUserByPersonalCode(personInformation));
        }

        [Fact]
        public async Task DeleteResidencePlaceForUser_ValidRequest_RemovesAndReturnsResidencePlace()
        {
            // Arrange
            var residencePlace = new ResidencePlace { Id = Guid.NewGuid() };

            var mockDbSet = new Mock<DbSet<ResidencePlace>>();
            mockDbSet.Setup(d => d.Remove(It.IsAny<ResidencePlace>())).Callback<ResidencePlace>((s) => residencePlace = null);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.ResidencePlaces).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.DeleteResidencePlaceForUser(residencePlace);

            // Assert
            Assert.Null(residencePlace);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteResidencePlaceForUser_NullRequest_ThrowsException()
        {
            // Arrange
            ResidencePlace residencePlace = null;

            var mockDbSet = new Mock<DbSet<ResidencePlace>>();
            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.ResidencePlaces).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => personInformationRepository.DeleteResidencePlaceForUser(residencePlace));
        }

        [Fact]
        public async Task DeleteResidencePlaceForUser_NonExistingResidencePlace_CompletesWithoutException()
        {
            // Arrange
            var residencePlace = new ResidencePlace { Id = Guid.NewGuid() };

            var residencePlaces = new List<ResidencePlace>().AsQueryable();

            var mockDbSet = new Mock<DbSet<ResidencePlace>>();

            mockDbSet.As<IQueryable<ResidencePlace>>().Setup(m => m.Provider).Returns(residencePlaces.AsQueryable().Provider);
            mockDbSet.As<IQueryable<ResidencePlace>>().Setup(m => m.Expression).Returns(residencePlaces.AsQueryable().Expression);
            mockDbSet.As<IQueryable<ResidencePlace>>().Setup(m => m.ElementType).Returns(residencePlaces.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<ResidencePlace>>().Setup(m => m.GetEnumerator()).Returns(residencePlaces.AsQueryable().GetEnumerator());

            mockDbSet.As<IAsyncEnumerable<ResidencePlace>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(residencePlaces.ToAsyncEnumerable().GetAsyncEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.ResidencePlaces).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var ex = await Record.ExceptionAsync(() => personInformationRepository.DeleteResidencePlaceForUser(residencePlace));

            // Assert
            Assert.Null(ex);
        }
    }
}
