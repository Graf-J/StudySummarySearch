using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Services.SemesterServices
{
    public interface ISemesterService
    {
        ServiceResponse<List<int>> Query();
    }
}