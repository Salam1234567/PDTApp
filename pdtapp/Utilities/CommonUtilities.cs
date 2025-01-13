using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using pdtapp.DatabaseContext;
using pdtapp.PrecipitationDataImporter;
using pdtapp.PrecipitationFileReader;
using pdtapp.Repositories;
using pdtapp.Services;
using System;
using System.Data;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace pdtapp.Utilities
{
    /// <summary>
    /// Provide common utilites
    /// </summary>
    public static class CommonUtilities
    {
        //The DatabaseFilePath refers to the folder location on the SQL Server where the JBA Precipitation database is located.
        //This folder should be mapped to a folder on the local host where the SQL Server is running. For example C:\JBA. The
        // database file 'Precipitation.mdf' and the associated database log file should be present there.
        public static readonly string DatabaseFilePath = "/JBADatabase/Precipitation.mdf";
        public static readonly string DatabaseName = "Precipitation";

        //The Precipitation data folder
        //This container expects the Precipitation files to be found on the folder /JBAData.
        //This folder should be mapped to the local host, for example C:\JBAData
        public static readonly string PrecipitationFileFolder = @"/JBAData";


        public static readonly string DbConnection = "DefaultConnection";

        // The appsetting.json file contains the DefaultConnection SQL connection string
        // For development time uncomment the localhost connection string and the comment out the "sql-2019_1436"
        // connection string to enable debugging and working with the containerised SQL Server on the local host.
        // The server should be running.
        public static readonly string ConfigurationFile = "appsettings.json";

        /// <summary>
        /// Setup the dependency injection services container
        /// </summary>
        /// <param name="args"></param>
        /// <param name="dbConnection"></param>
        /// <param name="configurationFile"></param>
        /// <returns></returns>
        public static IServiceProvider SetupDependencyInectionContainer(
            string[] args,
            string dbConnection = "DefaultConnection",
            string configurationFile = "appsettings.json")
        {
            //Build configurations
            var configuration = new ConfigurationBuilder()
                  .AddJsonFile(configurationFile, optional: true, reloadOnChange: true)
                  .AddEnvironmentVariables()
                  .AddCommandLine(args)
                  .Build();

            //Build services collection
            IServiceCollection serviceCollection = new ServiceCollection();

            //Add configuration 
            serviceCollection.AddTransient<IConfiguration>(sp => configuration);

            //Add Precipitation database context
            serviceCollection.AddDbContext<IPrecipitationDbContext, PrecipitationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString(dbConnection);
                Console.WriteLine($"SQL Database Connection String: ({connectionString})");
                options.UseSqlServer(connectionString, op => op.EnableRetryOnFailure());
            });

            //Add Precipitation repository
            serviceCollection.AddTransient<IPrecipitationRepository, PrecipitationRepository>();

            //Add Precipitation service
            serviceCollection.AddTransient<IPrecipitationService, PrecipitationService>();

            //Add Precipitaion file reader
            serviceCollection.AddTransient<IPrecipitationFileReader, PrecipitationFileReader.PrecipitationFileReader>();

            //Add Precipitation File Importer
            serviceCollection.AddTransient<IPrecipitationFileImporter, PrecipitationFileImporter>();

            //Add Precipitation Folder Importer
            serviceCollection.AddTransient<IPrecipitationFolderImporter, PrecipitationFolderImporter>();

            //Build service provider and return
            return serviceCollection.BuildServiceProvider();
        }

        /// <summary>
        /// This is a folder watch utility. It watches an import folder for files with a certain filename pattern. 
        /// Once files found in the folder it process them using a folder importer utility.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="importfolder"></param>
        /// <param name="filePattern"></param>
        /// <param name="waitPeriod"></param>
        /// <returns></returns>
        public static async Task ImportPrecipitationFiles(IServiceProvider serviceProvider,
            string importfolder, string filePattern = "*.pre", int waitPeriod = 20000)
        {
            await Task.Run(() =>
            {
                var processdata = true;
                Console.WriteLine($"Precipitation Files Import Started.");
                do
                {
                    try
                    {
                        var filesToImportCount = Directory.GetFiles(importfolder, filePattern, SearchOption.TopDirectoryOnly).Length;
                        if (filesToImportCount > 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine($" {filesToImportCount} Precipitation file(s) has been found in " +
                                $"the configured import folder ({importfolder}).");

                            Console.WriteLine($"Precipitation Files Import Processing Started.");

                            using (var scope = serviceProvider.CreateScope())
                            {
                                var percipitationDataImporter = scope.ServiceProvider.GetRequiredService<IPrecipitationFolderImporter>();
                                percipitationDataImporter.ImportFolder(importfolder);
                            }

                            Console.WriteLine($" {filesToImportCount} Precipitation file(s) has been processed.");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine($"No Precipitation Files found in import folder ({importfolder}).");
                            Console.WriteLine($"Wait for ({GetSeconds(waitPeriod)}) seconds before trying ...");
                            Thread.Sleep(waitPeriod);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Exception occurred while importing precipitation file into database. Exception ({ex.Message}).");
                        Console.WriteLine("Processing will continue ...");
                    }

                } while (processdata);
            });
        }

        /// <summary>
        /// Perform an SQL Server connection. 
        /// If not successful, wait a period of time and try again until a successful connection is established.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="connectionString"></param>
        /// <param name="waitPeriod"></param>
        /// <returns></returns>
        public static async Task<bool> EstablishSqlServerConnection(
            IServiceProvider serviceProvider,
            string connectionString, int waitPeriod = 5000)
        {
            var connectionSuccess = await Task.Run(() =>
            {
                var success = true;
                var tryConnect = false;
                var tryCcount = 0;

                do
                {
                    tryCcount++;

                    try
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Attempt to connect to SQL Server. Try count ({tryCcount})");
                        success = ConnectToSQLServer(connectionString);

                        if (!success)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"Failed to connect to SQL Server ({connectionString}).");
                            Console.WriteLine($"Retry to connect will be attempted in {GetSeconds(waitPeriod)} seconds.");
                            Thread.Sleep(waitPeriod);

                        }
                        else
                        {
                            Console.WriteLine($"SQL Server connection succeeded ({connectionString}).");
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        Console.WriteLine($"Exception ocurred while trying to establish " +
                            $"a connection to the configured SQL Server ({connectionString}). Exception ({ex.Message})");
                    }

                    tryConnect = !success;

                } while (tryConnect);

                return success;
            });

            return connectionSuccess;
        }


        /// <summary>
        /// Connect to a SQL Server using the passed in connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static bool ConnectToSQLServer(string connectionString)
        {
            var success = false;

            Console.WriteLine();
            Console.WriteLine($"Try to connect to SQL server: ({connectionString})");

            using (SqlConnection sqlDatabaseConnection = new SqlConnection(connectionString))
            {
                try
                {
                    sqlDatabaseConnection.Open();
                    success = true;

                    Console.WriteLine();
                    Console.WriteLine("Connection to SQL Server Succeeded.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Connection to SQL Server Failed.");
                    Console.WriteLine(ex.Message);
                }
            }

            return success;
        }

        /// <summary>
        /// Logs to the console the files with a given filename pattern found in a folder
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="filenamePattern"></param>
        public static void LogFilesFoundInFolder(string folder, string filenamePattern = "*.pre")
        {
            Console.WriteLine();
            Console.WriteLine($"Folder {folder} contains the following files: ");

            try
            {
                var files = Directory.GetFiles(folder, filenamePattern);
                if (files.Length > 0)
                {

                    foreach (var item in files)
                    {
                        Console.WriteLine($"{item}");
                    }
                }
                else
                {
                    Console.WriteLine("No files found in folder");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Log import files failed. Exception ({ex.Message})");
            }
        }

        /// <summary>
        /// Attempt to connect to the configured database for a number of times. 
        /// If not available, an attempt to attach the database is performed.
        /// If negative number of try passed in, it will try to connect indefinetly
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="dbConnection"></param>
        /// <param name="trycount"></param>
        /// <param name="waitPeriod"></param>
        /// <returns></returns>
        public static async Task<bool> DatabaseAvailable(IServiceProvider serviceProvider,
            string dbConnection,
            string databaseName = "Precipitation",
            string databaseFilePath = "/JBADatabase/Precipitation.mdf",
            int trycount = 5,
            int waitPeriod = 20000)
        {
            var connectionSuccess = await Task.Run(() =>
            {
                var databaseAvailable = false;

                for (int connectTry = 0; (connectTry < trycount || trycount < 0) && !databaseAvailable; connectTry++)
                {

                    try
                    {
                        Console.WriteLine();
                        Console.WriteLine($"Try connect to database. {connectTry + 1}");
                        databaseAvailable = CanConnectToDatabase(serviceProvider);

                        if (!databaseAvailable)
                        {

                            var sqlServerConnectionString =GetSqlServerConnectionString(serviceProvider);
                            var databaseConnectionString = GetDatabaseConnectionString(serviceProvider, dbConnection);

                            Console.WriteLine($"Could not access configured database ({databaseConnectionString})");

                            Console.WriteLine($"Attach database ({databaseConnectionString}) started.");
                            databaseAvailable = AttachDatebase(sqlServerConnectionString, databaseConnectionString, databaseName, databaseFilePath);

                            if (!databaseAvailable)
                            {
                                Console.WriteLine($"Attch database failed ({databaseConnectionString}).");
                            }
                            else
                            {
                                Console.WriteLine($"Connection to database ({databaseConnectionString}) succeeded.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Database connection succeeded");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception occurred while trying to connect to database. Exception ({ex.Message})");
                    }

                    if (!databaseAvailable)
                    {
                        if (connectTry != trycount || trycount < 0)
                        {
                            Console.WriteLine($"Try to establish connection will be attempted in ({GetSeconds(waitPeriod)}) seconds");
                            Thread.Sleep(waitPeriod);
                        }
                    }
                }

                return databaseAvailable;
            });

            return connectionSuccess;
        }

        /// <summary>
        /// Determine if a connection to the configured database can be made
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static bool CanConnectToDatabase(IServiceProvider serviceProvider)
        {
            var success = true;
            try
            {

                using (var scope = serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<IPrecipitationDbContext>();

                    if (dbContext.Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator dbaseCreater && !dbaseCreater.Exists())
                    {
                        Console.WriteLine($"Failed to connect to the configured database.");
                        success = false;
                    }
                    else
                    {
                        Console.WriteLine("Connection the configured database succeeded.");
                    }

                    dbContext.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine();
                Console.WriteLine($"Connection to dataqbase failed. Exception ({ex.Message})");
            }

            return success;
        }


        /// <summary>
        /// Attach databe to SQL Server
        /// </summary>
        /// <param name="databaseConnectionString"></param>
        /// <param name="databaseName"></param>
        /// <param name="databaseFilePath"></param>
        /// <returns></returns>
        public static bool AttachDatebase(
            string sqlServerConnectionString,
            string databaseConnectionString,
            string databaseName = "Precipitation",
            string databaseFilePath = "/JBADatabase/Precipitation.mdf")
        {
            var success = true;
            using (SqlConnection sqlDatabaseConnection = new SqlConnection(sqlServerConnectionString))
            {
                try
                {
                    sqlDatabaseConnection.Open();
                    SqlCommand cmd = new SqlCommand("sp_attach_db");
                    cmd.Connection = sqlDatabaseConnection;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dbname", $"{databaseName.Trim()}");
                    cmd.Parameters.AddWithValue("@filename1", @$"{databaseFilePath.Trim()}");
                    
                    cmd.ExecuteNonQuery();

                    Console.WriteLine($"Database attached successfully.");
                }
                catch (Exception ex)
                {
                    success = false;
                    Console.WriteLine($"Failed to attach database 'Precipitation.mdf'. Exception {ex.Message}");
                }
            }

            return success;
        }

        /// <summary>
        /// Retrieve SQL Connection String form database connection string
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="databasename"></param>
        /// <returns></returns>
        public static string GetSqlServerConnectionString(IServiceProvider serviceProvider,
            string dbConnection = "DefaultConnection", string databasename = "Precipitation")
        {
            var removeString = $"Database={databasename};";

            var configuration = serviceProvider.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString(dbConnection);
            connectionString = connectionString.Replace(removeString, " ");

            return connectionString;

        }

        /// <summary>
        /// Return the configured database connection string
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="dbConnection"></param>
        /// <returns></returns>
        public static string GetDatabaseConnectionString(IServiceProvider serviceProvider,
            string dbConnection = "DefaultConnection")
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var connectionString = configuration.GetConnectionString(dbConnection);

            return connectionString;

        }

        /// <summary>
        /// Host Ping Utility
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static string PingSqlServer(string hostname = "sql_2019_1436", int trycount = 3, int timeout = 1000)
        {
            string ipAddress = string.Empty;
            var ping = new Ping();

            for (int count = 0; count < trycount; count++)
            {
                Console.WriteLine();
                Console.WriteLine($"Try ({count + 1}) ping {hostname}");
                try
                {

                    PingReply pingreply = ping.Send(hostname, timeout*3);
                    if (pingreply.Status == IPStatus.Success)
                    {
                        Console.WriteLine("Address: {0}", pingreply.Address);
                        Console.WriteLine("status: {0}", pingreply.Status);
                        Console.WriteLine("Round trip time: {0}", pingreply.RoundtripTime);
                        Console.WriteLine();

                        ipAddress = pingreply.Address.ToString();
                        break;
                    }

                }
                catch (PingException ex)
                {
                    Console.WriteLine($"Ping host {hostname} failed. Exception ({ex.Message}).");
                    Console.WriteLine();

                    Thread.Sleep(timeout * 5);
                }
            }

            return ipAddress;
        }

        /// <summary>
        /// Return seconds for a given milliseconds
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static int GetSeconds(int milliseconds)
        {
            return milliseconds / 1000;
        }

        /// <summary>
        /// Performs database migration used in development phase.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static void MigrateDatabase(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<IPrecipitationDbContext>();

                if (dbContext.Database.GetService<IDatabaseCreator>() is RelationalDatabaseCreator dbaseCreater && !dbaseCreater.Exists())
                {
                    try
                    {
                        Console.WriteLine("Database Migration Started");
                        dbContext.Database.Migrate();
                        Console.WriteLine("Database Migration Completed!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Migration Failed!. Exception ({e.Message})");
                    }

                }

                dbContext.Database.CloseConnection();
            }
        }

        /// <summary>
        /// Development time use only
        /// Check if a development Database migration is requested. 
        /// If requested an attempts to perform the migration is commenced
        /// </summary>
        /// <param name="args"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public static async Task<bool> MigrationRequested(
            string[] args,
            IServiceProvider serviceProvider,
            string migrationFlag = "MIGRATEDATABASE")
        {
            var migrationRequested = await Task.Run(() =>
            {
                var migrateDatebaseRequested = false;
                try
                {
                    var migratedatebaseFlag = string.Empty;
                    if (args.Length > 0)
                    {
                        migratedatebaseFlag = args[0];
                    }

                    if (migratedatebaseFlag.ToUpper().Equals(migrationFlag))
                    {
                        migrateDatebaseRequested = true;
                        MigrateDatabase(serviceProvider);
                    }

                    if (migrateDatebaseRequested)
                    {
                        Console.WriteLine("Datebase Migration has been performed");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred while performing Datebase Migration. Exception {ex.Message}");
                }


                return migrateDatebaseRequested;
            });

            return migrationRequested;
        }
    }
}
