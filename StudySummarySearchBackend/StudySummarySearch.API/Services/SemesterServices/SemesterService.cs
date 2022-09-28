using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.API.Data;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Services.SemesterServices
{
    public class SemesterService : ISemesterService
    {
        private readonly DataContext _context;

        public SemesterService(DataContext context)
        {
            _context = context;
        }
        
        public ServiceResponse<List<int>> Query()
        {
            var response = new ServiceResponse<List<int>>();

            try
            {
                var semesters = _context.Semesters
                    .OrderBy(s => s.Value)
                    .Select(s => s.Value)
                    .ToList();

                response.Data = semesters;
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