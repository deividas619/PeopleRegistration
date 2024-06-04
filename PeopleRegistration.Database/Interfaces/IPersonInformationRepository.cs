using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.Database.Interfaces
{
    public interface IPersonInformationRepository
    {
        Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForUser(string username);
        Task<PersonInformation> GetSinglePersonInformationForUserByPersonalCode(string personalCode);
        Task<PersonInformation> GetSinglePersonInformationForUserByObjectId(Guid id);
        Task<PersonInformation> AddPersonInformationForUser(PersonInformation personInformation);
        Task<PersonInformation> UpdatePersonInformationForUserByPersonalCode(PersonInformation request);
        Task<PersonInformation> UpdatePersonInformationForUserByObjectId(PersonInformation request);
        Task<PersonInformation> DeletePersonInformationForUserByPersonalCode(string personalCode);
        Task<PersonInformation> DeletePersonInformationForUserByObjectId(Guid id);
    }
}
