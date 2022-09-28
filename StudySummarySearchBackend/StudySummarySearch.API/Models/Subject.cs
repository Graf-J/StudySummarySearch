using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudySummarySearch.API.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Value { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Summary> Summaries { get; set; } = default!;
    }
}