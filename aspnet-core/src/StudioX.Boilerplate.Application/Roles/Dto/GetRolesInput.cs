using System.ComponentModel.DataAnnotations;
using System.Linq;
using StudioX.Application.Services.Dto;
using StudioX.Extensions;
using StudioX.Runtime.Validation;

namespace StudioX.Boilerplate.Roles.Dto
{
    public class GetRolesInput :  IPagedResultRequest, ISortedResultRequest, ICustomValidate
    {
        public const string DefaultSorting = "IsStatic DESC, IsDefault DESC";

        [Range(0, 100)]
        public int MaxResultCount { get; set; }

        public int SkipCount { get; set; }

        public string Sorting { get; set; }

        public string PermissionName { get; set; }

        public void AddValidationErrors(CustomValidationContext context)
        {
            var validSortingValues = new[] {
                                                "DisplayName DESC", "DisplayName ASC",
                                                "CreationTime DESC", "CreationTime ASC"
                                           };

            if (Sorting.IsNullOrEmpty() || Sorting.Split(',').Any(x => x.TrimStart().IsIn(validSortingValues)))
                return;
            context.Results.Add(new ValidationResult("Sorting is not valid. Valid values: " + string.Join(", ", validSortingValues)));
        }
    }
}