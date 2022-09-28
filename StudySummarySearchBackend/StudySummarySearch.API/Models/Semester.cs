using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StudySummarySearch.API.Models
{
    public class Semester
    {
        public int Id { get; set; }
        public int Value { get; set; }
        [JsonIgnore]
        public List<Summary> Summaries { get; set; } = default!;
    }
}