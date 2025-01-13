using pdtapp.Models;
using System.Collections.Generic;

namespace pdtapp.PrecipitationFileReader
{
    public interface IPrecipitationFileReader
    {
        /// <summary>
        /// Initialises the file reader
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="headerLinesCount"></param>
        public void Initialise(string filename, int headerLinesCount);

        /// <summary>
        /// Get Next available Grid items
        /// </summary>
        /// <returns></returns>
        List<GridItem> GetNextGridItems();

        /// <summary>
        /// Disposes the file readedr
        /// </summary>
        void Dispose();
    }
}