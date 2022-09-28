using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudySummarySearch.API.Data;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Services.SubjectServices
{
    public class SubjectService : ISubjectService
    {
        private readonly DataContext _context;

        public SubjectService(DataContext context)
        {
            _context = context;
        }
        public ServiceResponse<List<string>> Get(int? semester)
        {
            var response = new ServiceResponse<List<string>>();

            try
            {
                var subjects = _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .Where(s => semester != null ? s.Semester.Value == semester : true)
                    .Select(s => s.Subject.Value)
                    .Distinct()
                    .ToList();

                response.Data = subjects;
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