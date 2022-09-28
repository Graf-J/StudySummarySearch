using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.Contracts.Service;
using StudySummarySearch.Contracts.User;

namespace StudySummarySearch.API.Services.AuthServices
{
    public interface IAuthService
    {
        Task<ServiceResponse<UserLoginResponseDto>> Login(string userName, string password);
        Task<ServiceResponse<UserResponseDto>> Register(string userName, string password);
        Task<ServiceResponse> SetDropboxToken(string token);
        Task<ServiceResponse> SetDropboxToken(int id, string token);
    }
}