using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using StudioX.Collections.Extensions;
using StudioX.Extensions;
using StudioX.Runtime.Caching;
using StudioX.UI;
using StudioX.Web.Models;
using StudioX.WebApi.Controllers;

namespace StudioX.WebApi.Runtime.Caching
{
    [DontWrapResult]
    public class StudioXCacheController : StudioXApiController
    {
        private readonly ICacheManager cacheManager;

        public StudioXCacheController(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        [HttpPost]
        public async Task<AjaxResponse> Clear(ClearCacheModel model)
        {
            if (model.Password.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Password can not be null or empty!");
            }

            if (model.Caches.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Caches can not be null or empty!");
            }

            await CheckPassword(model.Password);

            var caches = cacheManager.GetAllCaches().Where(c => model.Caches.Contains(c.Name));
            foreach (var cache in caches)
            {
                await cache.ClearAsync();
            }

            return new AjaxResponse();
        }

        [HttpPost]
        [Route("api/StudioXCache/ClearAll")]
        public async Task<AjaxResponse> ClearAll(ClearAllCacheModel model)
        {
            if (model.Password.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Password can not be null or empty!");
            }

            await CheckPassword(model.Password);

            var caches = cacheManager.GetAllCaches();
            foreach (var cache in caches)
            {
                await cache.ClearAsync();
            }

            return new AjaxResponse();
        }

        private async Task CheckPassword(string password)
        {
            var actualPassword = await SettingManager.GetSettingValueAsync(ClearCacheSettingNames.Password);
            if (actualPassword != password)
            {
                throw new UserFriendlyException("Password is not correct!");
            }
        }
    }
}
