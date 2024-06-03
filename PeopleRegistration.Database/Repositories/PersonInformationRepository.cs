using Microsoft.EntityFrameworkCore;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.Entities;
using Serilog;

namespace PeopleRegistration.Database.Repositories
{
    public class PersonInformationRepository(ApplicationDbContext context, IUserRepository userRepository) : IPersonInformationRepository
    {
        public async Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForUser(string username)
        {
            try
            {
                return await context.PeopleInformation.Include(pi => pi.ResidencePlace).Where(pi => pi.User.Id == userRepository.GetUser(username).Id).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(GetAllPeopleInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> GetSinglePersonInformationForUser(string username)
        {
            try
            {
                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(GetSinglePersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> AddPersonInformationForUser(PersonInformation personInformation)
        {
            try
            {
                context.PeopleInformation.Add(personInformation);
                await context.SaveChangesAsync();

                Log.Information($"[{nameof(PersonInformationRepository)}.{nameof(AddPersonInformationForUser)}]: Successfully added Person Information: {personInformation.Id}");
                return personInformation;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(AddPersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> UpdatePersonInformationForUser(string username)
        {
            try
            {
                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(UpdatePersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> DeletePersonInformationForUser(string username)
        {
            try
            {
                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(DeletePersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }
    }
}
