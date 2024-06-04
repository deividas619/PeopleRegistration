using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.DTOs;
using Serilog;
using System.Security.Claims;
using PeopleRegistration.Shared.Attributes;
using PeopleRegistration.Shared.Entities;

namespace PeopleRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PersonInformationController(IPersonInformationService personInformationService) : Controller
    {
        [HttpGet("GetAllPeopleInformationForUser")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<IEnumerable<PersonInformation>>> GetAllPeopleInformationForUser()
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.GetAllPeopleInformationForUser(username);

                if (result is null)
                    return BadRequest(result);
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
        public async Task<ActionResult<PersonInformation>> GetSinglePersonInformationForUserByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.GetSinglePersonInformationForUserByPersonalCode(username, personalCode);

                if (result is null)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(GetSinglePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [HttpGet("GetSinglePersonInformationForUserByObjectId")]
        [ResponseCache(Duration = 30)]
        public async Task<ActionResult<PersonInformation>> GetSinglePersonInformationForUserByObjectId([FromQuery] Guid id)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.GetSinglePersonInformationForUserByObjectId(username, id);

                if (result is null)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(GetSinglePersonInformationForUserByObjectId)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("AddPersonInformationForUser")]
        public async Task<ActionResult<PersonInformation>> AddPersonInformationForUser([FromQuery] PersonInformationDto request)
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
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(AddPersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("UpdatePersonInformationForUserByPersonalCode")]
        public async Task<ActionResult<PersonInformation>> UpdatePersonInformationForUserByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode, [FromQuery] PersonInformationDto request)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.UpdatePersonInformationForUserByPersonalCode(username, personalCode, request);

                if (result is null)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(UpdatePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("DeletePersonInformationForUserPersonalCode")]
        public async Task<ActionResult<PersonInformation>> DeletePersonInformationForUserByPersonalCode([FromQuery, PersonalCodeValidation] string personalCode)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.DeletePersonInformationForUserByPersonalCode(username, personalCode);

                if (result is null)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(DeletePersonInformationForUserByPersonalCode)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("DeletePersonInformationForUserByObjectId")]
        public async Task<ActionResult<PersonInformation>> DeletePersonInformationForUserByObjectId([FromQuery] Guid id)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.DeletePersonInformationForUserByObjectId(username, id);

                if (result is null)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(DeletePersonInformationForUserByObjectId)}]: {e.Message}");
                throw;
            }
        }
    }
}
