using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudySummarySearch.Contracts.Service;
using Dropbox.Api;
using Dropbox.Api.Files;
using StudySummarySearch.Contracts.Summary;
using StudySummarySearch.API.Data;
using StudySummarySearch.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace StudySummarySearch.API.Services.SummaryServices
{
    public class SummaryService : ISummaryService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SummaryService(IConfiguration config, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        ServiceResponse<List<SummaryResponseDto>> ISummaryService.Query(int? semester, string? subject, string? keyword, string? name, int? authorId)
        {
            var response = new ServiceResponse<List<SummaryResponseDto>>();

            try
            {
                response.Data = _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .Include(s => s.Keywords)
                    .Include(s => s.Author)
                    .Where(s => semester != null ? s.Semester.Value == semester : true)
                    .Where(s => subject != null ? s.Subject.Value == subject : true)
                    .Where(s => name != null ? s.Name == name : true)
                    .Where(s => authorId != null ? s.Author.Id == authorId : true)
                    .Where(s => keyword != null ? s.Keywords.Select(k => k.Value).Contains(keyword) : true)
                    .OrderBy(s => s.CreatedAt)
                    .Select(s => new SummaryResponseDto 
                    {
                        Id = s.Id,
                        URL = s.URL,
                        Name = s.Name,
                        Semester = s.Semester.Value,
                        Subject = s.Subject.Value,
                        Keywords = s.Keywords.Select(k => k.Value).ToList(),
                        Author = s.Author.Username
                    }).ToList();
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;

        }

        public async Task<ServiceResponse<int>> Add(SummaryRequestDto request)
        {
            var response = new ServiceResponse<int>();

            try
            {
                // Check, if Summary already exist, to prevent overwriting issues
                var dbSummary = await _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .FirstOrDefaultAsync(s => s.Semester.Value == request.Semester && s.Subject.Value == request.Subject && s.Name == request.Name);
                if (!(dbSummary == null))
                {
                    response.Status = ServiceErrors.Duplicate;
                    response.Message = $"Semester_{request.Semester}/{request.Subject}/{request.Name} already exists.";
                    return response;
                }

                var author = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!.ToString()));

                var summary = new Summary {
                    Name = request.Name,
                    Author = author!, 
                    Keywords = new List<Keyword>()
                };
                
                // If Semester already exists -> link existing Semester to Summary, else Create new Semester
                var dbSemester = await _context.Semesters.FirstOrDefaultAsync(s => s.Value == request.Semester);
                if (dbSemester == null)
                    summary.Semester = new Semester { Value = request.Semester };
                else
                    summary.Semester = dbSemester;
                
                // If Subject already exists -> link existing Subject to Summary, else Create new Subject
                var dbSubject = await _context.Subjects.FirstOrDefaultAsync(s => s.Value == request.Subject.ToLower());
                if (dbSubject == null)
                    summary.Subject = new Subject { Value = request.Subject.ToLower() };
                else
                    summary.Subject = dbSubject;

                if (!(request.Keywords == null))
                {
                    // If Keyword already exists -> link existing Keyword to Summary, else Create new Keyword
                    foreach (var k in request.Keywords)
                    {
                        var keyword = k.ToLower();
                        var dbKeyword = await _context.Keywords.FirstOrDefaultAsync(k => k.Value == keyword);
                        if (dbKeyword == null)
                            summary.Keywords.Add(new Keyword { Value = keyword });
                        else
                            summary.Keywords.Add(dbKeyword);
                    }
                }

                var responseSummary = _context.Summaries.Add(summary);
                await _context.SaveChangesAsync();

                response.Data = responseSummary.Entity.Id;
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<SummaryResponseDto>> Upload(int id, IFormFile image)
        {
            var response = new ServiceResponse<SummaryResponseDto>();

            try
            {
                var summary = await _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .Include(s => s.Keywords)
                    .Include(s => s.Author)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (summary == null)
                {
                    response.Status = ServiceErrors.NotFound;
                    response.Message = "Summary not found.";
                    return response;
                }

                var uploadReqUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!.ToString()));
                
                if (!(summary.Author.Id == uploadReqUser!.Id || uploadReqUser!.Role == "SuperUser")) 
                {
                    response.Status = ServiceErrors.Forbidden;
                    response.Message = "You are only allowed to upload to your own Summaries.";
                    return response;
                }

                using (var stream = new MemoryStream()) 
                {
                    await image.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    if (uploadReqUser.DropboxAccessToken == null)
                    {
                        response.Status = ServiceErrors.Unauthorized;
                        response.Message = "No Dropbox Token provided.";
                        return response;
                    }

                    try {
                        var sharedLink = await UploadToDropbox(uploadReqUser.DropboxAccessToken, stream, summary.Semester.Value, summary.Subject.Value, summary.Name);

                        summary.URL = sharedLink;
                        await _context.SaveChangesAsync();

                        response.Data = new SummaryResponseDto
                        {
                            Id = summary.Id,
                            URL = sharedLink,
                            Name = summary.Name,
                            Semester = summary.Semester.Value,
                            Subject = summary.Subject.Value,
                            Keywords = summary.Keywords.Select(k => k.Value).ToList(),
                            Author = summary.Author.Username
                        };
                    } 
                    catch {
                        response.Status = ServiceErrors.Unknown;
                        response.Message = "Dropbox token is invalid.";
                    }
                }
            }
            catch (AuthException)
            {
                response.Status = ServiceErrors.Unauthorized;
                response.Message = "Dropbox Access Token expired.";
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<SummaryResponseDto>> Update(int id, SummaryRequestDto request)
        {
            var response = new ServiceResponse<SummaryResponseDto>();

            try 
            {
                var summary = await _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .Include(s => s.Keywords)
                    .Include(s => s.Author)
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (summary == null)
                {
                    response.Status = ServiceErrors.NotFound;
                    response.Message = "Summary not found.";
                    return response;
                }

                var duplicateSummary = await _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .FirstOrDefaultAsync(s => s.Semester.Value == request.Semester && s.Subject.Value == request.Subject && s.Name == request.Name && s.Id != id);
                if (duplicateSummary != null) 
                {
                    response.Status = ServiceErrors.Duplicate;
                    response.Message = $"Semester_{request.Semester}/{request.Subject}/{request.Name} already exists.";
                    return response;
                }

                var updateReqUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!.ToString()));
                if (!(summary.Author.Id == updateReqUser!.Id || updateReqUser!.Role == "SuperUser")) 
                {
                    response.Status = ServiceErrors.Forbidden;
                    response.Message = "You are only allowed to update your own Summaries.";
                    return response;
                }

                if (
                    summary.URL != null && 
                    !(summary.Semester.Value == request.Semester && summary.Subject.Value == request.Subject && summary.Name == request.Name)
                )
                {
                    using (var dbx = new DropboxClient(updateReqUser.DropboxAccessToken)) 
                    {
                        await dbx.Files.MoveV2Async(
                            $"/Semester_{summary.Semester.Value}/{summary.Subject.Value.ToLower()}/{summary.Name}.jpg", 
                            $"/Semester_{request.Semester}/{request.Subject.ToLower()}/{request.Name}.jpg"
                        );

                        var summaryInSemesterWithSubject = await _context.Summaries
                            .Include(s => s.Semester)
                            .Include(s => s.Subject)
                            .FirstOrDefaultAsync(s => s.Subject.Value == summary.Subject.Value && s.Semester.Value == summary.Semester.Value && s.Id != summary.Id);
                        if (summaryInSemesterWithSubject == null)
                            await dbx.Files.DeleteV2Async($"/Semester_{summary.Semester.Value}/{summary.Subject.Value}");

                        var summaryWithSemester = await _context.Summaries
                            .Include(s => s.Semester)
                            .FirstOrDefaultAsync(s => s.Semester.Value == summary.Semester.Value && s.Id != summary.Id);
                        if (summaryWithSemester == null)
                            await dbx.Files.DeleteV2Async($"/Semester_{summary.Semester.Value}");
                    }
                }

                if (summary.Subject.Value != request.Subject.ToLower())
                {
                    var dbSubject = await _context.Subjects.FirstOrDefaultAsync(s => s.Value == request.Subject.ToLower());
                    if (dbSubject == null)
                        summary.Subject = new Subject { Value = request.Subject.ToLower() };
                    else
                        summary.Subject = dbSubject;
                }

                if (summary.Semester.Value != request.Semester)
                {
                    var dbSemester = await _context.Semesters.FirstOrDefaultAsync(s => s.Value == request.Semester);
                    if (dbSemester == null)
                        summary.Semester = new Semester { Value = request.Semester };
                    else
                        summary.Semester = dbSemester;
                }

                summary.Name = request.Name;

                List<string> keywords = request.Keywords == null ? new List<string>() : request.Keywords.Select(k => k.ToLower()).ToList();
                var keywordsToDelete = summary.Keywords.Select(k => k.Value).ToList().Except(keywords).ToList();
                var keywordsToCreate = keywords.Except(summary.Keywords.Select(k => k.Value).ToList()).ToList();

                foreach (var keyword in keywordsToCreate)
                {
                    var dbKeyword = await _context.Keywords.FirstOrDefaultAsync(k => k.Value == keyword);
                    if (dbKeyword == null)
                        summary.Keywords.Add(new Keyword { Value = keyword });
                    else
                        summary.Keywords.Add(dbKeyword);
                }

                foreach (var keyword in keywordsToDelete)
                {
                    var dbKeyword = await _context.Keywords.FirstOrDefaultAsync(k => k.Value == keyword);
                    summary.Keywords.Remove(dbKeyword!);
                    var summaryWithKeyword = await _context.Summaries.FirstOrDefaultAsync(s => s.Keywords.Contains(dbKeyword!) && s.Id != summary.Id);
                    if (summaryWithKeyword == null)
                        _context.Keywords.Remove(dbKeyword!);
                }

                await _context.SaveChangesAsync();

                response.Data = new SummaryResponseDto
                {
                    Id = summary.Id,
                    URL = summary.URL,
                    Name = summary.Name,
                    Semester = summary.Semester.Value,
                    Subject = summary.Subject.Value,
                    Keywords = summary.Keywords.Select(k => k.Value).ToList(),
                    Author = summary.Author.Username
                };
            }
            catch (AuthException)
            {
                response.Status = ServiceErrors.Unauthorized;
                response.Message = "Dropbox Access Token expired.";
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse> Delete(int id)
        {
            var response = new ServiceResponse();

            try
            {
                var summary = await _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .Include(s => s.Keywords)
                    .Include(s => s.Author)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (summary == null) 
                {
                    response.Status = ServiceErrors.NotFound;
                    response.Message = "Summary not found.";
                    return response;
                }

                var deleteReqUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!.ToString()));
                
                if (!(summary.Author.Id == deleteReqUser!.Id || deleteReqUser!.Role == "SuperUser")) 
                {
                    response.Status = ServiceErrors.Forbidden;
                    response.Message = "You are only allowed to delete your own Summaries.";
                    return response;
                }

                if (deleteReqUser.DropboxAccessToken == null)
                {
                    response.Status = ServiceErrors.Unauthorized;
                    response.Message = "No Dropbox Token provided.";
                    return response;
                }

                using (var dbx = new DropboxClient(deleteReqUser.DropboxAccessToken)) 
                {
                    if (summary.URL != null) await dbx.Files.DeleteV2Async($"/Semester_{summary.Semester.Value}/{summary.Subject.Value}/{summary.Name}.jpg");

                    // Remove Subject, if it doesn't exist in any other Summary and Delete Path if there are no more files in Dropbox
                    var summariesWithSubject = _context.Summaries
                        .Include(s => s.Semester)
                        .Include(s => s.Subject)
                        .Where(s => s.Subject.Value == summary.Subject.Value && s.Id != summary.Id)
                        .ToList();
                    if (summariesWithSubject.Count == 0)
                    {
                        _context.Subjects.Remove(summary.Subject);
                        if (summary.URL != null) await dbx.Files.DeleteV2Async($"/Semester_{summary.Semester.Value}/{summary.Subject.Value}");
                    } 
                    else {
                        var summaryInSemesterWithSubject = summariesWithSubject.FirstOrDefault(s => s.Semester.Value == summary.Semester.Value);
                        if (summaryInSemesterWithSubject == null) 
                        {
                            if (summary.URL != null) await dbx.Files.DeleteV2Async($"/Semester_{summary.Semester.Value}/{summary.Subject.Value}");
                        }
                    }

                    // Remove Semester, if it doesn't exist in any other Summary
                    var summaryWithSemester = await _context.Summaries
                        .Include(s => s.Semester)
                        .FirstOrDefaultAsync(s => s.Semester.Value == summary.Semester.Value && s.Id != summary.Id);
                    if (summaryWithSemester == null)
                    {
                        if (summary.URL != null) await dbx.Files.DeleteV2Async($"/Semester_{summary.Semester.Value}");
                        _context.Semesters.Remove(summary.Semester);
                    }
                }

                _context.Summaries.Remove(summary);

                // Remove Keyword, if it doesn't exist in any other Summary
                foreach (var keyword in summary.Keywords)
                {
                    var summaryWithKeyword = await _context.Summaries
                        .FirstOrDefaultAsync(s => s.Keywords.Contains(keyword) && s.Id != summary.Id);
                    if (summaryWithKeyword == null)
                        _context.Keywords.Remove(keyword);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse> DeleteImage(int id)
        {
            var response = new ServiceResponse();

            try
            {
                var summary = await _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .Include(s => s.Author)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (summary == null)
                {
                    response.Status = ServiceErrors.NotFound;
                    response.Message = "Summary not found.";
                    return response;
                }

                var deleteImageReqUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == int.Parse(_httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!.ToString()));

                if (!(summary.Author.Id == deleteImageReqUser!.Id || deleteImageReqUser!.Role == "SuperUser")) 
                {
                    response.Status = ServiceErrors.Forbidden;
                    response.Message = "You are only allowed to update your own Summaries.";
                    return response;
                }

                if (deleteImageReqUser.DropboxAccessToken == null)
                {
                    response.Status = ServiceErrors.Unauthorized;
                    response.Message = "No Dropbox Token provided.";
                    return response;
                }

                using (var dbx = new DropboxClient(deleteImageReqUser.DropboxAccessToken)) 
                {
                    await dbx.Files.DeleteV2Async($"/Semester_{summary.Semester.Value}/{summary.Subject.Value}/{summary.Name}.jpg");

                    // Remove Subject, if it doesn't exist in any other Summary and Delete Path if there are no more files in Dropbox
                    var summaryInSemesterWithSubject = await _context.Summaries
                        .Include(s => s.Semester)
                        .Include(s => s.Subject)
                        .FirstOrDefaultAsync(s => s.Subject.Value == summary.Subject.Value && s.Semester.Value == summary.Semester.Value && s.Id != summary.Id);
                    if (summaryInSemesterWithSubject == null)
                    {
                        await dbx.Files.DeleteV2Async($"/Semester_{summary.Semester.Value}/{summary.Subject.Value}");
                    }

                    // Remove Semester, if it doesn't exist in any other Summary
                    var summaryWithSemester = await _context.Summaries
                        .Include(s => s.Semester)
                        .FirstOrDefaultAsync(s => s.Semester.Value == summary.Semester.Value && s.Id != summary.Id);
                    if (summaryWithSemester == null)
                    {
                        await dbx.Files.DeleteV2Async($"/Semester_{summary.Semester.Value}");
                    }
                }

                summary.URL = null;
                await _context.SaveChangesAsync();
            } 
            catch (AuthException)
            {
                response.Status = ServiceErrors.Unauthorized;
                response.Message = "Dropbox Access Token expired.";
            }
            catch (Exception ex)
            {
                response.Status = ServiceErrors.Unknown;
                response.Message = ex.Message;
            }

            return response;
        }

        private async Task<string> UploadToDropbox(string dbxToken, Stream stream, int semester, string subject, string name) 
        {
            var path = $"/Semester_{semester}/{subject}/{name}.jpg";

            using (var dbx = new DropboxClient(dbxToken)) 
            {
                var upload = await dbx.Files.UploadAsync(
                    path,
                    WriteMode.Overwrite.Instance,
                    body: stream
                );

                var sharedLink = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(path);

                return sharedLink.Url.Split("?")[0] + "?raw=1";
            }
        }
    }
}