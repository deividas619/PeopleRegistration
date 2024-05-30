using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.Attributes;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;
using Serilog;
using System.Security.Claims;

namespace PeopleRegistration.API.Controllers
{
    public class PersonInformationController : Controller
    {
        [ApiController]
        [Route("api/[controller]")]
        [Authorize]
        public class UserController(IUserService userService, IJwtService jwtService) : ControllerBase
        {
            [HttpPost("Register")]
            [Unauthorized]
            public ActionResult<ResponseDto> Register([FromQuery] UserDto request)
            {
                try
                {
                    var response = userService.Register(request.Username, request.Password);

                    if (!response.IsSuccess)
                        return BadRequest(response.Message);
                    return response;
                }
                catch (Exception e)
                {
                    Log.Error($"[{nameof(UserController)}.{nameof(Register)}]: {e.Message}");
                    throw;
                }
            }
        }
    }
}
