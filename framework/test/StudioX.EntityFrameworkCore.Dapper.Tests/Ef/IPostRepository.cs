using System;

using StudioX.Domain.Repositories;
using StudioX.EntityFrameworkCore.Dapper.Tests.Domain;

namespace StudioX.EntityFrameworkCore.Dapper.Tests.Ef
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
    }
}