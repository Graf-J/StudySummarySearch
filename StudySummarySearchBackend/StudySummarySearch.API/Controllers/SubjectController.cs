using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudySummarySearch.API.Services.SubjectServices;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public ActionResult<List<string>> Get(int? semester)
        {
            var response = _subjectService.Get(semester);

            switch (response.Status)
            {
                case ServiceErrors.Ok:
                    return Ok(response.Data);
                default:
                    return Problem(response.Message);
            }
        }
    }
}