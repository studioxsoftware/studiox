using System;
using Microsoft.EntityFrameworkCore;
using StudioX.Domain.Uow;
using StudioX.MultiTenancy;

namespace StudioX.EntityFrameworkCore.Uow
{
    /// <summary>
    ///     Extension methods for UnitOfWork.
    /// </summary>
    public static class UnitOfWorkExtensions
    {
        /// <summary>
        ///     Gets a DbContext as a part of active unit of work.
        ///     This method can be called when current unit of work is an <see cref="EfCoreUnitOfWork" />.
        /// </summary>
        /// <typeparam name="TDbContext">Type of the DbContext</typeparam>
        /// <param name="unitOfWork">Current (active) unit of work</param>
        public static TDbContext GetDbContext<TDbContext>(this IActiveUnitOfWork unitOfWork)
            where TDbContext : DbContext
        {
            return GetDbContext<TDbContext>(unitOfWork, null);
        }

        public static TDbContext GetDbContext<TDbContext>(this IActiveUnitOfWork unitOfWork,
            MultiTenancySides? multiTenancySide)
            where TDbContext : DbContext
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            if (!(unitOfWork is EfCoreUnitOfWork))
            {
                throw new ArgumentException("unitOfWork is not type of " + typeof(EfCoreUnitOfWork).FullName,
                    "unitOfWork");
            }

            return (unitOfWork as EfCoreUnitOfWork).GetOrCreateDbContext<TDbContext>(multiTenancySide);
        }
    }
}