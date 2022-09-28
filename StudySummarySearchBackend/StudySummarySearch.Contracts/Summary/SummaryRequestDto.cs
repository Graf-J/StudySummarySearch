using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudySummarySearch.Contracts.Summary
{
    public class SummaryRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required, Range(1, 10)]
        public int Semester { get; set; }
        [Required]
        public string Subject { get; set; } = string.Empty;
        public List<string>? Keywords { get; set; }
    }
}