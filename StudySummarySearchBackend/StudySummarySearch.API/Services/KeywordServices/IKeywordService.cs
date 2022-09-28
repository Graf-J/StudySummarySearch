using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Services.KeywordServices
{
    public interface IKeywordService
    {
        ServiceResponse<List<string>> Get();
    }
}