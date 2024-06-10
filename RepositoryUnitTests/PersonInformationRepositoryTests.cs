using Microsoft.EntityFrameworkCore;
using Moq;
using PeopleRegistration.Database.Repositories;
using PeopleRegistration.Database;
using PeopleRegistration.Shared.Entities;

namespace RepositoryUnitTests
{
    public class PersonInformationRepositoryTests
    {
        [Fact]
        public async Task GetAllPeopleInformationForUser_ExistingUsername_ReturnsPeopleInformationList()
        {
            // Arrange
            var username = "existinguser";
            var personInformations = new List<PersonInformation>
            {
                new PersonInformation { User = new User { Username = username } },
                new PersonInformation { User = new User { Username = username } }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.GetAllPeopleInformationForUser(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllPeopleInformationForUser_NonExistingUsername_ReturnsEmptyList()
        {
            // Arrange
            var username = "nonexistinguser";

            var personInformations = new List<PersonInformation>
            {
                new PersonInformation { User = new User { Username = "existinguser" } },
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

        [Fact]
        public async Task GetSinglePersonInformationForUserByPersonalCode_ExistingUserAndCode_ReturnsPersonInformation()
        {
            // Arrange
            var username = "existinguser";
            var personalCode = "1234567890";
            var personInformation = new PersonInformation { User = new User { Username = username }, PersonalCode = personalCode };

            var personInformations = new List<PersonInformation> { personInformation }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(personalCode, result.PersonalCode);
        }

        [Fact]
        public async Task GetSinglePersonInformationForUserByPersonalCode_NonExistingUserAndCode_ReturnsNull()
        {
            // Arrange
            var username = "nonexistinguser";
            var personalCode = "1234567890";

            var personInformations = new List<PersonInformation>
            {
                new PersonInformation { User = new User { Username = "existinguser" }, PersonalCode = "9876543210" }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

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

        [Fact]
        public async Task AddPersonInformationForUser_DuplicatePersonalCode_ThrowsException()
        {
            // Arrange
            var personInformation = new PersonInformation { PersonalCode = "1234567890" };

            var personInformations = new List<PersonInformation> { personInformation }.AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);
            mockContext.Setup(c => c.SaveChangesAsync(default)).Throws(new DbUpdateException());

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => personInformationRepository.AddPersonInformationForUser(personInformation));
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

        [Fact]
        public async Task UpdatePersonInformationForUserByPersonalCode_ValidRequest_UpdatesAndReturnsPersonInformation()
        {
            // Arrange
            var personInformation = new PersonInformation { PersonalCode = "1234567890" };

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            mockDbSet.Setup(d => d.Update(It.IsAny<PersonInformation>())).Callback<PersonInformation>((s) => personInformation = s);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.UpdatePersonInformationForUserByPersonalCode(personInformation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(personInformation, result);
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

        [Fact]
        public async Task UpdatePersonInformationForUserByPersonalCode_NonExistingPersonInformation_ReturnsDefaultPersonInformation()
        {
            // Arrange
            var personInformation = new PersonInformation { PersonalCode = "1234567890" };

            var personInformations = new List<PersonInformation>().AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.UpdatePersonInformationForUserByPersonalCode(personInformation);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(default, result.Name);
            Assert.Equal(default, result.LastName);
            Assert.Equal(default, result.Gender);
            Assert.Equal(default, result.DateOfBirth);
            Assert.Null(result.PhoneNumber);
            Assert.Null(result.Email);
        }

        [Fact]
        public async Task DeletePersonInformationForUserByPersonalCode_ExistingPersonInformation_RemovesAndReturnsPersonInformation()
        {
            // Arrange
            var personInformation = new PersonInformation { PersonalCode = "1234567890" };

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            mockDbSet.Setup(d => d.Remove(It.IsAny<PersonInformation>())).Callback<PersonInformation>((s) => personInformation = null);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var result = await personInformationRepository.DeletePersonInformationForUserByPersonalCode(personInformation);

            // Assert
            Assert.Null(personInformation);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeletePersonInformationForUserByPersonalCode_NonExistingPersonInformation_CompletesWithoutException()
        {
            // Arrange
            var personInformation = new PersonInformation { PersonalCode = "1234567890" };

            var personInformations = new List<PersonInformation>().AsQueryable();

            var mockDbSet = new Mock<DbSet<PersonInformation>>();
            SetupMockDbSet(mockDbSet, personInformations);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.PeopleInformation).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var ex = await Record.ExceptionAsync(() => personInformationRepository.DeletePersonInformationForUserByPersonalCode(personInformation));

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
            SetupMockDbSet(mockDbSet, residencePlaces);

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.ResidencePlaces).Returns(mockDbSet.Object);

            var personInformationRepository = new PersonInformationRepository(mockContext.Object);

            // Act
            var ex = await Record.ExceptionAsync(() => personInformationRepository.DeleteResidencePlaceForUser(residencePlace));

            // Assert
            Assert.Null(ex);
        }

        private void SetupMockDbSet<T>(Mock<DbSet<T>> mockDbSet, IQueryable<T> entities) where T : class
        {
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entities.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entities.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entities.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entities.GetEnumerator());
        }
    }
}
