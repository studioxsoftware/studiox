namespace StudioX.Owin
{
    public class StudioXOwinOptions
    {
        /// <summary>
        /// Default: true.
        /// </summary>
        public bool UseEmbeddedFiles { get; set; }

        public StudioXOwinOptions()
        {
            UseEmbeddedFiles = true;
        }
    }
}