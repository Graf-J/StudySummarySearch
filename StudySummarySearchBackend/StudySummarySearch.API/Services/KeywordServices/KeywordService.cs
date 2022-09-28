using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.API.Data;
using StudySummarySearch.Contracts.Service;
using Microsoft.EntityFrameworkCore;

namespace StudySummarySearch.API.Services.KeywordServices
{
    public class KeywordService : IKeywordService
    {
        private readonly DataContext _context;

        public KeywordService(DataContext context)
        {
            _context = context;
        }

        public ServiceResponse<List<string>> Get()
        {
            var response = new ServiceResponse<List<string>>();

            try
            {
                var keywords = _context.Keywords
                    .OrderBy(k => k.Value)
                    .Select(k => k.Value)
                    .ToList();

                response.Data = keywords;
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}