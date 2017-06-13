using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using StudioX.Application.Services.Dto;
using StudioX.Extensions;
using StudioX.Runtime.Validation;

namespace StudioX.Boilerplate.AuditLogs.Dto
{
    public class GetAuditLogsInput :  IPagedResultRequest, ISortedResultRequest, ICustomValidate
    {
        [Range(0, 100)]
        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }

        public string Sorting { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? ExecutionDurationFrom { get; set; }

        public int? ExecutionDurationTo { get; set; }

        public string UserName { get; set; }

        public string ServiceName { get; set; }

        public string MethodName { get; set; }

        public string BrowserInfo { get; set; }

        public bool? HasError { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            var validSortingValues = new[]
            {
                "UserName DESC", "UserName ASC",
                "ServiceName DESC", "ServiceName ASC",
                "MethodName DESC", "MethodName ASC",
                "ExecutionTime DESC", "ExecutionTime ASC",
                "ExecutionDuration DESC", "ExecutionDuration ASC",
                "Exception DESC", "Exception ASC",
                "ClientIpAddress DESC", "ClientIpAddress ASC",
                "ClientName DESC", "ClientName ASC",
                "BrowserInfo DESC", "BrowserInfo ASC",
                "Exception DESC", "Exception ASC"
            };

            if (Sorting.IsNullOrEmpty() || Sorting.Split(',').Any(x => x.TrimStart().IsIn(validSortingValues)))
                return;
            context.Results.Add(
                new ValidationResult("Sorting is not valid. Valid values: " + string.Join(", ", validSortingValues)));
        }
    }
}