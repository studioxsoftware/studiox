using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;

namespace StudioX.Configuration
{
    /// <summary>
    /// Implements <see cref="ISettingStore"/>.
    /// </summary>
    public class SettingStore : ISettingStore, ITransientDependency
    {
        private readonly IRepository<Setting, long> settingRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SettingStore(
            IRepository<Setting, long> settingRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            this.settingRepository = settingRepository;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual async Task<List<SettingInfo>> GetAllListAsync(int? tenantId, long? userId)
        {
            /* Combined SetTenantId and DisableFilter for backward compatibility.
             * SetTenantId switches database (for tenant) if needed.
             * DisableFilter and Where condition ensures to work even if tenantId is null for single db approach.
             */
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                using (unitOfWorkManager.Current.DisableFilter(StudioXDataFilters.MayHaveTenant))
                {
                    return
                        (await settingRepository.GetAllListAsync(s => s.UserId == userId && s.TenantId == tenantId))
                        .Select(s => s.ToSettingInfo())
                        .ToList();
                }
            }
        }

        [UnitOfWork]
        public virtual async Task<SettingInfo> GetSettingOrNullAsync(int? tenantId, long? userId, string name)
        {
            using (unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                using (unitOfWorkManager.Current.DisableFilter(StudioXDataFilters.MayHaveTenant))
                {
                    return (await settingRepository.FirstOrDefaultAsync(s => s.UserId == userId && s.Name == name && s.TenantId == tenantId))
                    .ToSettingInfo();
                }
            }
        }

        [UnitOfWork]
        public virtual async Task DeleteAsync(SettingInfo settingInfo)
        {
            using (unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
            {
                using (unitOfWorkManager.Current.DisableFilter(StudioXDataFilters.MayHaveTenant))
                {
                    await settingRepository.DeleteAsync(
                    s => s.UserId == settingInfo.UserId && s.Name == settingInfo.Name && s.TenantId == settingInfo.TenantId
                    );
                    await unitOfWorkManager.Current.SaveChangesAsync();
                }
            }
        }

        [UnitOfWork]
        public virtual async Task CreateAsync(SettingInfo settingInfo)
        {
            using (unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
            {
                using (unitOfWorkManager.Current.DisableFilter(StudioXDataFilters.MayHaveTenant))
                {
                    await settingRepository.InsertAsync(settingInfo.ToSetting());
                    await unitOfWorkManager.Current.SaveChangesAsync();
                }
            }
        }

        [UnitOfWork]
        public virtual async Task UpdateAsync(SettingInfo settingInfo)
        {
            using (unitOfWorkManager.Current.SetTenantId(settingInfo.TenantId))
            {
                using (unitOfWorkManager.Current.DisableFilter(StudioXDataFilters.MayHaveTenant))
                {
                    var setting = await settingRepository.FirstOrDefaultAsync(
                        s => s.TenantId == settingInfo.TenantId &&
                             s.UserId == settingInfo.UserId &&
                             s.Name == settingInfo.Name
                        );

                    if (setting != null)
                    {
                        setting.Value = settingInfo.Value;
                    }

                    await unitOfWorkManager.Current.SaveChangesAsync();
                }
            }
        }
    }
}