using pdtapp.PrecipitationFileReader;
using pdtapp.Services;
using System;
using System.IO;

namespace pdtapp.PrecipitationDataImporter
{
    /// <summary>
    /// Precipitation File Importer provide services to import precipitation files data into the Prcipitation database
    /// It uses a precipitation service and a prcipitation file reader to perform its tasks
    /// </summary>
    public class PrecipitationFileImporter : IPrecipitationFileImporter
    {
        private IPrecipitationFileReader _precipitationFileReader;
        private IPrecipitationService _precipitationService;

        private static string _archiveFolderName = "Processed";
        private int _precipitationHeaderLineCount = 5;

        public PrecipitationFileImporter(
            IPrecipitationFileReader precipitationFileReader,
            IPrecipitationService precipitationService)
        {
            _precipitationFileReader = precipitationFileReader;
            _precipitationService = precipitationService;
        }


        /// <summary>
        ///  Import all the precipitation data found in the passed in file to database
        /// </summary>
        /// <param name="precipitationFile"></param>
        /// <returns></returns>
        public bool ImportFile(string precipitationFile)
        {
            var success = true;
            try
            {
                Console.WriteLine();
                Console.WriteLine($"Precipitation data import for file ({precipitationFile}) started");


                //Initialise the prcipitation file reader
                _precipitationFileReader.Initialise(precipitationFile, _precipitationHeaderLineCount);

                // Featch all the available Grid values in the Precipitation file and add them to the database

                // Get first available data set
                var gridItems = _precipitationFileReader.GetNextGridItems();

                while (gridItems != null && gridItems.Count > 0)
                {
                    _precipitationService.SaveGridItems(gridItems);

                    //Get next available data set
                    gridItems = _precipitationFileReader.GetNextGridItems();
                }

                // Precipitation file data import to database completed.

                //Dispose file reader
                _precipitationFileReader.Dispose();

                //Archive file to processed folder
                ArchiveFile(precipitationFile);

                Console.WriteLine($"Precipitation data import for file ({precipitationFile}) completed.");
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine($"Precipitation file import failed for file ({precipitationFile}). Exception ({ex.Message})");
            }

            return success;
        }

        private static void ArchiveFile(string file)
        {
            try
            {
                var precipitationFileFolder = Path.GetDirectoryName(file);
                var filename = Path.GetFileName(file);

                var archiveFolder = $"{precipitationFileFolder}/{_archiveFolderName}";

                Console.WriteLine($"Archive prcipitation file ({file}) to archive folder ({archiveFolder}) started.");

                if (!Directory.Exists(archiveFolder))
                {
                    Directory.CreateDirectory(archiveFolder);
                }

                var archiveFile = $"{archiveFolder}/{filename}";
                File.Move(file, archiveFile, overwrite: true);

                Console.WriteLine($"Archive prcipitation file ({file}) to archive folder ({archiveFolder}) completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Failed to archive prcipitation file ({file}). Exception ({ex.Message}).");
            }

        }
    }
}
