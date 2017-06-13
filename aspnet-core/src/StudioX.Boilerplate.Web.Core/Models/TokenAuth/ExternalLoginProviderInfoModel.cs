using StudioX.AutoMapper;
using StudioX.Boilerplate.Authentication.External;

namespace StudioX.Boilerplate.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
