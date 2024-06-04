using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.BusinessLogic.Interfaces
{
    public interface IPersonInformationService
    {
        Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForUser(string username);
        Task<PersonInformation> GetSinglePersonInformationForUserByPersonalCode(string username, string personalCode);
        Task<PersonInformation> GetSinglePersonInformationForUserByObjectId(string username, Guid id);
        Task<PersonInformation> AddPersonInformationForUser(string username, PersonInformationDto personInformation, byte[] imageBytes, string imageEncoding);
        Task<PersonInformation> UpdatePersonInformationForUserByPersonalCode(string username, string personalCode, PersonInformationDto request);
        Task<PersonInformation> UpdatePersonInformationForUserByObjectId(string username, string personalCode, Guid id);
        Task<PersonInformation> DeletePersonInformationForUserByPersonalCode(string username, string personalCode);
        Task<PersonInformation> DeletePersonInformationForUserByObjectId(string username, Guid id);
    }
}
