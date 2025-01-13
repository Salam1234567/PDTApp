using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using pdtapp.DatabaseContext;
using pdtapp.Entities;

namespace pdtapp.Repositories
{
    /// <summary>
    /// Teh Precipitation repository provide services to interact with the Precipitation database
    /// </summary>
    public class PrecipitationRepository : IPrecipitationRepository
    {
        private readonly IPrecipitationDbContext _precipitationDbContext;

        public PrecipitationRepository(IPrecipitationDbContext dbContext)
        {
            _precipitationDbContext = dbContext;
        }
        /// <summary>
        /// Add a list of Percipitation entities to database
        /// </summary>
        /// <param name="precipitations"></param>
        /// <returns></returns>
        public async Task<int> AddPrecipitations(List<Precipitation> precipitations)
        {
            _precipitationDbContext.Precipitations.AddRange(precipitations);
            return await Task.FromResult(_precipitationDbContext.SaveChanges());
        }

        /// <summary>
        /// Retrieve all Precipitation rows from database Precipitation table
        /// </summary>
        /// <returns></returns>
        public async Task<List<Precipitation>> GetPrecipitations()
        {
            var precipitations = await _precipitationDbContext.Precipitations.ToListAsync();
            return precipitations;
        }

        /// <summary>
        /// Update a list of Percipitation entities in database
        /// </summary>
        /// <param name="precipitations"></param>
        /// <returns></returns>
        public async Task<int> UpdatePrecipitations(List<Precipitation> precipitations)
        {
            _precipitationDbContext.Precipitations.AttachRange(precipitations);
            precipitations.ForEach(p => _precipitationDbContext.Entry(p).State = EntityState.Modified);
            return await Task.FromResult(_precipitationDbContext.SaveChanges());
        }
    }
}
