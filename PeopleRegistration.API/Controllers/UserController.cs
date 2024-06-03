using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PeopleRegistration.BusinessLogic.Interfaces;
using PeopleRegistration.Shared.Attributes;
using PeopleRegistration.Shared.DTOs;
using PeopleRegistration.Shared.Entities;
using Serilog;

namespace PeopleRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                return Ok(response);
            }
            catch (Exception e)
            {
                Log.Error($"[{nameof(UserController)}.{nameof(Register)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public ActionResult<ResponseDto> Login([FromQuery] Login request)
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
                Log.Error($"[{nameof(UserController)}.{nameof(Login)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public ActionResult<ResponseDto> ChangePassword([FromQuery] ChangePassword request)
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
                Log.Error($"[{nameof(UserController)}.{nameof(ChangePassword)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("ChangeRole")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public ActionResult<ResponseDto> ChangeRole([FromQuery] string username, [FromQuery] UserRole newRole)
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
                Log.Error($"[{nameof(UserController)}.{nameof(ChangeRole)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("GetUserActiveStatus")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public ActionResult<ResponseDto> GetUserActiveStatus([FromQuery] string username)
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
                Log.Error($"[{nameof(UserController)}.{nameof(GetUserActiveStatus)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("ChangeUserActiveStatus")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public ActionResult<ResponseDto> ChangeUserActiveStatus([FromQuery] string username)
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
                Log.Error($"[{nameof(UserController)}.{nameof(ChangeUserActiveStatus)}]: {e.Message}");
                throw;
            }
        }

        [HttpPost("DeleteUser")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public ActionResult<ResponseDto> DeleteUser([FromQuery] string username)
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
                Log.Error($"[{nameof(UserController)}.{nameof(DeleteUser)}]: {e.Message}");
                throw;
            }
        }
    }
}
