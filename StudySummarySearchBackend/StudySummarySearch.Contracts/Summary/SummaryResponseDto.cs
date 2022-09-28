using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudySummarySearch.Contracts.Summary
{
    public class SummaryResponseDto
    {
        public int Id { get; set; }
        public string? URL { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Semester { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public List<string> Keywords { get; set; } = default!;
        public string Author { get; set; } = default!;
    }
}