using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using StudySummarySearch.API.Data;
using Microsoft.EntityFrameworkCore;
using StudySummarySearch.API.Models;
using StudySummarySearch.Contracts.Service;
using StudySummarySearch.Contracts.User;
using System.Security.Claims;

namespace StudySummarySearch.API.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IConfiguration config, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<UserLoginResponseDto>> Login(string userName, string password)
        {
            var response = new ServiceResponse<UserLoginResponseDto>();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

                if (user == null) {
                    response.Status = ServiceErrors.NotFound;
                    response.Message = "User not found.";
                    return response;
                }

                if (!IsPasswordValid(password, user.PasswordHash, user.PasswordSalt)) {
                    response.Status = ServiceErrors.Unauthorized;
                    response.Message = "Invalid Password.";
                    return response;
                }

                response.Data = new UserLoginResponseDto {
                    Username = user.Username,
                    Jwt = GenerateToken(user.Id, user.Role)
                };
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<UserResponseDto>> Register(string userName, string password)
        {
            var response = new ServiceResponse<UserResponseDto>();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == userName);

                if (!(user == null)) 
                {
                    response.Status = ServiceErrors.Duplicate;
                    response.Message = "Username already in use";
                    return response;
                }

                GeneratePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

                var addedUser = new User { 
                    Username = userName, 
                    PasswordHash = passwordHash, 
                    PasswordSalt = passwordSalt
                };
                _context.Users.Add(addedUser);

                await _context.SaveChangesAsync();
                response.Data = new UserResponseDto {
                    Id = addedUser.Id,
                    Username = addedUser.Username
                };
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }

        private void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) 
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool IsPasswordValid(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string GenerateToken(int userId, string userRole)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, userRole)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:TokenSecret").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(6),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}