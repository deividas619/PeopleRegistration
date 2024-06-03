using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Database.Interfaces;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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

        public async Task<PersonInformation> GetSinglePersonInformationForUser(string username)
        {
            try
            {
                return await personInformationRepository.GetSinglePersonInformationForUser(username);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(GetSinglePersonInformationForUser)}]: {e.Message}");
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

        public async Task<PersonInformation> UpdatePersonInformationForUser(string username)
        {
            try
            {
                return await personInformationRepository.UpdatePersonInformationForUser(username);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(UpdatePersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformation> DeletePersonInformationForUser(string username)
        {
            try
            {
                return await personInformationRepository.DeletePersonInformationForUser(username);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(DeletePersonInformationForUser)}]: {e.Message}");
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
                        int newWidth = 75;
                        int newHeight = 75;

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
                                    throw new NotSupportedException("Unsupported image format.");
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
