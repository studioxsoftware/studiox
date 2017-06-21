namespace StudioX.Boilerplate.Settings.Dto
{
    public class PasswordComplexityDto
    {
        public virtual bool UseDefaultPasswordComplexitySettings { get; set; }

        public virtual int MaxLength { get; set; }

        public virtual int MinLength { get; set; }

        public virtual int RequiredFrequencyPasswordChange { get; set; }

        public virtual int NumberOfPreviouslyUsedPasswords { get; set; }

        public virtual bool UseUpperCaseLetters { get; set; }

        public virtual bool UseLowerCaseLetters { get; set; }

        public virtual bool UseNumbers { get; set; }

        public virtual bool UsePunctuations { get; set; }
    }
}