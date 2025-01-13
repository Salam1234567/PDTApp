using Microsoft.EntityFrameworkCore;
using pdtapp.Entities;

namespace pdtapp.DatabaseContext
{
    /// <summary>
    /// EF Database context
    /// </summary>
    public class PrecipitationDbContext : DbContext, IPrecipitationDbContext
    {
        /// <summary>
        /// Precipitation database context
        /// </summary>
        /// <param name="options"></param>
        public PrecipitationDbContext(DbContextOptions<PrecipitationDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// Precipitation entites
        /// </summary>
        public DbSet<Precipitation> Precipitations { get; set; }

        /// <summary>
        /// Inovked on Model creation
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
