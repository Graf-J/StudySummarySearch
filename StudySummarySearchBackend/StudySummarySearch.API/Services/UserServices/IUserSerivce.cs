using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.Contracts.Service;
using StudySummarySearch.Contracts.User;

namespace StudySummarySearch.API.Services.UserServices
{
    public interface IUserService
    {
        ServiceResponse<List<UserResponseDto>> Get();
        Task<ServiceResponse> Delete(int id);
    }
}