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
            get { return _user; }
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

                _user = value;
            }
        }
        private Type _user;

        public Type Role
        {
            get { return _role; }
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

                _role = value;
            }
        }
        private Type _role;

        public Type Tenant
        {
            get { return _tenant; }
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

                _tenant = value;
            }
        }
        private Type _tenant;
    }
}