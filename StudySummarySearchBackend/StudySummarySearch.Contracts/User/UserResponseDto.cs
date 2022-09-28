using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudySummarySearch.Contracts.User
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}