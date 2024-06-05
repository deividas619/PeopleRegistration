using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.Database.Interfaces
{
    public interface IPersonInformationRepository
    {
        Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForUser(string username);
        Task<PersonInformation> GetSinglePersonInformationForUserByPersonalCode(string username, string personalCode);
        Task<PersonInformation> AddPersonInformationForUser(PersonInformation personInformation);
        Task<PersonInformation> UpdatePersonInformationForUserByPersonalCode(PersonInformation request);
        Task<PersonInformation> DeletePersonInformationForUserByPersonalCode(string username, string personalCode);
    }
}
