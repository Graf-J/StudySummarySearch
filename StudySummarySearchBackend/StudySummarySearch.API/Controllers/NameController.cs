using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySummarySearch.API.Services.NameServices;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NameController : ControllerBase
    {
        private readonly INameService _nameService;

        public NameController(INameService nameService)
        {
            _nameService = nameService;
        }

        [HttpGet]
        public ActionResult<List<string>> Query(int semester, string subject)
        {
            var response = _nameService.Query(semester, subject);

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