using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudySummarySearch.Contracts.User
{
    public class UserLoginResponseDto
    {
        public string Username { get; set; } = string.Empty;
        public string Jwt { get; set; } = string.Empty;
    }
}