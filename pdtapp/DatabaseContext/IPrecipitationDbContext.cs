using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using pdtapp.Entities;

namespace pdtapp.DatabaseContext
{
    public interface IPrecipitationDbContext
    {
        DbSet<Precipitation> Precipitations { get; set; }
        int SaveChanges();
        DatabaseFacade Database { get; }

        EntityEntry<TEntity> Entry<TEntity>(TEntity precipitation) where TEntity : class;
    }
}