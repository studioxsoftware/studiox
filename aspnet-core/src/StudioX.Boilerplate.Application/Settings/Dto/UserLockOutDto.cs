namespace StudioX.Boilerplate.Settings.Dto
{
    public class UserLockOutDto
    {
        public virtual bool IsEnabled { get; set; }

        public virtual int DefaultAccountLockoutSeconds { get; set; }

        public virtual int MaxFailedAccessAttemptsBeforeLockout { get; set; }
    }
}
