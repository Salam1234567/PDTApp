using System;
using System.IO;

namespace pdtapp.PrecipitationDataImporter
{
    /// <summary>
    /// Precipitation Folder Importer imports to the database (all prcipitation files found in the passed in folder) 
    /// precipitation file data using a precipitation file importer
    /// </summary>
    public class PrecipitationFolderImporter : IPrecipitationFolderImporter
    {
        private IPrecipitationFileImporter _precipitationFileImporter;
        private string _filenameParttern = "*.pre";

        public PrecipitationFolderImporter(
            IPrecipitationFileImporter precipitationFileImporter)
        {
            _precipitationFileImporter = precipitationFileImporter;
        }

        /// <summary>
        /// Import all the precipitation files data (*.pre) found in the passed in file folder
        /// </summary>
        /// <param name="precipitationFileFolder"></param>
        public void ImportFolder(string precipitationFileFolder)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine($"Precipitation data import for folder ({precipitationFileFolder}) started.");

                //Get Precipitation files
                if (Directory.Exists(precipitationFileFolder))
                {

                    var files = Directory.GetFiles(precipitationFileFolder, _filenameParttern);

                    //For each precipitation file import file using the prcipitation file importer
                    foreach (var file in files)
                    {
                        Console.WriteLine($"Started import processing of Precipitation file ({file}).");

                        if (_precipitationFileImporter.ImportFile(file))
                        {
                            Console.WriteLine($"Precipitation file ({file}) imported successfully.");
                        }
                        else
                        {
                            Console.WriteLine($"Precipitation file ({file}) import falied.");
                        }
                    }
                }

                Console.WriteLine($"Precipitation data import for folder ({precipitationFileFolder}) completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Precipitation file folder import failed for folder ({precipitationFileFolder}). Exception ({ex.Message})");
            }
        }
    }
}
