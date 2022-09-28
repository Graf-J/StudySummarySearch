using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Services.SubjectServices
{
    public interface ISubjectService
    {
        ServiceResponse<List<string>> Get(int? semester);
    }
}