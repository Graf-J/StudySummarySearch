using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudySummarySearch.API.Models
{
    public class Summary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public User Author { get; set;} = default!;
        public Semester Semester { get; set; } = default!;
        public Subject Subject { get; set; } = default!;
        public byte[]? Image { get;set; }
        public List<Keyword> Keywords { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}