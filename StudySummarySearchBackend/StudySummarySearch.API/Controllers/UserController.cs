using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySummarySearch.API.Services.UserServices;
using StudySummarySearch.Contracts.Service;
using StudySummarySearch.Contracts.User;

namespace StudySummarySearch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<UserResponseDto>> GetUsers()
        {
            var response = _userService.Get();

            switch (response.Status)
            {
                case ServiceErrors.Ok:
                    return Ok(response.Data);
                default:
                    return Problem(response.Message);
            }
        }

        [Authorize(Roles = "SuperUser")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userService.Delete(id);

            switch (response.Status)
            {
                case ServiceErrors.NotFound:
                    return NotFound(response.Message);
                case ServiceErrors.Unauthorized:
                    return Unauthorized(response.Message);
                case ServiceErrors.Ok:
                    return Ok();
                default:
                    return Problem(response.Message);
            }
        }
    }
}