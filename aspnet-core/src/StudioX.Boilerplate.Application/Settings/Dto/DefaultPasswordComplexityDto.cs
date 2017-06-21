namespace StudioX.Boilerplate.Settings.Dto
{
    public class DefaultPasswordComplexityDto
    {
        public virtual int RequiredLength { get; set; }

        public virtual bool RequireUppercase { get; set; }

        public virtual bool RequireLowercase { get; set; }

        public virtual bool RequireDigit { get; set; }

        public virtual bool RequireNonAlphanumeric { get; set; }
    }
}