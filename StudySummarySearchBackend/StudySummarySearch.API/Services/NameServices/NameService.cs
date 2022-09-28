using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudySummarySearch.API.Data;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Services.NameServices
{
    public class NameService : INameService
    {
        private readonly DataContext _context;

        public NameService(DataContext context)
        {
            _context = context;
        }

        public ServiceResponse<List<string>> Query(int semester, string subject)
        {
            var response = new ServiceResponse<List<string>>();

            try
            {
                var names = _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .Where(s => s.Semester.Value == semester && s.Subject.Value == subject)
                    .OrderBy(s => s.CreatedAt)
                    .Select(s => s.Name)
                    .ToList();

                response.Data = names;
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