using System.Collections.Generic;
using pdtapp.Models;

namespace pdtapp.Services
{
    public interface IPrecipitationService
    {
        int SaveGridItems(List<GridItem> gridItems);
    }
}