using pdtapp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pdtapp.PrecipitationFileReader
{
    /// <summary>
    /// File reader provides services to read precipitation file and transform them into GridItem objects.
    /// </summary>
    public class PrecipitationFileReader : IPrecipitationFileReader
    {
        private FileStream _filestream;
        private StreamReader _streamReader;
        private string _filename;
        private string[] Headers { get; set; }

        private long LineNumber { get; set; }
        private int HeaderLinesCount { get; set; }

        private readonly string _years = "Years";
        private int StartYear { get; set; } = 0;
        private int EndYear { get; set; } = 0;

        private bool CanRead => _filestream != null && _streamReader != null && !_streamReader.EndOfStream;

        public PrecipitationFileReader()
        {

        }

        /// <summary>
        /// Initialises the file reader
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="headerLinesCount"></param>
        public void Initialise(string filename, int headerLinesCount)
        {
            Dispose();
            try
            {
                //Setup file stream
                _filename = filename;
                _filestream = File.OpenRead(_filename);
                _streamReader = new StreamReader(_filestream);

                //Setup Headers
                HeaderLinesCount = headerLinesCount;
                Headers = new string[HeaderLinesCount];
                ReadHeaders();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to initialise prcipitation file reader for {filename}. Exception ({ex.Message})");
                Dispose();
            }


        }

        /// <summary>
        /// Read File headers and setup the header values
        /// </summary>
        public void ReadHeaders()
        {
            if (_streamReader == null) return;

            //Read all the header lines as per the configured count
            while (LineNumber < HeaderLinesCount)
            {
                Headers[LineNumber] = ReadLine();
            }

            //Setup the required header values
            SetHeaderValues();
        }

        /// <summary>
        /// Set required File headers
        /// </summary>
        private void SetHeaderValues()
        {
            if (Headers?.Length == 5)
            {
                //For the current processing the Start and End Year is required for the reading of data
                //Setup the Strat and End Year
                var headerLine = Headers[4];
                var lineParts = headerLine.Replace("[", "").Replace("]", ";").Split(";").ToList();
                lineParts.ForEach(s =>
                 {
                     var sparts = s.Trim().Split("=");
                     if (sparts[0].Equals(_years)) // if the header line contains the start and end year
                     {
                         //Found the Start and End Year Entry
                         var startEndYear = sparts[1].Split("-");

                         //Set the values
                         StartYear = int.Parse(startEndYear[0].Trim());
                         EndYear = int.Parse(startEndYear[1].Trim());
                     }

                     //Other setup should be added here

                 });
            }

        }

        /// <summary>
        /// Read the next available Grid Items from the file
        /// </summary>
        /// <returns></returns>
        public List<GridItem> GetNextGridItems()
        {
            //Read the Grid Ref Line
            var gridRefLine = ReadLine();
            if (gridRefLine == null)
            {
                return null;
            }

            //From the read line set the X and Y Ref values
            var xyRefs = gridRefLine.Trim().Split("=")[1].Trim().Split(",");
            var xref = int.Parse(xyRefs[0]);
            var yref = int.Parse(xyRefs[1]);

            //Setup a list to hold the gird items
            var gridItems = new List<GridItem>();

            //Read the Grid Lines Items
            //For each year in the configured year range  add the year items one item per month
            for (var currentYear = StartYear; currentYear <= EndYear; currentYear++)
            {
                var valuesLine = ReadLine();

                //Start with month Jan (1)
                var month = 1;

                var yearItems = valuesLine.Trim().Split(" ")
                    .Where(value => !string.IsNullOrEmpty(value))
                    .Select(value => new GridItem(xref, yref, currentYear, month++, int.Parse(value))).ToList();

                //Add the Year Items
                gridItems.AddRange(yearItems);
            }

            return gridItems;
        }

        /// <summary>
        /// Read the next available line in the file
        /// </summary>
        /// <returns></returns>
        private string ReadLine()
        {
            if (!CanRead)
                return null;

            var line = _streamReader.ReadLine();
            LineNumber++;

            return line;
        }

        /// <summary>
        /// Dispose the file reader
        /// </summary>
        public void Dispose()
        {
            _streamReader?.Dispose();
            _filestream?.Dispose();

            LineNumber = 0;
            _streamReader = null;
            _filestream = null;

            HeaderLinesCount = 0;
            Headers = null;

            StartYear = 0;
            EndYear = 0;
        }

    }
}
