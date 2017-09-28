using System;
using StudioX.Authorization.Users;

namespace StudioX.Runtime.Session
{
    public static class StudioXSessionExtensions
    {
        public static bool IsUser(this IStudioXSession session, StudioXUserBase user)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return session.TenantId == user.TenantId && 
                session.UserId.HasValue && 
                session.UserId.Value == user.Id;
        }
    }
}
