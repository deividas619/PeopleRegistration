using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.Database.Interfaces
{
    public interface IPersonInformationRepository
    {
        Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForUser(string username);
        Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForAdmin();
        Task<PersonInformation> GetSinglePersonInformationForUserByPersonalCode(string username, string personalCode);
        Task<PersonInformation> GetSinglePersonInformationForAdminByPersonalCode(string personalCode);
        Task<PersonInformation> AddPersonInformationForUser(PersonInformation personInformation);
        Task<PersonInformation> UpdatePersonInformationForUserByPersonalCode(PersonInformation request);
        Task<PersonInformation> DeletePersonInformationForUserByPersonalCode(PersonInformation request);
        Task<ResidencePlace> DeleteResidencePlaceForUser(ResidencePlace request);
    }
}
