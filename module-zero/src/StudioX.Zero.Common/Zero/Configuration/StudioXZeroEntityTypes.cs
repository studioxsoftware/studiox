using System;
using System.Reflection;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.MultiTenancy;

namespace StudioX.Zero.Configuration
{
    public class StudioXZeroEntityTypes : IStudioXZeroEntityTypes
    {
        public Type User
        {
            get => user;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof (StudioXUserBase).IsAssignableFrom(value))
                {
                    throw new StudioXException(value.AssemblyQualifiedName + " should be derived from " + typeof(StudioXUserBase).AssemblyQualifiedName);
                }

                user = value;
            }
        }
        private Type user;

        public Type Role
        {
            get => role;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(StudioXRoleBase).IsAssignableFrom(value))
                {
                    throw new StudioXException(value.AssemblyQualifiedName + " should be derived from " + typeof(StudioXRoleBase).AssemblyQualifiedName);
                }

                role = value;
            }
        }
        private Type role;

        public Type Tenant
        {
            get => tenant;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(StudioXTenantBase).IsAssignableFrom(value))
                {
                    throw new StudioXException(value.AssemblyQualifiedName + " should be derived from " + typeof(StudioXTenantBase).AssemblyQualifiedName);
                }

                tenant = value;
            }
        }
        private Type tenant;
    }
}