namespace StudioX.Boilerplate.Settings.Dto
{
    public class UserManagementDto
    {
        public virtual bool IsEmailConfirmationRequiredForLogin { get; set; }

        public virtual bool IsNewRegisteredUserActiveByDefault { get; set; }
    }
}