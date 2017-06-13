using System;
using StudioX.Domain.Repositories;
using StudioX.EntityFrameworkCore.Tests.Domain;

namespace StudioX.EntityFrameworkCore.Tests.Ef
{
    public interface IPostRepository : IRepository<Post, Guid>
    {
    }
}