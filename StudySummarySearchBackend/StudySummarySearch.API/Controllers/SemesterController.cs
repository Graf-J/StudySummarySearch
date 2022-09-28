using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudySummarySearch.API.Services.SemesterServices;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemesterController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet]
        public ActionResult<List<int>> Query()
        {
            var response = _semesterService.Query();

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