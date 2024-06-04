using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Net;

namespace PeopleRegistration.BusinessLogic.Services
{
    public class PersonInformationService(IPersonInformationRepository personInformationRepository, IUserRepository userRepository) : IPersonInformationService
    {
        public async Task<IEnumerable<PersonInformation>> GetAllPeopleInformationForUser(string username)
        {
            try
            {
                return await personInformationRepository.GetAllPeopleInformationForUser(username);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(GetAllPeopleInformationForUser)}]: {e.Message}");
                throw;
            }
        }
        
        public async Task<PersonInformation> GetSinglePersonInformationForUserByPersonalCode(string username, string personalCode)
        {
            try
            {
                return await personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(GetSinglePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> GetSinglePersonInformationForUserByObjectId(string username, Guid id)
        {
            try
            {
                return await personInformationRepository.GetSinglePersonInformationForUserByObjectId(username, id);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(GetSinglePersonInformationForUserByObjectId)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> AddPersonInformationForUser(string username, PersonInformationDto personInformation, byte[] imageBytes, string imageEncoding)
        {
            try
            {
                return await personInformationRepository.AddPersonInformationForUser(new PersonInformation
                {
                    Id = Guid.NewGuid(),
                    Name = personInformation.Name,
                    LastName = personInformation.LastName,
                    Gender = personInformation.Gender,
                    DateOfBirth = personInformation.DateOfBirth,
                    PersonalCode = personInformation.PersonalCode,
                    PhoneNumber = personInformation.PhoneNumber,
                    Email = personInformation.Email,
                    ProfilePhoto = imageBytes,
                    ProfilePhotoEncoding = imageEncoding,
                    ProfilePhotoThumbnail = GenerateThumbnail(imageBytes, imageEncoding),
                    User = userRepository.GetUser(username),
                    ResidencePlace = personInformation.ResidencePlace
                });
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(AddPersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> UpdatePersonInformationForUserByPersonalCode(string username, string personalCode, PersonInformationDto request)
        {
            try
            {
                var existingPersonInformation = await personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

                if (existingPersonInformation == null)
                    //return new ResponseDto(false, "Not found!");
                    return null;

                if (existingPersonInformation is not null)
                {


                    return await personInformationRepository.UpdatePersonInformationForUserByPersonalCode(personalCode, newRequest);
                }
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(UpdatePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> DeletePersonInformationForUserByPersonalCode(string username, string personalCode)
        {
            try
            {
                return await personInformationRepository.DeletePersonInformationForUserByPersonalCode(username, personalCode);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(DeletePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> DeletePersonInformationForUserByObjectId(string username, Guid id)
        {
            try
            {
                return await personInformationRepository.DeletePersonInformationForUserByObjectId(username, id);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(DeletePersonInformationForUserByObjectId)}]: {e.Message}");
                throw;
            }
        }

        public byte[] GenerateThumbnail(byte[] imageBytes, string imageEncoding)
        {
            try
            {
                if (imageBytes is not null && imageEncoding is not null)
                {
                    using (var image = Image.Load(imageBytes))
                    {
                        int newWidth = 200;
                        int newHeight = 200;

                        image.Mutate(i => i.Resize(newWidth, newHeight));

                        using (var resizedStream = new MemoryStream())
                        {
                            switch (imageEncoding)
                            {
                                case "image/png":
                                    image.SaveAsPngAsync(resizedStream);
                                    break;
                                case "image/jpg":
                                    image.SaveAsJpegAsync(resizedStream);
                                    break;
                                case "image/jpeg":
                                    image.SaveAsJpegAsync(resizedStream);
                                    break;
                                case "image/gif":
                                    image.SaveAsGifAsync(resizedStream);
                                    break;
                                default:
                                    throw new NotSupportedException("Unsupported image format!");
                            }

                            return resizedStream.ToArray();
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(GenerateThumbnail)}]: {e.Message}");
                throw;
            }
        }
    }
}
