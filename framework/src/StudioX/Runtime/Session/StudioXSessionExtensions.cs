namespace StudioX.Runtime.Session
{
    /// <summary>
    /// Extension methods for <see cref="IStudioXSession"/>.
    /// </summary>
    public static class StudioXSessionExtensions
    {
        /// <summary>
        /// Gets current User's Id.
        /// Throws <see cref="StudioXException"/> if <see cref="IStudioXSession.UserId"/> is null.
        /// </summary>
        /// <param name="session">Session object.</param>
        /// <returns>Current User's Id.</returns>
        public static long GetUserId(this IStudioXSession session)
        {
            if (!session.UserId.HasValue)
            {
                throw new StudioXException("Session.UserId is null! Probably, user is not logged in.");
            }

            return session.UserId.Value;
        }

        /// <summary>
        /// Gets current Tenant's Id.
        /// Throws <see cref="StudioXException"/> if <see cref="IStudioXSession.TenantId"/> is null.
        /// </summary>
        /// <param name="session">Session object.</param>
        /// <returns>Current Tenant's Id.</returns>
        /// <exception cref="StudioXException"></exception>
        public static int GetTenantId(this IStudioXSession session)
        {
            if (!session.TenantId.HasValue)
            {
                throw new StudioXException("Session.TenantId is null! Possible problems: No user logged in or current logged in user in a host user (TenantId is always null for host users).");
            }

            return session.TenantId.Value;
        }

        /// <summary>
        /// Creates <see cref="UserIdentifier"/> from given session.
        /// Returns null if <see cref="IStudioXSession.UserId"/> is null.
        /// </summary>
        /// <param name="session">The session.</param>
        public static UserIdentifier ToUserIdentifier(this IStudioXSession session)
        {
            return session.UserId == null
                ? null
                : new UserIdentifier(session.TenantId, session.GetUserId());
        }
    }
}