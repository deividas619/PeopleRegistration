using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.DTOs;
using Serilog;
using System.Security.Claims;
using PeopleRegistration.Shared.Attributes;

namespace PeopleRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonInformationController(IPersonInformationService personInformationService) : Controller
    {
        [HttpGet("GetAllPeopleInformationForUser")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<PersonInformationDto>>> GetAllPeopleInformationForUser()
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.GetAllPeopleInformationForUser(username);

                if (result is null)
                    return BadRequest($"There is no information stored for User '{username}'!");

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(GetAllPeopleInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        [HttpGet("GetSinglePersonInformationForUserByPersonalCode")]
        [ResponseCache(Duration = 30)]
        //public async Task<ActionResult<PersonInformationDto>> GetSinglePersonInformationForUserByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode)
        public async Task<ActionResult<PersonInformationDto>> GetSinglePersonInformationForUserByPersonalCode([FromQuery] string personalCode)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

                if (result is null)
                    return BadRequest($"There is no information by Personal Code '{personalCode}' stored for User '{username}'!");

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(GetSinglePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [ResponseCache(Duration = 300)]
        [HttpGet("GetPersonInformationPhotoByPersonalCode")]
        //public async Task<ActionResult<PersonInformationDto>> GetPersonInformationPhotoByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode)
        public async Task<ActionResult<PersonInformationDto>> GetPersonInformationPhotoByPersonalCode([FromQuery] string personalCode)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var (result, type) = await personInformationService.GetPersonInformationPhotoByPersonalCode(username, personalCode);

                if (result is null)
                    return BadRequest($"No image was found by Personal Code '{personalCode}' for User '{username}'!");

                return File(result, type);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(GetPersonInformationPhotoByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("AddPersonInformationForUser")]
        public async Task<ActionResult<PersonInformationDto>> AddPersonInformationForUser([FromForm] PersonInformationDto request)
        {
            try
            {
                byte[] imageBytes = null;
                string imageEncoding = null;

                if (request.ProfilePhoto is not null)
                {
                    using var memoryStream = new MemoryStream();
                    request.ProfilePhoto.CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                    imageEncoding = request.ProfilePhoto.ContentType;
                }

                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.AddPersonInformationForUser(username, request, imageBytes, imageEncoding);

                if (result is null)
                    return BadRequest("Failed to add Person Information!");

                // return that personinformation with this personal code already exists for this user

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(AddPersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        [HttpPut("UpdatePersonInformationForUserByPersonalCode")]
        //public async Task<ActionResult<PersonInformationDto>> UpdatePersonInformationForUserByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode, [FromForm] PersonInformationDto request)
        public async Task<ActionResult<PersonInformationDto>> UpdatePersonInformationForUserByPersonalCode([FromQuery] string personalCode, [FromForm] PersonInformationDto request)
        {
            try
            {
                byte[] imageBytes = null;
                string imageEncoding = null;

                if (request.ProfilePhoto is not null)
                {
                    using var memoryStream = new MemoryStream();
                    request.ProfilePhoto.CopyTo(memoryStream);
                    imageBytes = memoryStream.ToArray();
                    imageEncoding = request.ProfilePhoto.ContentType;
                }

                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.UpdatePersonInformationForUserByPersonalCode(username, personalCode, request, imageBytes, imageEncoding);

                if (result is null)
                    return BadRequest(result);

                // return that personinformation with this personal code does not exist for this user

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(UpdatePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [HttpDelete("DeletePersonInformationForUserPersonalCode")]
        //public async Task<ActionResult<PersonInformationDto>> DeletePersonInformationForUserByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode)
        public async Task<ActionResult<PersonInformationDto>> DeletePersonInformationForUserByPersonalCode([FromQuery] string personalCode)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.DeletePersonInformationForUserByPersonalCode(username, personalCode);

                if (result is null)
                    return BadRequest($"Failed to delete Person Information by Personal Code '{personalCode}' for User '{username}'!");

                // return that personinformation with this personal code does not exist for this user

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(DeletePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }
    }
}
