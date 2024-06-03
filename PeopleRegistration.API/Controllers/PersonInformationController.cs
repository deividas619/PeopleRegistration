using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.DTOs;
using Serilog;
using System.Security.Claims;
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
        public async Task<ActionResult<IEnumerable<PersonInformation>>> GetAllPeopleInformationForUser([FromQuery] UserDto request)
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

        [HttpGet("GetSinglePersonInformationForUser")]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<PersonInformation>> GetSinglePersonInformationForUser([FromQuery] UserDto request)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var result = await personInformationService.GetSinglePersonInformationForUser(username);

                if (result is null)
                    return BadRequest(result);
                return Ok(result);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(GetSinglePersonInformationForUser)}]: {e.Message}");
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

        [HttpPost("UpdatePersonInformationForUser")]
        public async Task<ActionResult<PersonInformation>> UpdatePersonInformationForUser([FromQuery] UserDto request)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                /*var result = await personInformationService.UpdatePersonInformationForUser();

                if (result is null)
                    return BadRequest(result);
                return Ok(result);*/
                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(UpdatePersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("DeletePersonInformationForUser")]
        public async Task<ActionResult<PersonInformation>> DeletePersonInformationForUser([FromQuery] UserDto request)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                /*var result = await personInformationService.DeletePersonInformationForUser();

                if (result is null)
                    return BadRequest(result);
                return Ok(result);*/
                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(DeletePersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }
    }
}
