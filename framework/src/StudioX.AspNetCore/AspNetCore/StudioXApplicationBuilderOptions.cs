namespace StudioX.AspNetCore
{
	public class StudioXApplicationBuilderOptions
	{
		/// <summary>
		/// Default: true.
		/// </summary>
		public bool UseCastleLoggerFactory { get; set; }

		/// <summary>
		/// Default: true.
		/// </summary>
		public bool UseStudioXRequestLocalization { get; set; }

		public StudioXApplicationBuilderOptions()
		{
			UseCastleLoggerFactory = true;
			UseStudioXRequestLocalization = true;
		}
	}
}