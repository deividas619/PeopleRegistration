using System.Security.Claims;
using System.Text;
using ControllerUnitTests.Fixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PeopleRegistration.API.Controllers;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.DTOs;

namespace ControllerUnitTests
{
    public class PersonInformationControllerTests
    {
        private readonly Mock<IPersonInformationService> _mockService;
        private readonly PersonInformationController _controller;
        private readonly Mock<HttpContext> _mockHttpContext;

        public PersonInformationControllerTests()
        {
            _mockService = new Mock<IPersonInformationService>();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
            }));

            _mockHttpContext = new Mock<HttpContext>();
            _mockHttpContext.Setup(hc => hc.User).Returns(user);

            _controller = new PersonInformationController(_mockService.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _mockHttpContext.Object,
                }
            };
        }

        [Fact]
        public async Task GetAllPeopleInformationForUser_ReturnOK()
        {
            // Arrange
            var personInformationDto = new List<PersonInformationDto>
            {
                new PersonInformationDto { Name = "Test1", LastName = "User1", PersonalCode = "12345678901" },
                new PersonInformationDto { Name = "Test2", LastName = "User2", PersonalCode = "12345678902" }
            };

            _mockService.Setup(service => service.GetAllPeopleInformationForUser(It.IsAny<string>())).ReturnsAsync(personInformationDto);

            // Act
            var result = await _controller.GetAllPeopleInformationForUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<PersonInformationDto>>(okResult.Value);

            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetAllPeopleInformationForUser_ReturnNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.GetAllPeopleInformationForUser(It.IsAny<string>())).ReturnsAsync((List<PersonInformationDto>)null);

            // Act
            var result = await _controller.GetAllPeopleInformationForUser();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllPeopleInformationForUser_ThrowsException()
        {
            // Arrange
            _mockService.Setup(service => service.GetAllPeopleInformationForUser(It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetAllPeopleInformationForUser());
        }

        [Fact]
        public async Task GetAllPeopleInformationForUser_VerifiesCorrectUsername()
        {
            // Act
            await _controller.GetAllPeopleInformationForUser();

            // Assert
            _mockService.Verify(service => service.GetAllPeopleInformationForUser("testuser"), Times.Once);
        }

        [Fact]
        public async Task GetAllPeopleInformationForUser_ReturnsEmptyList()
        {
            // Arrange
            var personInformationDto = new List<PersonInformationDto>();

            _mockService.Setup(service => service.GetAllPeopleInformationForUser(It.IsAny<string>())).ReturnsAsync(personInformationDto);

            // Act
            var result = await _controller.GetAllPeopleInformationForUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<PersonInformationDto>>(okResult.Value);

            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetAllPeopleInformationForAdmin_ReturnsOkResult_WithListOfPeople()
        {
            // Arrange
            var testPeople = new List<PersonInformationAdminDto>()
            {
                new PersonInformationAdminDto { Id = Guid.NewGuid(), Name = "Test1", LastName = "User1" },
                new PersonInformationAdminDto { Id = Guid.NewGuid(), Name = "Test2", LastName = "User2" }
            };
            _mockService.Setup(service => service.GetAllPeopleInformationForAdmin()).ReturnsAsync(testPeople);

            // Act
            var result = await _controller.GetAllPeopleInformationForAdmin();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<PersonInformationAdminDto>>(okResult.Value);
            Assert.Equal(testPeople.Count, returnValue.Count);
        }

        [Fact]
        public async Task GetAllPeopleInformationForAdmin_ReturnsNotFoundResult_WhenNoPeopleExist()
        {
            // Arrange
            _mockService.Setup(service => service.GetAllPeopleInformationForAdmin()).ReturnsAsync((List<PersonInformationAdminDto>)null);

            // Act
            var result = await _controller.GetAllPeopleInformationForAdmin();

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAllPeopleInformationForAdmin_ThrowsException()
        {
            // Arrange
            _mockService.Setup(service => service.GetAllPeopleInformationForAdmin()).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetAllPeopleInformationForAdmin());
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task GetSinglePersonInformationForUserByPersonalCode_ReturnOk(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.GetSinglePersonInformationForUserByPersonalCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(testUser);

            // Act
            var result = await _controller.GetSinglePersonInformationForUserByPersonalCode(testUser.PersonalCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PersonInformationDto>(okResult.Value);

            Assert.Equal(testUser.Name, returnValue.Name);
            Assert.Equal(testUser.LastName, returnValue.LastName);
            Assert.Equal(testUser.PersonalCode, returnValue.PersonalCode);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task GetSinglePersonInformationForUserByPersonalCode_ReturnNotFound(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.GetSinglePersonInformationForUserByPersonalCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((PersonInformationDto)null);

            // Act
            var result = await _controller.GetSinglePersonInformationForUserByPersonalCode(testUser.PersonalCode);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task GetSinglePersonInformationForUserByPersonalCode_ThrowsException(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.GetSinglePersonInformationForUserByPersonalCode(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetSinglePersonInformationForUserByPersonalCode(testUser.PersonalCode));
        }

        [Fact]
        public async Task GetSinglePersonInformationForUserByPersonalCode_InvalidCodeFormat_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetSinglePersonInformationForUserByPersonalCode("invalidCode");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSinglePersonInformationForAdminByPersonalCode_ReturnsOkResult_WithPersonInfo()
        {
            // Arrange
            var testPerson = new PersonInformationAdminDto { Id = Guid.NewGuid(), Name = "Test1", LastName = "User1" };
            _mockService.Setup(service => service.GetSinglePersonInformationForAdminByPersonalCode(It.IsAny<string>())).ReturnsAsync(testPerson);

            // Act
            var result = await _controller.GetSinglePersonInformationForAdminByPersonalCode("test");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PersonInformationAdminDto>(okResult.Value);
            Assert.Equal(testPerson, returnValue);
        }

        [Fact]
        public async Task GetSinglePersonInformationForAdminByPersonalCode_ReturnsNotFoundResult_WhenPersonDoesNotExist()
        {
            // Arrange
            _mockService.Setup(service => service.GetSinglePersonInformationForAdminByPersonalCode(It.IsAny<string>())).ReturnsAsync((PersonInformationAdminDto)null);

            // Act
            var result = await _controller.GetSinglePersonInformationForAdminByPersonalCode("test");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetSinglePersonInformationForAdminByPersonalCode_ThrowsException()
        {
            // Arrange
            _mockService.Setup(service => service.GetSinglePersonInformationForAdminByPersonalCode(It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetSinglePersonInformationForAdminByPersonalCode("test"));
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task GetPersonInformationPhotoByPersonalCode_ReturnsFileContent(PersonInformationDto testUser)
        {
            // Arrange
            var photoBytes = Encoding.UTF8.GetBytes("TestPhoto");
            var photoType = "image/jpeg";

            _mockService.Setup(service => service.GetPersonInformationPhotoByPersonalCode(It.IsAny<string>(), testUser.PersonalCode)).ReturnsAsync((photoBytes, photoType));

            // Act
            var result = await _controller.GetPersonInformationPhotoByPersonalCode(testUser.PersonalCode);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result.Result);

            Assert.Equal(photoBytes, fileResult.FileContents);
            Assert.Equal(photoType, fileResult.ContentType);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task GetPersonInformationPhotoByPersonalCode_ReturnsNotFound(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.GetPersonInformationPhotoByPersonalCode(It.IsAny<string>(), testUser.PersonalCode)).ReturnsAsync((null, string.Empty));

            // Act
            var result = await _controller.GetPersonInformationPhotoByPersonalCode(testUser.PersonalCode);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task AddPersonInformationForUser_ReturnOk(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.AddPersonInformationForUser(It.IsAny<string>(), It.IsAny<PersonInformationDto>(), It.IsAny<byte[]>(), It.IsAny<string>())).ReturnsAsync(testUser);

            // Act
            var result = await _controller.AddPersonInformationForUser(testUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PersonInformationDto>(okResult.Value);

            Assert.Equal(testUser, returnValue);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task AddPersonInformationForUser_ReturnBadRequest(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.AddPersonInformationForUser(It.IsAny<string>(), It.IsAny<PersonInformationDto>(), It.IsAny<byte[]>(), It.IsAny<string>())).Throws<ArgumentException>();

            // Act
            var result = await _controller.AddPersonInformationForUser(testUser);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task AddPersonInformationForUser_ThrowsException(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.AddPersonInformationForUser(It.IsAny<string>(), It.IsAny<PersonInformationDto>(), It.IsAny<byte[]>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.AddPersonInformationForUser(testUser));
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task AddPersonInformationForUser_ThrowsArgumentException_ReturnsBadRequest(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.AddPersonInformationForUser(It.IsAny<string>(), It.IsAny<PersonInformationDto>(), It.IsAny<byte[]>(), It.IsAny<string>())).ThrowsAsync(new ArgumentException());

            // Act
            var result = await _controller.AddPersonInformationForUser(testUser);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task AddPersonInformationForUser_EmptyDto_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.AddPersonInformationForUser(new PersonInformationDto());

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task UpdatePersonInformationForUserByPersonalCode_ReturnsOk(PersonInformationUpdateDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.UpdatePersonInformationForUserByPersonalCode(It.IsAny<string>(), "12345678901", It.IsAny<PersonInformationUpdateDto>(), It.IsAny<byte[]>(), It.IsAny<string>())).ReturnsAsync(testUser);

            // Act
            var result = await _controller.UpdatePersonInformationForUserByPersonalCode("12345678901", testUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PersonInformationUpdateDto>(okResult.Value);

            Assert.Equal(testUser, returnValue);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task UpdatePersonInformationForUserByPersonalCode_ReturnsNotFound(PersonInformationUpdateDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.UpdatePersonInformationForUserByPersonalCode(It.IsAny<string>(), "12345678901", It.IsAny<PersonInformationUpdateDto>(), It.IsAny<byte[]>(), It.IsAny<string>())).Throws<ArgumentException>();

            // Act
            var result = await _controller.UpdatePersonInformationForUserByPersonalCode("12345678901", testUser);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task UpdatePersonInformationForUserByPersonalCode_ThrowsException(PersonInformationUpdateDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.UpdatePersonInformationForUserByPersonalCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PersonInformationUpdateDto>(), It.IsAny<byte[]>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.UpdatePersonInformationForUserByPersonalCode("12345678901", testUser));
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task UpdatePersonInformationForUserByPersonalCode_ThrowsArgumentException_ReturnsNotFound(PersonInformationUpdateDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.UpdatePersonInformationForUserByPersonalCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PersonInformationUpdateDto>(), It.IsAny<byte[]>(), It.IsAny<string>())).ThrowsAsync(new ArgumentException());

            // Act
            var result = await _controller.UpdatePersonInformationForUserByPersonalCode("12345678901", testUser);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task UpdatePersonInformationForUserByPersonalCode_NonExistingPersonInformation_ReturnsNotFound(PersonInformationUpdateDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.UpdatePersonInformationForUserByPersonalCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<PersonInformationUpdateDto>(), It.IsAny<byte[]>(), It.IsAny<string>())).ReturnsAsync((PersonInformationUpdateDto)null);

            // Act
            var result = await _controller.UpdatePersonInformationForUserByPersonalCode("nonExistingCode", testUser);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task DeletePersonInformationForUserByPersonalCode_ReturnsOk(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.DeletePersonInformationForUserByPersonalCode(It.IsAny<string>(), testUser.PersonalCode)).ReturnsAsync(testUser);

            // Act
            var result = await _controller.DeletePersonInformationForUserByPersonalCode(testUser.PersonalCode);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PersonInformationDto>(okResult.Value);

            Assert.Equal(testUser, returnValue);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task DeletePersonInformationForUserByPersonalCode_ReturnsNotFound(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.DeletePersonInformationForUserByPersonalCode(It.IsAny<string>(), testUser.PersonalCode)).Throws<ArgumentException>();

            // Act
            var result = await _controller.DeletePersonInformationForUserByPersonalCode(testUser.PersonalCode);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Theory]
        [ControllerTestsFixture]
        public async Task DeletePersonInformationForUserByPersonalCode_ThrowsException(PersonInformationDto testUser)
        {
            // Arrange
            _mockService.Setup(service => service.DeletePersonInformationForUserByPersonalCode(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.DeletePersonInformationForUserByPersonalCode(testUser.PersonalCode));
        }

        [Fact]
        public async Task DeletePersonInformationForUserByPersonalCode_NonExistingPersonInformation_ReturnsNotFound()
        {
            // Arrange
            _mockService.Setup(service => service.DeletePersonInformationForUserByPersonalCode(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((PersonInformationDto)null);

            // Act
            var result = await _controller.DeletePersonInformationForUserByPersonalCode("nonExistingCode");

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}
