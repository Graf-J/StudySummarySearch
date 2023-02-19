using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudySummarySearch.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string? DropboxAccessToken { get; set; }
        public byte[] PasswordHash { get; set; } = {};
        public byte[] PasswordSalt { get; set; } = {};
    }
}