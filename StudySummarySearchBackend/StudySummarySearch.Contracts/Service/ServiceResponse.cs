using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudySummarySearch.Contracts.Service
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public ServiceErrors Status { get; set; } = ServiceErrors.Ok;
        public string Message { get; set; } = string.Empty;
    }

    public class ServiceResponse
    {
        public ServiceErrors Status { get; set; } = ServiceErrors.Ok;
        public string Message { get; set; } = string.Empty;
    }
}