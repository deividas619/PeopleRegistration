using PeopleRegistration.Shared.DTOs;

namespace PeopleRegistration.BusinessLogic.Interfaces
{
    public interface IPersonInformationService
    {
        Task<IEnumerable<PersonInformationDto>> GetAllPeopleInformationForUser(string username);
        Task<PersonInformationDto> GetSinglePersonInformationForUserByPersonalCode(string username, string personalCode);
        Task<(byte[], string)> GetPersonInformationPhotoByPersonalCode(string username, string personalCode);
        Task<PersonInformationDto> AddPersonInformationForUser(string username, PersonInformationDto personInformation, byte[] imageBytes, string imageEncoding);
        Task<PersonInformationDto> UpdatePersonInformationForUserByPersonalCode(string username, string personalCode, PersonInformationDto request, byte[] imageBytes, string imageEncoding);
        Task<PersonInformationDto> DeletePersonInformationForUserByPersonalCode(string username, string personalCode);
    }
}
