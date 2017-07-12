using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using StudioX.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Roles.Dto;
using Xunit;

namespace StudioX.Boilerplate.Tests.Roles
{
    public class UpdateRoleInputTests : BoilerplateDtoTestBase<UpdateRoleInput>
    {
        protected override UpdateRoleInput GetDto()
        {
            return new UpdateRoleInput
            {
                Id = 1,
                Name = "Role",
                DisplayName = "Role",
                Description = "Role Description",
                Permissions = new List<string> {"PermissionName"}
            };
        }

        [Fact]
        public virtual async Task NameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.Name = null;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task NameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.Name = string.Empty;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task NameWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.Name = new string('a', StudioXRoleBase.MaxNameLength + 1);

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task NameWithSpacesInValueShouldBeValid()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.Name = "Role Name With Space";

            //Act, Assert
            await Validate(updateInput);
        }

        [Fact]
        public virtual async Task DisplayNameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.DisplayName = null;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task DisplayNameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.DisplayName = string.Empty;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task DisplayNameWithSpacesInValueShouldBeValid()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.DisplayName = "Role DisplayName With Space";

            //Act, Assert
            await Validate(updateInput);
        }

        [Fact]
        public virtual async Task DisplayNameWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.DisplayName = new string('a', StudioXRoleBase.MaxDisplayNameLength + 1);

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task DescriptionWithNullValueShouldBeValid()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.Description = null;

            //Act, Assert
            await Validate(updateInput);
        }

        [Fact]
        public virtual async Task DescriptionWithEmptyValueShouldBeValid()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.Description = string.Empty;

            //Act, Assert
            await Validate(updateInput);
        }

        [Fact]
        public virtual async Task DescriptionWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.Description = new string('a', Role.MaxDescriptionLength + 1);

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }
    }
}