using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySummarySearch.API.Services.AuthServices;
using StudySummarySearch.Contracts.Service;
using StudySummarySearch.Contracts.User;

namespace StudySummarySearch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authSerivce;

        public AuthController(IAuthService authSerivce)
        {
            _authSerivce = authSerivce;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponseDto>> LoginUser(UserLoginRequestDto request) 
        {
            var response = await _authSerivce.Login(request.UserName, request.Password);

            switch (response.Status) 
            {
                case ServiceErrors.NotFound:
                    return NotFound(response.Message);
                case ServiceErrors.Unauthorized:
                    return Unauthorized(response.Message);
                case ServiceErrors.Ok:
                    return Ok(response.Data);
                default:
                    return Problem(response.Message);
            }
        }

        [Authorize(Roles = "SuperUser")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserRegisterRequestDto request)
        {
            var response = await _authSerivce.Register(request.UserName, request.Password);

            switch (response.Status)
            {
                case ServiceErrors.Duplicate:
                    return BadRequest(response.Message);
                case ServiceErrors.Ok:
                    return Ok(response.Data);
                default:
                    return Problem(response.Message);
            }
        }

        [Authorize(Roles = "SuperUser")]
        [HttpPost("dropbox-token")]
        public async Task<IActionResult> SetDropboxToken(UserSetDropboxTokenRequest request)
        {
            var response = await _authSerivce.SetDropboxToken(request.Token);

            switch (response.Status)
            {
                case ServiceErrors.NotFound:
                    return BadRequest(response.Message);
                case ServiceErrors.Ok:
                    return Ok();
                default:
                    return Problem(response.Message);
            }
        }

        [Authorize(Roles = "SuperUser")]
        [HttpPost("dropbox-token/{id}")]
        public async Task<IActionResult> SetDropboxToken(int id, UserSetDropboxTokenRequest request)
        {
            var response = await _authSerivce.SetDropboxToken(id, request.Token);

            switch (response.Status)
            {
                case ServiceErrors.NotFound:
                    return BadRequest(response.Message);
                case ServiceErrors.Ok:
                    return Ok();
                default:
                    return Problem(response.Message);
            }
        }
    }
}