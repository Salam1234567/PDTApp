using pdtapp.Entities;
using pdtapp.Models;
using pdtapp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdtapp.Services
{
    /// <summary>
    /// Precipitation service. Provide service to interact with SQL Precipitation Repository
    /// </summary>
    public class PrecipitationService : IPrecipitationService
    {
        private readonly IPrecipitationRepository _percipitationRepository;

        public PrecipitationService(IPrecipitationRepository percipitationRepository)
        {
            _percipitationRepository = percipitationRepository;
        }

        /// <summary>
        /// Saves GridItem data to the Precipitation database
        /// It trransofrm the GridItem objects  to a Precipitation entites before saving.
        /// </summary>
        /// <param name="gridItems"></param>
        /// <returns></returns>
        public int SaveGridItems(List<GridItem> gridItems)
        {
            //Transofrm GridItem into Precipitation Entity
            var precipitations = gridItems.Select(item =>
                                    new Precipitation
                                    {
                                        Xref = item.Xref,
                                        Yref = item.Yref,
                                        Value = item.Value,
                                        Date = new DateTime(item.Year, item.Month, item.Day, 0, 0, 0, DateTimeKind.Utc)
                                    }).ToList();

            //Add Precipitation to database
            var count = _percipitationRepository.AddPrecipitations(precipitations).Result;
            return count;
        }
    }
}
