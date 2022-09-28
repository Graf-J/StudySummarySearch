using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudySummarySearch.API.Services.KeywordServices;
using StudySummarySearch.Contracts.Service;

namespace StudySummarySearch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KeywordController : ControllerBase
    {
        private readonly IKeywordService _keywordService;

        public KeywordController(IKeywordService keywordService)
        {
            _keywordService = keywordService;
        }

        [HttpGet]
        public ActionResult<List<string>> Get()
        {
            var response = _keywordService.Get();

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