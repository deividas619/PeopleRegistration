using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonRegistration.BusinessLogic.Interfaces;
using PersonRegistration.Shared.DTOs;

namespace PersonRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, IJwtService jwtService) : ControllerBase
    {
        [HttpPost("Register")]
        [AllowAnonymous]
        public ActionResult<ResponseDto> Register([FromQuery] UserDto request)
        {
            var response = userService.Register(request.Username, request.Password);

            if (!response.IsSuccess)
                return BadRequest(response.Message);
            return response;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public ActionResult<ResponseDto> Login(string username, string password)
        {
            var response = userService.Login(username, password);

            if (!response.IsSuccess)
                return BadRequest(response.Message);

            return Ok(jwtService.GetJwtToken(username));
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public ActionResult<ResponseDto> ChangePassword(string oldPassword, string newPassword, string newPasswordAgain)
        {
            var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
            var response = userService.ChangeUserPassword(username, oldPassword, newPassword, newPasswordAgain);

            if (!response.IsSuccess)
                return BadRequest(response.Message);
            return response;
        }
    }
}
