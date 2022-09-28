using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Services.NameServices
{
    public interface INameService
    {
        ServiceResponse<List<string>> Query(int semester, string subject);
    }
}