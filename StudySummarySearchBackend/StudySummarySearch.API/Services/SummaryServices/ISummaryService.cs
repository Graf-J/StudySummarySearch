using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.Contracts.Service;
using StudySummarySearch.Contracts.Summary;

namespace StudySummarySearch.API.Services.SummaryServices
{
    public interface ISummaryService
    {
        ServiceResponse<List<SummaryResponseDto>> Query(int? semester, string? subject, string? keyword, string? name, int? authorId);
        Task<ServiceResponse<int>> Add(SummaryRequestDto request);
        Task<ServiceResponse<SummaryResponseDto>> Upload(int id, IFormFile image);
        Task<ServiceResponse> DeleteImage(int id);
        Task<ServiceResponse<SummaryResponseDto>> Update(int id, SummaryRequestDto request);
        Task<ServiceResponse> Delete(int id);
    }
}