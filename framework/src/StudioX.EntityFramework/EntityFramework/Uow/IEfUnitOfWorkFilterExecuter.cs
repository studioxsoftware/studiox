using System.Data.Entity;
using StudioX.Domain.Uow;

namespace StudioX.EntityFramework.Uow
{
    public interface IEfUnitOfWorkFilterExecuter : IUnitOfWorkFilterExecuter
    {
        void ApplyCurrentFilters(IUnitOfWork unitOfWork, DbContext dbContext);
    }
}