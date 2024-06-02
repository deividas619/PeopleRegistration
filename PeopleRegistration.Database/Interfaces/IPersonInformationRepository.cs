using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.Database.Interfaces
{
    public interface IPersonInformationRepository
    {
        Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForUser(string username);
        Task<PersonInformation> GetSinglePersonInformationForUser(string username);
        Task<PersonInformation> AddPersonInformationForUser(string username);
        Task<PersonInformation> UpdatePersonInformationForUser(string username);
        Task<PersonInformation> DeletePersonInformationForUser(string username);
    }
}
