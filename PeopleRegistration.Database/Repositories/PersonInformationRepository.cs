using Microsoft.EntityFrameworkCore;
using PeopleRegistration.Database.Interfaces;
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
                Log.Error($"[{nameof(PersonInformationRepository)}_{nameof(GetAllPeopleInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForAdmin()
        {
            try
            {
                return await context.PeopleInformation.ToListAsync();
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}_{nameof(GetAllPeopleInformationForAdmin)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> GetSinglePersonInformationForUserByPersonalCode(string username, string personalCode)
        {
            try
            {
                return await context.PeopleInformation.Include(pi => pi.ResidencePlace).Where(pi => pi.User.Username == username && pi.PersonalCode == personalCode).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}_{nameof(GetSinglePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> GetSinglePersonInformationForAdminByPersonalCode(string personalCode)
        {
            try
            {
                return await context.PeopleInformation.Where(pi => pi.PersonalCode == personalCode).FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}_{nameof(GetSinglePersonInformationForAdminByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> AddPersonInformationForUser(PersonInformation request)
        {
            try
            {
                context.PeopleInformation.Add(request);
                await context.SaveChangesAsync();

                Log.Information($"[{nameof(PersonInformationRepository)}_{nameof(AddPersonInformationForUser)}]: Successfully added Person Information: '{request.Id}'");
                return request;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}_{nameof(AddPersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> UpdatePersonInformationForUserByPersonalCode(PersonInformation request)
        {
            try
            {
                context.Update(request);
                await context.SaveChangesAsync();

                Log.Information($"[{nameof(PersonInformationRepository)}_{nameof(UpdatePersonInformationForUserByPersonalCode)}]: Successfully updated Person Information by Personal Code: '{request.PersonalCode}'");
                return request;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}_{nameof(UpdatePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> DeletePersonInformationForUserByPersonalCode(PersonInformation request)
        {
            try
            {
                if (request.ResidencePlace is not null)
                    context.ResidencePlaces.Remove(request.ResidencePlace);
                
                context.PeopleInformation.Remove(request);
                await context.SaveChangesAsync();

                Log.Information($"[{nameof(PersonInformationRepository)}_{nameof(DeletePersonInformationForUserByPersonalCode)}]: Successfully removed Person Information and its Residence Place by Personal Code: '{request.PersonalCode}'");
            
                return request;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}_{nameof(DeletePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<ResidencePlace> DeleteResidencePlaceForUser(ResidencePlace request)
        {
            try
            {
                context.ResidencePlaces.Remove(request);
                await context.SaveChangesAsync();

                Log.Information($"[{nameof(PersonInformationRepository)}_{nameof(DeleteResidencePlaceForUser)}]: Successfully removed Residence Place by Id: '{request.Id}'");

                return request;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationRepository)}_{nameof(DeleteResidencePlaceForUser)}]: {e.Message}");
                throw;
            }
        }
    }
}
