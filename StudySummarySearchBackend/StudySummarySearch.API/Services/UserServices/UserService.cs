using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudySummarySearch.API.Data;
using StudySummarySearch.API.Models;
using StudySummarySearch.Contracts.Service;
using StudySummarySearch.Contracts.User;

namespace StudySummarySearch.API.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public ServiceResponse<List<UserResponseDto>> Get()
        {
            var response = new ServiceResponse<List<UserResponseDto>>();

            try
            {
                response.Data = _context.Users
                    .Select(u => new UserResponseDto { Id = u.Id, Username = u.Username})
                    .OrderBy(u => u.Username)
                    .ToList();;
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse> Delete(int id)
        {
            var response = new ServiceResponse();
    
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) 
                {
                    response.Status = ServiceErrors.NotFound;
                    response.Message = "User not found";
                    return response;
                };

                var summaries = _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .Include(s => s.Author)
                    .Include(s => s.Keywords)
                    .Where(s => s.Author.Id == id)
                    .ToList();

                // TODO: Remove all Summaries, Keywords, Subjects and Semesters, which relate to only this Author

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}