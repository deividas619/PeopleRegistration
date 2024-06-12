using Moq;
using PeopleRegistration.BusinessLogic.Services;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;

namespace BusinessLogicUnitTests
{
    public class PersonInformationServiceTests
    {
        private readonly Mock<IPersonInformationRepository> _personInformationRepositoryMock;
        private readonly Mock<IUserRepository> _userRepoMock;

        public PersonInformationServiceTests()
        {
            _personInformationRepositoryMock = new Mock<IPersonInformationRepository>();
            _userRepoMock = new Mock<IUserRepository>();
        }

        [Fact]
        public async Task GetAllPeopleInformationForUser_ReturnsCorrectData()
        {
            // Arrange
            var personInformation = new List<PersonInformation>
            {
                new PersonInformation { Name = "Test1", LastName = "User1", PersonalCode = "12345678901" },
                new PersonInformation { Name = "Test2", LastName = "User2", PersonalCode = "12345678902" }
            };
            
            _personInformationRepositoryMock.Setup(x => x.GetAllPeopleInformationForUser("testUser")).ReturnsAsync(personInformation);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.GetAllPeopleInformationForUser("testUser");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllPeopleInformationForUser_ThrowsException_WhenErrorOccursInRepository()
        {
            // Arrange
            _personInformationRepositoryMock.Setup(x => x.GetAllPeopleInformationForUser("testUser")).ThrowsAsync(new Exception());
            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetAllPeopleInformationForUser("testUser"));
        }

