using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.DTOs;
using Serilog;
using System.Security.Claims;
using PeopleRegistration.Shared.Attributes;
using PeopleRegistration.Shared.Enums;

namespace PeopleRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonInformationController(IPersonInformationService personInformationService) : Controller
    {
        [HttpGet("GetAllPeopleInformationForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<PersonInformationDto>>> GetAllPeopleInformationForUser()
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.GetAllPeopleInformationForUser(username);

                if (result is null)
                    return NotFound($"There is no information stored for User '{username}'!");

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}_{nameof(GetAllPeopleInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        [HttpGet("GetAllPeopleInformationForAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 60)]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<IEnumerable<PersonInformationAdminDto>>> GetAllPeopleInformationForAdmin()
        {
            try
            {
                var result = await personInformationService.GetAllPeopleInformationForAdmin();

                if (result is null)
                    return NotFound($"There is no information stored!");

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}_{nameof(GetAllPeopleInformationForAdmin)}]: {e.Message}");
                throw;
            }
        }

        [HttpGet("GetSinglePersonInformationForUserByPersonalCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 30)]
        public async Task<ActionResult<PersonInformationDto>> GetSinglePersonInformationForUserByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

                if (result is null)
                    return NotFound($"There is no information by Personal Code '{personalCode}' stored for User '{username}'!");

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}_{nameof(GetSinglePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [HttpGet("GetSinglePersonInformationForAdminByPersonalCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 30)]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<ActionResult<PersonInformationAdminDto>> GetSinglePersonInformationForAdminByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode)
        {
            try
            {
                var result = await personInformationService.GetSinglePersonInformationForAdminByPersonalCode(personalCode);

                if (result is null)
                    return NotFound($"There is no information by Personal Code '{personalCode}'!");

                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}_{nameof(GetSinglePersonInformationForAdminByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [HttpGet("GetPersonInformationPhotoByPersonalCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ResponseCache(Duration = 300)]
        public async Task<ActionResult<PersonInformationDto>> GetPersonInformationPhotoByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var (result, type) = await personInformationService.GetPersonInformationPhotoByPersonalCode(username, personalCode);

                if (result is null)
                    return NotFound($"No image was found by Personal Code '{personalCode}' for User '{username}'!");

                return File(result, type);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}_{nameof(GetPersonInformationPhotoByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("AddPersonInformationForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
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

                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}_{nameof(AddPersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        [HttpPut("UpdatePersonInformationForUserByPersonalCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PersonInformationDto>> UpdatePersonInformationForUserByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode, [FromForm] PersonInformationUpdateDto request)
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

                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}_{nameof(UpdatePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [HttpDelete("DeletePersonInformationForUserPersonalCode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PersonInformationDto>> DeletePersonInformationForUserByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.DeletePersonInformationForUserByPersonalCode(username, personalCode);

                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}_{nameof(DeletePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }
    }
}
