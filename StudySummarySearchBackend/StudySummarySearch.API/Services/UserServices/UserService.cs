using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.EntityFrameworkCore;
using StudySummarySearch.API.Data;
using StudySummarySearch.API.Models;
using StudySummarySearch.Contracts.Service;
using StudySummarySearch.Contracts.User;

namespace StudySummarySearch.API.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public ServiceResponse<List<UserResponseDto>> Get()
        {
            var response = new ServiceResponse<List<UserResponseDto>>();

            try
            {
                response.Data = _context.Users
                    .Select(u => new UserResponseDto { Id = u.Id, Username = u.Username})
                    .OrderBy(u => u.Username)
                    .ToList();;
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
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) 
                {
                    response.Status = ServiceErrors.NotFound;
                    response.Message = "User not found";
                    return response;
                };

                var summaries = _context.Summaries
                    .Include(s => s.Semester)
                    .Include(s => s.Subject)
                    .Include(s => s.Author)
                    .Include(s => s.Keywords)
                    .Where(s => s.Author.Id == id)
                    .ToList();

                if (summaries.Count != 0)
                {
                    using (var dbx = new DropboxClient(user.DropboxAccessToken))
                    {
                        List<DeleteArg> deleteSummaryPaths = summaries
                            .Select(s => new DeleteArg($"/Semester_{s.Semester.Value}/{s.Subject.Value}/{s.Name}.jpg", null))
                            .ToList();
                        await dbx.Files.DeleteBatchAsync(deleteSummaryPaths);

                        var semesters = new List<Semester>();
                        foreach (Summary summary in summaries) 
                        {
                            if (!semesters.Any(s => s.Id == summary.Semester.Id)) 
                                semesters.Add(summary.Semester);
                        }

                        foreach (Semester semester in semesters)
                        {
                            var summariesInSemester = _context.Summaries
                                .Include(s => s.Semester)
                                .Include(s => s.Subject)
                                .Include(s => s.Author)
                                .Where(s => s.Semester == semester && s.Author.Id != id)
                                .ToList();
                            if (summariesInSemester.Count == 0) 
                            {
                                await dbx.Files.DeleteV2Async($"/Semester_{semester.Value}");
                                _context.Semesters.Remove(semester);
                                continue;
                            }

                            var authorSubjects = summaries.Where(s => s.Semester.Id == semester.Id).Select(s => s.Subject).ToList();
                            var notAuthorSubjects = summariesInSemester.Select(s => s.Subject).ToList();
                            var subjectsToDelete = authorSubjects
                                .Where(authorSubject => !notAuthorSubjects.Any(semesterSubject => authorSubject.Id == semesterSubject.Id))
                                .ToList();
                            
                            foreach (var subject in subjectsToDelete)
                            {
                                var summaryWithSubject = await _context.Summaries
                                    .Include(s => s.Author)
                                    .Include(s => s.Subject)
                                    .Where(s => s.Author.Id != id && s.Subject.Id == subject.Id)
                                    .FirstOrDefaultAsync();
                                if (summaryWithSubject == null) 
                                    _context.Subjects.Remove(subject);
                            }

                            var deleteSubjectPaths = subjectsToDelete.Select(s => $"/Semester_{semester.Value}/{s.Value}").ToList();
                            foreach (string path in deleteSubjectPaths) 
                            {
                                await dbx.Files.DeleteV2Async(path);
                            }
                        }
                    }

                    var authorKeywords = new List<Keyword>();
                    var notAuthorKeywords = new List<Keyword>();

                    foreach (var summary in summaries)
                    {
                        foreach (var keyword in summary.Keywords) 
                        {
                            if (!authorKeywords.Any(k => k.Id == keyword.Id))
                                authorKeywords.Add(keyword);
                        }
                    }

                    var allSummaries = _context.Summaries
                        .Include(s => s.Author)
                        .Include(s => s.Keywords)
                        .Where(s => s.Author.Id != id)
                        .ToList();

                    foreach (var summary in allSummaries)
                    {
                        foreach (var keyword in summary.Keywords) 
                        {
                            if (!notAuthorKeywords.Any(k => k.Id == keyword.Id))
                                notAuthorKeywords.Add(keyword);
                        }
                    }
                    
                    var keywordsToDelete = authorKeywords
                        .Where(authorKeyword => !notAuthorKeywords.Any(notAuthorKeyword => authorKeyword == notAuthorKeyword))
                        .ToList();
                    
                    if (keywordsToDelete.Count != 0)
                        _context.Keywords.RemoveRange(keywordsToDelete);

                    _context.Summaries.RemoveRange(summaries);
                }

                _context.Users.Remove(user);
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
    }
}