using System.Collections.Generic;
using System.Threading.Tasks;
using Shouldly;
using StudioX.Authorization.Roles;
using StudioX.Boilerplate.Authorization;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.Roles.Dto;
using Xunit;

namespace StudioX.Boilerplate.Tests.Roles
{
    public class CreateRoleInputTests : BoilerplateDtoTestBase<CreateRoleInput>
    {
        protected override CreateRoleInput GetDto()
        {
            return new CreateRoleInput
            {
                Name = "New Role",
                DisplayName = "New Role",
                Description = "New Role Description",
                Permissions = new List<string>
                    {
                        PermissionNames.System.Administration.Users.MainMenu,
                        PermissionNames.System.Administration.Roles.MainMenu
                    }
            };
        }

        [Fact]
        public virtual async Task NameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var input = GetDto();
            input.Name = null;

            //Act, Assert
            await Validate(input)
                .ShouldThrowAsync<ShouldAssertException>();
        }


        [Fact]
        public virtual async Task NameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var input = GetDto();
            input.Name = string.Empty;

            //Act, Assert
            await Validate(input)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task NameWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var input = GetDto();
            input.Name = new string('a', StudioXRoleBase.MaxNameLength + 1);

            //Act, Assert
            await Validate(input)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task NameWithSpacesInValueShouldBeValid()
        {
            //Arrange
            var input = GetDto();
            input.Name = "Role Name With Space";

            //Act, Assert
            await Validate(input);
        }

        [Fact]
        public virtual async Task DisplayNameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var input = GetDto();
            input.DisplayName = null;

            //Act, Assert
            await Validate(input)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task DisplayNameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var input = GetDto();
            input.DisplayName = string.Empty;

            //Act, Assert
            await Validate(input)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task DisplayNameWithSpacesInValueShouldBeValid()
        {
            //Arrange
            var input = GetDto();
            input.DisplayName = "Role DisplayName With Space";

            //Act, Assert
            await Validate(input);
        }

        [Fact]
        public virtual async Task DisplayNameWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var input = GetDto();
            input.DisplayName = new string('a', StudioXRoleBase.MaxDisplayNameLength + 1);

            //Act, Assert
            await Validate(input)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public virtual async Task DescriptionWithNullValueShouldBeValid()
        {
            //Arrange
            var input = GetDto();
            input.Description = null;

            //Act, Assert
            await Validate(input);
        }

        [Fact]
        public virtual async Task DescriptionWithEmptyValueShouldBeValid()
        {
            //Arrange
            var input = GetDto();
            input.Description = string.Empty;

            //Act, Assert
            await Validate(input);
        }

        [Fact]
        public virtual async Task DescriptionWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var input = GetDto();
            input.Description = new string('a', Role.MaxDescriptionLength + 1);

            //Act, Assert
            await Validate(input)
                .ShouldThrowAsync<ShouldAssertException>();
        }
    }
}