        [Fact]
        public async Task GetAllPeopleInformationForUser_ReturnsNull_WhenUsernameIsEmpty()
        {
            // Arrange
            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.GetAllPeopleInformationForUser("");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllPeopleInformationForAdmin_ReturnsListOfPeople()
        {
            // Arrange
            var personInformation = new List<PersonInformation>()
            {
                new PersonInformation { Id = Guid.NewGuid(), Name = "Test1", LastName = "User1" },
                new PersonInformation { Id = Guid.NewGuid(), Name = "Test2", LastName = "User2" }
            };

            _personInformationRepositoryMock.Setup(x => x.GetAllPeopleInformationForAdmin()).ReturnsAsync(personInformation);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.GetAllPeopleInformationForAdmin();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetSinglePersonInformationForUserByPersonalCode_ReturnsCorrectData()
        {
            // Arrange
            var testUser = new PersonInformation { Name = "Test1", LastName = "User1", PersonalCode = "12345678902" };
            
            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode("Test1", "12345678902")).ReturnsAsync(testUser);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.GetSinglePersonInformationForUserByPersonalCode("Test1", "12345678902");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test1", result.Name);
        }

        [Fact]
        public async Task GetSinglePersonInformationForUserByPersonalCode_ThrowsException_WhenErrorOccursInRepository()
        {
            // Arrange
            string username = "testUser";
            string personalCode = "12345";
            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personalCode)).ThrowsAsync(new Exception());
            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetSinglePersonInformationForUserByPersonalCode(username, personalCode));
        }

        [Fact]
        public async Task GetSinglePersonInformationForUserByPersonalCode_ReturnsNull_WhenUsernameOrPersonalCodeIsEmpty()
        {
            // Arrange
            string username = string.Empty;
            string personalCode = "12345";
            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

            // Assert
            Assert.Null(result);

            username = "testUser";
            personalCode = string.Empty;

            // Act
            result = await service.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSinglePersonInformationForAdminByPersonalCode_ReturnsPersonInfo()
        {
            // Arrange
            var testUser = new PersonInformation { Name = "Test1", LastName = "User1", PersonalCode = "12345678902" };

            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForAdminByPersonalCode("12345678902")).ReturnsAsync(testUser);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.GetSinglePersonInformationForAdminByPersonalCode("12345678902");

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPersonInformationPhotoByPersonalCode_ReturnsCorrectData()
        {
            // Arrange
            string username = "testUser";
            string personalCode = "12345";
            var personInformation = new PersonInformation
            {
                Name = "John",
                LastName = "Doe",
                PersonalCode = "12345",
                ProfilePhotoEncoding = "image/jpeg"
            };

            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personalCode)).ReturnsAsync(personInformation);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.GetPersonInformationPhotoByPersonalCode(username, personalCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("image/jpeg", result.Item2);
        }

        [Fact]
        public async Task GetPersonInformationPhotoByPersonalCode_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            string username = "testUser";
            string personalCode = "12345";
            var personInformation = new PersonInformation { Name = "John", LastName = "Doe", PersonalCode = "12345" };
            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personalCode)).ReturnsAsync((PersonInformation)null);
            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.GetPersonInformationPhotoByPersonalCode(username, personalCode);

            // Assert
            Assert.Null(result.Item1);
            Assert.Null(result.Item2);

            _personInformationRepositoryMock.Verify(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personalCode), Times.Once);
        }

        [Fact]
        public async Task GetPersonInformationPhotoByPersonalCode_ThrowsException_WhenErrorOccursInRepository()
        {
            // Arrange
            string username = "testUser";
            string personalCode = "12345";
            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personalCode)).ThrowsAsync(new Exception());
            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetPersonInformationPhotoByPersonalCode(username, personalCode));
        }

        [Fact]
        public async Task AddPersonInformationForUser_ThrowsException_WhenPersonAlreadyExists()
        {
            // Arrange
            string username = "testUser";
            var personInformation = new PersonInformation { Name = "John", LastName = "Doe", PersonalCode = "12345" };

            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personInformation.PersonalCode)).ReturnsAsync(personInformation);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act & Assert
            var personInformationDto = new PersonInformationDto { Name = "John", LastName = "Doe", PersonalCode = "12345" };
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddPersonInformationForUser(username, personInformationDto, new byte[0], "imageEncodingPlaceholder"));
        }

        [Fact]
        public async Task AddPersonInformationForUser_ReturnsAddedData_WhenDataIsValid()
        {
            // Arrange
            string username = "testUser";
            var personInformation = new PersonInformation { Name = "John", LastName = "Doe", PersonalCode = "12345" };
            var personInformationDto = new PersonInformationDto { Name = "John", LastName = "Doe", PersonalCode = "12345" };

            _personInformationRepositoryMock.Setup(x => x.AddPersonInformationForUser(It.IsAny<PersonInformation>())).ReturnsAsync(personInformation);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.AddPersonInformationForUser(username, personInformationDto, new byte[0], "imageEncodingPlaceholder");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(personInformationDto.Name, result.Name);
            Assert.Equal(personInformationDto.LastName, result.LastName);
        }

        [Fact]
        public async Task AddPersonInformationForUser_ThrowsArgumentException_WhenUsernameIsEmptyOrPersonInformationIsNull()
        {
            // Arrange
            string username = string.Empty;
            var personInformation = new PersonInformationDto { Name = "John", LastName = "Doe", PersonalCode = "12345" };
            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.AddPersonInformationForUser(username, personInformation, new byte[0], "imageEncodingPlaceholder"));

            username = "testUser";
            personInformation = null;

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.AddPersonInformationForUser(username, personInformation, new byte[0], "imageEncodingPlaceholder"));
        }

        [Fact]
        public async Task UpdatePersonInformationForUserByPersonalCode_UpdatesDataCorrectly()
        {
            // Arrange
            string username = "testUser";
            string personalCode = "12345";
            var personInformation = new PersonInformation { Name = "John Updated", LastName = "Doe Updated", PersonalCode = personalCode };
            var personInformationUpdateDto = new PersonInformationUpdateDto { Name = "John Updated", LastName = "Doe Updated" };

            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personalCode)).ReturnsAsync(personInformation);
            _personInformationRepositoryMock.Setup(x => x.UpdatePersonInformationForUserByPersonalCode(It.IsAny<PersonInformation>())).ReturnsAsync(personInformation);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.UpdatePersonInformationForUserByPersonalCode(username, personalCode, personInformationUpdateDto, new byte[0], "imageEncodingPlaceholder");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John Updated", result.Name);
            Assert.Equal("Doe Updated", result.LastName);
        }

        [Fact]
        public async Task UpdatePersonInformationForUserByPersonalCode_ThrowsException_WhenPersonNotFound()
        {
            // Arrange
            string username = "testUser";
            string personalCode = "12345";
            var personInformationUpdate = new PersonInformationUpdateDto { Name = "John Updated", LastName = "Doe Updated" };

            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personalCode)).ReturnsAsync((PersonInformation)null);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdatePersonInformationForUserByPersonalCode(username, personalCode, personInformationUpdate, new byte[0], "imageEncodingPlaceholder"));
        }

        [Fact]
        public async Task UpdatePersonInformationForUserByPersonalCode_ThrowsException_WhenErrorOccursInRepository()
        {
            // Arrange
            string username = "testUser";
            string personalCode = "12345";
            var personInformationUpdate = new PersonInformationUpdateDto { Name = "John Updated", LastName = "Doe Updated" };
            _personInformationRepositoryMock.Setup(x => x.UpdatePersonInformationForUserByPersonalCode(It.IsAny<PersonInformation>())).ThrowsAsync(new Exception());
            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdatePersonInformationForUserByPersonalCode(username, personalCode, personInformationUpdate, new byte[0], "imageEncodingPlaceholder"));
        }

        [Fact]
        public async Task UpdatePersonInformationForUserByPersonalCode_ThrowsArgumentException_WhenUsernameOrPersonalCodeIsEmptyOrPersonInformationUpdateIsNull()
        {
            // Arrange
            string username = string.Empty;
            string personalCode = "12345";
            var personInformationUpdate = new PersonInformationUpdateDto { Name = "John Updated", LastName = "Doe Updated" };
            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdatePersonInformationForUserByPersonalCode(username, personalCode, personInformationUpdate, new byte[0], "imageEncodingPlaceholder"));

            username = "testUser";
            personalCode = string.Empty;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdatePersonInformationForUserByPersonalCode(username, personalCode, personInformationUpdate, new byte[0], "imageEncodingPlaceholder"));

            personalCode = "12345";
            personInformationUpdate = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdatePersonInformationForUserByPersonalCode(username, personalCode, personInformationUpdate, new byte[0], "imageEncodingPlaceholder"));
        }

        [Fact]
        public async Task DeletePersonInformationForUserByPersonalCode_DeletesDataCorrectly()
        {
            // Arrange
            string username = "testUser";
            string personalCode = "12345";
            var personInformation = new PersonInformation { Name = "John", LastName = "Doe", PersonalCode = "12345" };

            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personalCode)).ReturnsAsync(personInformation);
            _personInformationRepositoryMock.Setup(x => x.DeletePersonInformationForUserByPersonalCode(It.IsAny<PersonInformation>())).ReturnsAsync(personInformation);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act
            var result = await service.DeletePersonInformationForUserByPersonalCode(username, personalCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(personInformation.Name, result.Name);
            Assert.Equal(personInformation.LastName, result.LastName);
        }

        [Fact]
        public async Task DeletePersonInformationForUserByPersonalCode_ThrowsException_WhenPersonNotFound()
        {
            // Arrange
            string username = "testUser";
            string personalCode = "12345";

            _personInformationRepositoryMock.Setup(x => x.GetSinglePersonInformationForUserByPersonalCode(username, personalCode)).ReturnsAsync((PersonInformation)null);

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.DeletePersonInformationForUserByPersonalCode(username, personalCode));
        }

        [Fact]
        public async Task ResizePhoto_ThrowsException_WhenImageEncodingIsUnsupported()
        {
            // Arrange
            string username = "testUser";
            var personInformation = new PersonInformationDto { Name = "John", LastName = "Doe", PersonalCode = "12345" };
            var unsupportedImageEncoding = "unsupportedEncoding";

            var service = new PersonInformationService(_personInformationRepositoryMock.Object, _userRepoMock.Object);
            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.AddPersonInformationForUser(username, personInformation, new byte[0], unsupportedImageEncoding));
        }
    }
}
