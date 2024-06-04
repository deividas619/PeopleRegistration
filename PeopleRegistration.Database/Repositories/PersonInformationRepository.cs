using Microsoft.EntityFrameworkCore;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;
using Serilog;

namespace PeopleRegistration.Database.Repositories
{
    public class PersonInformationRepository(ApplicationDbContext context) : IPersonInformationRepository
    {
        public async Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForUser(string username)
        {
            try
            {
                return await context.PeopleInformation.Include(pi => pi.ResidencePlace).Where(pi => pi.User.Username == username).ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(GetAllPeopleInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> GetSinglePersonInformationForUserByPersonalCode(string username, string personalCode)
        {
            try
            {
                return await context.PeopleInformation.Include(pi => pi.ResidencePlace).Where(pi => pi.User.Username == username).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(GetSinglePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> GetSinglePersonInformationForUserByObjectId(string username, Guid id)
        {
            try
            {
                return await context.PeopleInformation.Include(pi => pi.ResidencePlace).Where(pi => pi.User.Username == username && pi.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(GetSinglePersonInformationForUserByObjectId)}]: {e.Message}");
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

        public async Task<PersonInformation> UpdatePersonInformationForUserByPersonalCode(string personalCode, PersonInformation request)
        {
            try
            {
                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(UpdatePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> DeletePersonInformationForUserByPersonalCode(string username, string personalCode)
        {
            try
            {
                var personInformationToRemove = await context.PeopleInformation.Include(pi => pi.ResidencePlace).FirstOrDefaultAsync(pi => pi.PersonalCode == personalCode);

                context.PeopleInformation.Remove(personInformationToRemove);
                await context.SaveChangesAsync();

                Log.Information($"[{nameof(PersonInformationRepository)}.{nameof(DeletePersonInformationForUserByPersonalCode)}]: Successfully removed Person Information by Personal Code: {personInformationToRemove.PersonalCode}");
                return personInformationToRemove;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(DeletePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> DeletePersonInformationForUserByObjectId(string username, Guid id)
        {
            try
            {
                var personInformationToRemove = await context.PeopleInformation.Include(pi => pi.ResidencePlace).FirstOrDefaultAsync(pi => pi.Id == id);

                context.PeopleInformation.Remove(personInformationToRemove);
                await context.SaveChangesAsync();

                Log.Information($"[{nameof(PersonInformationRepository)}.{nameof(DeletePersonInformationForUserByObjectId)}]: Successfully removed Person Information by object Id: {personInformationToRemove.Id}");
                return personInformationToRemove;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}.{nameof(DeletePersonInformationForUserByObjectId)}]: {e.Message}");
                throw;
            }
        }
    }
}
