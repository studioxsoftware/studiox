using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using StudioX.Application.Services.Dto;
using StudioX.Extensions;
using StudioX.Runtime.Validation;

namespace StudioX.Boilerplate.Users.Dto
{
    public class GetUsersInput :  IPagedResultRequest, ISortedResultRequest, ICustomValidate
    {
        [Range(0, 100)]
        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }

        public string Sorting { get; set; }

        public string SearchString { get; set; }

        public List<string> PermissionNames { get; set; }

        public List<long> RoleIds { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            var validSortingValues = new[]
            {
                "FirstName DESC", "FirstName ASC",
                "LastName DESC", "LastName ASC",
                "UserName DESC", "UserName ASC",
                "EmailAddress DESC", "EmailAddress ASC",
                "LastLoginTime DESC", "LastLoginTime ASC",
                "IsActive DESC", "IsActive ASC",
                "CreationTime DESC", "CreationTime ASC"
            };

            if (Sorting.IsNullOrEmpty() || Sorting.Split(',').Any(x => x.TrimStart().IsIn(validSortingValues)))
                return;
            context.Results.Add(
                new ValidationResult("Sorting is not valid. Valid values: " + string.Join(", ", validSortingValues)));
        }
    }
}