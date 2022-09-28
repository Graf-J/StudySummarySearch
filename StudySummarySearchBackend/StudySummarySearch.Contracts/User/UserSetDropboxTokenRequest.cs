using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudySummarySearch.Contracts.User
{
    public class UserSetDropboxTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}