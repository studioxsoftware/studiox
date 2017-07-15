using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;

namespace StudioX.Boilerplate.Tests
{
    public abstract class BoilerplateDtoTestBase<TDto>
        where TDto : class
    {
        protected abstract TDto GetDto();

        protected async Task Validate(TDto createDto)
        {
            var context = new ValidationContext(createDto);
            ICollection<ValidationResult> results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(createDto, context, results, true);

            if (!isValid)
            {
                var message = results.Aggregate("", (current, result) => current + result.ErrorMessage);
                throw new ShouldAssertException(message);
            }
        }
    }
}