using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.BusinessLogic.Interfaces
{
    public interface IPersonInformationService
    {
        Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForUser(string username);
        Task<PersonInformation> GetSinglePersonInformationForUser(string username);
        Task<PersonInformation> AddPersonInformationForUser(string username, PersonInformationDto personInformation, byte[] imageBytes, string imageEncoding);
        Task<PersonInformation> UpdatePersonInformationForUser(string username);
        Task<PersonInformation> DeletePersonInformationForUser(string username);
    }
}
