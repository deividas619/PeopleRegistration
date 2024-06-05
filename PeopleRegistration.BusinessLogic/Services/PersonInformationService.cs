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
        public async Task<IEnumerable<PersonInformationDto>> GetAllPeopleInformationForUser(string username)
        {
            try
            {
                var repositoryOutput = await personInformationRepository.GetAllPeopleInformationForUser(username);
                if (repositoryOutput is not null)
                    return repositoryOutput.Select(ConvertToDto).ToList();

                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(GetAllPeopleInformationForUser)}]: {e.Message}");
                throw;
            }
        }
        
        public async Task<PersonInformationDto> GetSinglePersonInformationForUserByPersonalCode(string username, string personalCode)
        {
            try
            {
                var repositoryOutput = await personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);
                if (repositoryOutput is not null)
                    return ConvertToDto(repositoryOutput);

                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(GetSinglePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<(byte[], string)> GetPersonInformationPhotoByPersonalCode(string username, string personalCode)
        {
            try
            {
                var personInformation = await personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

                if (personInformation is not null)
                    return (personInformation.ProfilePhoto, personInformation.ProfilePhotoEncoding);
                
                return (null, null);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(GetPersonInformationPhotoByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformationDto> AddPersonInformationForUser(string username, PersonInformationDto personInformation, byte[] imageBytes, string imageEncoding)
        {
            try
            {
                var existingPersonInformation = personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(username, personInformation.PersonalCode);

                if (existingPersonInformation is not null)
                    return null; // return that personinformation with this personal code already exists for this user

                await personInformationRepository.AddPersonInformationForUser(new PersonInformation
                {
                    Name = personInformation.Name,
                    LastName = personInformation.LastName,
                    Gender = personInformation.Gender,
                    DateOfBirth = personInformation.DateOfBirth,
                    PersonalCode = personInformation.PersonalCode,
                    PhoneNumber = personInformation.PhoneNumber,
                    Email = personInformation.Email,
                    ProfilePhoto = ResizePhoto(imageBytes, imageEncoding),
                    ProfilePhotoEncoding = imageEncoding,
                    User = userRepository.GetUser(username),
                    ResidencePlace = new ResidencePlace
                    {
                        City = personInformation.ResidencePlace.City != null && personInformation.ResidencePlace.City != "string" ? personInformation.ResidencePlace.City : null,
                        Street = personInformation.ResidencePlace.Street != null && personInformation.ResidencePlace.Street != "string" ? personInformation.ResidencePlace.Street : null,
                        HouseNumber = personInformation.ResidencePlace.HouseNumber != null && personInformation.ResidencePlace.HouseNumber != "string" ? personInformation.ResidencePlace.HouseNumber : null,
                        ApartmentNumber = personInformation.ResidencePlace.ApartmentNumber != null && personInformation.ResidencePlace.ApartmentNumber != "string" ? personInformation.ResidencePlace.ApartmentNumber : null
                    }
                });

                return personInformation;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(AddPersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformationDto> UpdatePersonInformationForUserByPersonalCode(string username, string personalCode, PersonInformationDto request, byte[] imageBytes, string imageEncoding)
        {
            try
            {
                var existingPersonInformation = await personInformationRepository.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

                if (existingPersonInformation is not null)
                {
                    existingPersonInformation.Name = request.Name;

                    if (imageBytes is not null)
                    {
                        existingPersonInformation.ProfilePhoto = imageBytes;
                        existingPersonInformation.ProfilePhotoEncoding = imageEncoding;
                    }

                    //return await personInformationRepository.UpdatePersonInformationForUserByPersonalCode(existingPersonInformation);
                }

                return null; // return that personinformation with this personal code does not exist for this user
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(UpdatePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        public async Task<PersonInformationDto> DeletePersonInformationForUserByPersonalCode(string username, string personalCode)
        {
            try
            {
                var existingPersonInformation = await personInformationRepository.DeletePersonInformationForUserByPersonalCode(username, personalCode);

                if (existingPersonInformation is not null)
                    return ConvertToDto(existingPersonInformation);

                return null; // return that personinformation with this personal code does not exist for this user
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(DeletePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        private byte[] ResizePhoto(byte[] imageBytes, string imageEncoding)
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
                Log.Error($"[{nameof(PersonInformationService)}.{nameof(ResizePhoto)}]: {e.Message}");
                throw;
            }
        }

        private PersonInformationDto ConvertToDto(PersonInformation personInformation)
        {
            return new PersonInformationDto
            {
                Name = personInformation.Name,
                LastName = personInformation.LastName,
                Gender = personInformation.Gender,
                DateOfBirth = personInformation.DateOfBirth,
                PersonalCode = personInformation.PersonalCode,
                PhoneNumber = personInformation.PhoneNumber,
                Email = personInformation.Email,
                ResidencePlace = personInformation.ResidencePlace != null ? ConvertToDto(personInformation.ResidencePlace) : null
            };
        }

        private ResidencePlaceDto ConvertToDto(ResidencePlace residencePlace)
        {
            return new ResidencePlaceDto
            {
                City = residencePlace.City,
                Street = residencePlace.Street,
                HouseNumber = residencePlace.HouseNumber,
                ApartmentNumber = residencePlace.ApartmentNumber
            };
        }
    }
}
