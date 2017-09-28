using StudioX.Domain.Entities.Auditing;
using Microsoft.AspNet.Identity;

namespace StudioX.Authorization.Users
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public abstract class StudioXUser<TUser> : StudioXUserBase, IUser<long>, IFullAudited<TUser>
        where TUser : StudioXUser<TUser>
    {
        public virtual TUser DeleterUser { get; set; }

        public virtual TUser CreatorUser { get; set; }

        public virtual TUser LastModifierUser { get; set; }
    }
}