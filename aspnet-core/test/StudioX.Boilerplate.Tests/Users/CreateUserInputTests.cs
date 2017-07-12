using System.Threading.Tasks;
using Shouldly;
using StudioX.Authorization.Users;
using StudioX.Boilerplate.Users.Dto;
using Xunit;

namespace StudioX.Boilerplate.Tests.Users
{
    public class CreateUserInputTests : BoilerplateDtoTestBase<CreateUserInput>
    {
        protected override CreateUserInput GetDto()
        {
            return new CreateUserInput
            {
                EmailAddress = "new.user@volosoft.com",
                IsActive = true,
                FirstName = "New",
                LastName = "User",
                Password = "123qwe",
                UserName = "New.User",
                Roles = new[] {"Admin"}
            };
        }

        [Fact]
        public async Task FirstNameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.FirstName = null;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task FirstNameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.FirstName = string.Empty;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task FirstNameWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.FirstName = new string('a', StudioXUserBase.MaxFirstNameLength + 1);

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task LastNameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.LastName = null;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task LastNameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.LastName = string.Empty;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task LastNameWithOverMaxLengthValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.LastName = new string('a', StudioXUserBase.MaxLastNameLength + 1);

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task EmailAddressWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.EmailAddress = null;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task EmailAddressWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.EmailAddress = string.Empty;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task EmailAddressWithInvalidValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.EmailAddress = "InvalidEmailAddress";

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task UserNameWithOverMaxLengthEmailAddressShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.EmailAddress = new string('a', StudioXUserBase.MaxEmailAddressLength + 1);

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task UserNameWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.UserName = null;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task UserNameWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.UserName = string.Empty;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task UserNameOverMaxLengthShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.UserName = new string('a', StudioXUserBase.MaxUserNameLength + 1);

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task PasswordWithNullValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.Password = null;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }

        [Fact]
        public async Task PasswordWithEmptyValueShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.Password = string.Empty;

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }


        [Fact]
        public async Task PasswordWithOverMaxLengthEmailAddressShouldCauseValidationFailure()
        {
            //Arrange
            var createInput = GetDto();
            createInput.Password = new string('a', StudioXUserBase.MaxPlainPasswordLength + 1);

            //Act, Assert
            await Validate(createInput)
                .ShouldThrowAsync<ShouldAssertException>();
        }
    }
}