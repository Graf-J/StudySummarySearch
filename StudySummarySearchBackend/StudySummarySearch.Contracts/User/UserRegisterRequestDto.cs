using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudySummarySearch.Contracts.User
{
    public class UserRegisterRequestDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required, Compare("Password")]
        public string ComparePassword { get; set; } = string.Empty;
    }
}