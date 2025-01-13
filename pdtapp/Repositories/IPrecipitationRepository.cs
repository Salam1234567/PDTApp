using System.Collections.Generic;
using System.Threading.Tasks;
using pdtapp.Entities;

namespace pdtapp.Repositories
{
    public interface IPrecipitationRepository
    {
        Task<List<Precipitation>> GetPrecipitations();
        Task<int> AddPrecipitations(List<Precipitation> precipitations);
        Task<int> UpdatePrecipitations(List<Precipitation> precipitations);
    }
}
