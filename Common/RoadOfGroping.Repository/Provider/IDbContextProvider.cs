using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Common.DependencyInjection;

namespace RoadOfGroping.Repository.Provider
{
    public interface IDbContextProvider : ITransientDependency
    {
        DbContext GetDbContext();
    }
}