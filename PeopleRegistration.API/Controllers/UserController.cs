using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.Attributes;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Enums;
using Serilog;

namespace PeopleRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(IUserService userService, IJwtService jwtService) : ControllerBase
    {
        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Unauthorized]
        public ActionResult<ResponseDto> Register([FromQuery] UserDto request)
        {
            try
            {
                var response = userService.Register(request.Username, request.Password);

                if (!response.IsSuccess)
                    return BadRequest(response.Message);

                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserController)}_{nameof(Register)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        public ActionResult<ResponseDto> Login([FromQuery] LoginDto request)
        {
            try
            {
                var response = userService.Login(request.Username, request.Password);

                if (!response.IsSuccess)
                    return BadRequest(response.Message);

                return Ok(jwtService.GetJwtToken(request.Username, response.Role));
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserController)}_{nameof(Login)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize]
        public ActionResult<ResponseDto> ChangePassword([FromQuery] ChangePasswordDto request)
        {
            try
            {
                var username = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var response = userService.ChangeUserPassword(username, request.OldPassword, request.NewPassword, request.NewPasswordAgain);

                if (!response.IsSuccess)
                    return BadRequest(response.Message);

                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserController)}_{nameof(ChangePassword)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("ChangeRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public ActionResult<ResponseDto> ChangeRole([FromQuery, Required] string username, [FromQuery] UserRole newRole)
        {
            try
            {
                var response = userService.ChangeRole(username, newRole);

                if (!response.IsSuccess)
                    return BadRequest(response.Message);

                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserController)}_{nameof(ChangeRole)}]: {e.Message}");
                throw;
            }
        }

        [HttpGet("GetUserActiveStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public ActionResult<ResponseDto> GetUserActiveStatus([FromQuery, Required] string username)
        {
            try
            {
                var response = userService.GetUserActiveStatus(username);

                if (!response.IsSuccess)
                    return BadRequest(response.Message);

                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserController)}_{nameof(GetUserActiveStatus)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("ChangeUserActiveStatus")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public ActionResult<ResponseDto> ChangeUserActiveStatus([FromQuery, Required] string username)
        {
            try
            {
                var loggedInUsername = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var response = userService.ChangeUserActiveStatus(username, loggedInUsername);

                if (!response.IsSuccess)
                    return BadRequest(response.Message);

                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserController)}_{nameof(ChangeUserActiveStatus)}]: {e.Message}");
                throw;
            }
        }

        [HttpDelete("DeleteUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public ActionResult<ResponseDto> DeleteUser([FromQuery, Required] string username)
        {
            try
            {
                var loggedInUsername = HttpContext.User.FindFirst(ClaimTypes.Name).Value;
                var response = userService.DeleteUser(username, loggedInUsername);

                if (!response.IsSuccess)
                    return BadRequest(response.Message);

                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserController)}_{nameof(DeleteUser)}]: {e.Message}");
                throw;
            }
        }
    }
}
