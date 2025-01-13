using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using pdtapp.DatabaseContext;
using pdtapp.Utilities;
using System;
using System.Threading;
namespace pdtapp
{
    public class Program : IDesignTimeDbContextFactory<PrecipitationDbContext>
    {
        //The dependency injection service container 
        private static IServiceProvider Container;
        private static int _waitPeriod = 20000;  // 20 seconds

        public static void Main(string[] args)
        {

            Console.WriteLine("Hello! Precipitation Data Transform Application (pdtapp) started.");

            //Setup the service container
            Container =
                CommonUtilities.SetupDependencyInectionContainer(args,
                CommonUtilities.DbConnection,
                CommonUtilities.ConfigurationFile);

            // During development you may pass "localhost" for non container running
            CommonUtilities.PingSqlServer();

            //On start up log any file found in the import folder
            Console.WriteLine();
            Console.WriteLine($"Precipitation Import Folder ({CommonUtilities.PrecipitationFileFolder})");
            CommonUtilities.LogFilesFoundInFolder(CommonUtilities.PrecipitationFileFolder);

            //Ensure SQL Server can be reached
            var connectionString = CommonUtilities.GetSqlServerConnectionString(Container, CommonUtilities.DbConnection);

            if (!CommonUtilities.EstablishSqlServerConnection(Container, connectionString).Result)
            {
                Console.WriteLine("Application could not establish a connection to the SQL Server. Application will terminate.");
                return;
            }

            //Check database available
            if (!CommonUtilities.DatabaseAvailable(Container,
                CommonUtilities.DbConnection,
                CommonUtilities.DatabaseName,
                CommonUtilities.DatabaseFilePath,
                -1).Result)
            {
                Console.WriteLine("Import Database not available on the configured SQL Server. Application will terminate.");
                Console.WriteLine("Wait ...");
                Thread.Sleep(_waitPeriod);
                return;
            }
            else
            {
                //On startup the SQL Server needs to initialise before serving quries.
                //Wait a period of time before importing data

                Console.WriteLine($"Wait for ({CommonUtilities.GetSeconds(_waitPeriod)}) seconds before starting data import ...");
                Thread.Sleep(_waitPeriod);
            }

            CommonUtilities.ImportPrecipitationFiles(Container, CommonUtilities.PrecipitationFileFolder).Wait();

            Console.WriteLine("Precipitation Data Transform application terminating");
        }

        /// <summary>
        /// Required For database migration tools used during development time
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public PrecipitationDbContext CreateDbContext(string[] args)
        {
            //Get configuration
            var configuration = new ConfigurationBuilder()
                  .AddJsonFile(CommonUtilities.ConfigurationFile, optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables()
                  .AddCommandLine(args)
                  .Build();

            //Get option builder
            var optionsBuilder = new DbContextOptionsBuilder<PrecipitationDbContext>()
                .UseSqlServer(configuration.GetConnectionString(CommonUtilities.DbConnection));

            return new PrecipitationDbContext(optionsBuilder.Options);
        }
    }
}
