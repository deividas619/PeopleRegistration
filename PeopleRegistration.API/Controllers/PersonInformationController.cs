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
                var response = await personInformationService.GetAllPeopleInformationForUser(username);

                /*if (!response.IsSuccess)
                    return BadRequest(response.Message);
                return response;*/
                return null;
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
                var response = await personInformationService.GetSinglePersonInformationForUser(username);

                /*if (!response.IsSuccess)
                    return BadRequest(response.Message);
                return response;*/
                return null;
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(PersonInformationController)}.{nameof(GetSinglePersonInformationForUser)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("AddPersonInformationForUser")]
        public async Task<ActionResult<PersonInformation>> AddPersonInformationForUser([FromQuery] UserDto request)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var response = await personInformationService.AddPersonInformationForUser(username);

                /*if (!response.IsSuccess)
                    return BadRequest(response.Message);
                return response;*/
                return null;
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
                //var response = await personInformationService.UpdatePersonInformationForUser();

                /*if (!response.IsSuccess)
                    return BadRequest(response.Message);
                return response;*/
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
                //var response = await personInformationService.DeletePersonInformationForUser();

                /*if (!response.IsSuccess)
                    return BadRequest(response.Message);
                return response;*/
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
