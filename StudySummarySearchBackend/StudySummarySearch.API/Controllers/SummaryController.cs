using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudySummarySearch.API.Services.SummaryServices;
using StudySummarySearch.Contracts.Service;
using StudySummarySearch.Contracts.Summary;

namespace StudySummarySearch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SummaryController : ControllerBase
    {
        private readonly ISummaryService _summaryService;

        public SummaryController(ISummaryService summaryService)
        {
            _summaryService = summaryService;
        }

        [HttpGet]
        public ActionResult<IQueryable<SummaryResponseDto>> QuerySummaries(int? semester, string? subject, string? keyword, string? name, int? authorId)
        {
            var response = _summaryService.Query(semester, subject, keyword, name, authorId);

            switch (response.Status)
            {
                case ServiceErrors.Ok:
                    return Ok(response.Data);
                default:
                    return Problem(response.Message);
            }
        }

        [Authorize(Roles = "SuperUser,Admin")]
        [HttpPost]
        public async Task<ActionResult<int>> CreateSummary(SummaryRequestDto request) 
        {
            var response = await _summaryService.Add(request);

            switch (response.Status)
            {
                case ServiceErrors.Ok:
                    return Ok(response.Data);
                case ServiceErrors.Duplicate:
                    return BadRequest(response.Message);
                case ServiceErrors.Forbidden:
                    return BadRequest(response.Message);
                default:
                    return Problem(response.Message);
            }
        }

        [Authorize(Roles = "SuperUser,Admin")]
        [HttpPost("upload/{id}")]
        public async Task<ActionResult<SummaryResponseDto>> UploadImage(int id, IFormFile image)
        {
            var response = await _summaryService.Upload(id, image);

            switch (response.Status)
            {
                case ServiceErrors.NotFound:
                    return NotFound(response.Message);
                case ServiceErrors.Unauthorized:
                    return Unauthorized(response.Message);
                case ServiceErrors.Forbidden:
                    return BadRequest(response.Message);
                case ServiceErrors.Ok:
                    return Ok(response.Data);
                default:
                    return Problem(response.Message);
            }
        }

        [Authorize(Roles = "SuperUser,Admin")]
        [HttpDelete("image/{id}")]
        public async Task<ActionResult<SummaryResponseDto>> DeleteImage(int id)
        {
            var response = await _summaryService.DeleteImage(id);

            switch (response.Status)
            {
                case ServiceErrors.NotFound:
                    return NotFound(response.Message);
                case ServiceErrors.Forbidden:
                    return BadRequest(response.Message);
                case ServiceErrors.Unauthorized:
                    return Unauthorized(response.Message);
                case ServiceErrors.Ok:
                    return Ok();
                default:
                    return Problem(response.Message);

            }
        }

        [Authorize(Roles = "SuperUser,Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<SummaryResponseDto>> Update(int id, SummaryRequestDto request)
        {
            var response = await _summaryService.Update(id, request);

            switch (response.Status)
            {
                case ServiceErrors.NotFound:
                    return NotFound(response.Message);
                case ServiceErrors.Unauthorized:
                    return Unauthorized(response.Message);
                case ServiceErrors.Forbidden:
                    return BadRequest(response.Message);
                case ServiceErrors.Duplicate:
                    return BadRequest(response.Message);
                case ServiceErrors.Ok:
                    return Ok(response.Data);
                default:
                    return Problem(response.Message);
            }
        }

        [Authorize(Roles = "SuperUser,Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSummary(int id)
        {
            var response = await _summaryService.Delete(id);

            switch (response.Status)
            {
                case ServiceErrors.NotFound:
                    return NotFound(response.Message);
                case ServiceErrors.Unauthorized:
                    return Unauthorized(response.Message);
                case ServiceErrors.Forbidden:
                    return BadRequest(response.Message);
                case ServiceErrors.Ok:
                    return Ok();
                default:
                    return Problem(response.Message);
            }
        }
    }
}