using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;
using StudioX.ZeroCore.SampleApp.Core;

namespace StudioX.ZeroCore.SampleApp.Application.Users
{
    [AutoMap(typeof(User))]
    public class UserDto : EntityDto<long>
    {
        public string UserName { get; set; }
    }
}