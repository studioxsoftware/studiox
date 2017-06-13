namespace StudioX.Web.Models.StudioXUserConfiguration
{
    public class StudioXMultiTenancyConfigDto
    {
        public bool IsEnabled { get; set; }

        public StudioXMultiTenancySidesConfigDto Sides { get; private set; }

        public StudioXMultiTenancyConfigDto()
        {
            Sides = new StudioXMultiTenancySidesConfigDto();
        }
    }
}