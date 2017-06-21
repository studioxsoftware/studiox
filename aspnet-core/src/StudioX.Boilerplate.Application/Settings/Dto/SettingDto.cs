namespace StudioX.Boilerplate.Settings.Dto
{
    public class SettingDto
    {
        public virtual DefaultPasswordComplexityDto DefaultPasswordComplexity { get; set; }

        public virtual PasswordComplexityDto PasswordComplexity { get; set; }

        public virtual UserLockOutDto UserLockOut { get; set; }

        public virtual UserManagementDto UserManagement { get; set; }
    }
}