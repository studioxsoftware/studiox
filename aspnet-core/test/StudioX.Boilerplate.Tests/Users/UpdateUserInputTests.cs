using System.Threading.Tasks;
using Shouldly;
using StudioX.Authorization.Users;
using StudioX.Boilerplate.Users.Dto;
using Xunit;

namespace StudioX.Boilerplate.Tests.Users
{
    public class UpdateUserInputTests : BoilerplateDtoTestBase<UpdateUserInput>
    {
        protected override UpdateUserInput GetDto()
        {
            return new UpdateUserInput
            {
                Id = 1,
                EmailAddress = "new.user@studioxsoftware.com",
                IsActive = true,
                FirstName = "New",
                LastName = "User",
                UserName = "New.User",
                Roles = new[] {"Admin"}
            };
        }

        [Fact]
        public async Task NameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.FirstName = null;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task NameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.FirstName = string.Empty;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task NameWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.FirstName = new string('a', StudioXUserBase.MaxFirstNameLength + 1);

            //Act, Assert
            await Validate(updateInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task LastNameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.LastName = null;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task LastNameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.LastName = string.Empty;

            //Act, Assert
            await Validate(updateInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task LastNameWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.LastName = new string('a', StudioXUserBase.MaxLastNameLength + 1);

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }
        
        [Fact]
        public async Task EmailAddressWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.EmailAddress = null;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task EmailAddressWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.EmailAddress = string.Empty;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task EmailAddressWithInvalidValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.EmailAddress = "InvalidEmailAddress";

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task UserNameWithOverMaxLengthEmailAddressShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();updateInput.EmailAddress = new string('a', StudioXUserBase.MaxEmailAddressLength + 1);

            //Act, Assert
            await Validate(updateInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task UserNameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();updateInput.UserName = null;

            //Act, Assert
            await Validate(updateInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task UserNameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.UserName = string.Empty;

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task UserNameWithOverMaxLengthShouldCauseValidationFailure()
        {
            //Arrange
            var updateInput = GetDto();
            updateInput.UserName = new string('a', StudioXUserBase.MaxUserNameLength + 1);

            //Act, Assert
            await Validate(updateInput).ShouldThrowAsync<ShouldAssertException>();
        }
    }
}