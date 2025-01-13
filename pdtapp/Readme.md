# Local Deployment

To deploy the application on the local host machine, a Dokcer Desktop should \ be installed and running in Linux container mode. Open a cmd command window with administrator privileges. Navigate to the C:\JBA\JBAPDT8L folder and perform the following steps.

Perform the following to run the application:

- Copy the following command to the cmd console window and execute the command.

```
C:\SC\JBA\JBAPDT8L>docker-compose -f "C:/SC/JBA/JBAPDT8L/docker-compose.yaml"  up --force-recreate --detach
```

This should run the application and produces an output to indicate the successful compilation and running of the application. Similar to the following output

```
C:\SC\JBA\JBAPDT8L>docker-compose -f "C:/SC/JBA/JBAPDT8L/docker-compose.yaml"  up --force-recreate --detach
time="2025-01-12T18:12:46Z" level=warning msg="C:\\SC\\JBA\\JBAPDT8L\\docker-compose.yaml: `version` is obsolete"
[+] Building 117.2s (18/18) FINISHED                                                                                                                                                              docker:default
 => [pdtapp internal] load build definition from pdtapp-dockerfile                                                                                                                                          0.6s
 => => transferring dockerfile: 1.45kB                                                                                                                                                                      0.0s
 => [pdtapp internal] load metadata for mcr.microsoft.com/dotnet/sdk:8.0                                                                                                                                    1.6s
 => [pdtapp internal] load metadata for mcr.microsoft.com/dotnet/runtime:8.0                                                                                                                                0.0s
 => [pdtapp internal] load .dockerignore                                                                                                                                                                    0.3s
 => => transferring context: 464B                                                                                                                                                                           0.0s
 => [pdtapp build 1/7] FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:f25e4f51fa06e3b14af1a1135013a3e96055b76caa0e76afc0096d64a77879fd                                                                        0.0s
 => [pdtapp base 1/2] FROM mcr.microsoft.com/dotnet/runtime:8.0                                                                                                                                             0.0s
 => [pdtapp internal] load build context                                                                                                                                                                    0.7s
 => => transferring context: 52.73kB                                                                                                                                                                        0.3s
 => CACHED [pdtapp build 2/7] WORKDIR /src                                                                                                                                                                  0.0s
 => [pdtapp build 3/7] COPY [pdtapp/pdtapp.csproj, pdtapp/]                                                                                                                                                 3.2s
 => [pdtapp build 4/7] RUN dotnet restore "./pdtapp/pdtapp.csproj"   --interactive                                                                                                                         60.9s
 => [pdtapp build 5/7] COPY . .                                                                                                                                                                             4.4s
 => [pdtapp build 6/7] WORKDIR /src/pdtapp                                                                                                                                                                  2.6s
 => [pdtapp build 7/7] RUN dotnet build "./pdtapp.csproj" -c %BUILD_CONFIGURATION% -o /app/build                                                                                                           16.5s
 => [pdtapp publish 1/1] RUN dotnet publish "./pdtapp.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false                                                                                 11.8s
 => CACHED [pdtapp base 2/2] WORKDIR /app                                                                                                                                                                   0.0s
 => CACHED [pdtapp final 1/2] WORKDIR /app                                                                                                                                                                  0.0s
 => [pdtapp final 2/2] COPY --from=publish /app/publish .                                                                                                                                                   4.6s
 => [pdtapp] exporting to image                                                                                                                                                                             3.1s
 => => exporting layers                                                                                                                                                                                     2.1s
 => => writing image sha256:dc5f52c151316a16028ed59df8777086c8ac5646075a9c68b3b634afdd595c35                                                                                                                0.1s
 => => naming to docker.io/library/jbapdt8l-pdtapp                                                                                                                                                          0.2s
[+] Running 3/3
```

- Open the Docker Desktop Console and confirm the appliation is running. This should

# Development

## [1] Install MSSQL Server 2019 on Microsoft Windows 10 Enterprise Edition (Development Phase)

In order to run the application an MSSQL Server database need to be setup.
The database is located on the local host folder C:\JBA. The MSSQL container need to mount this folder (C:\JBA) to (/JBADatabase). This is important for the operation of the database from the remote application (pdtapp).

Docker Desktop must be swithed to Linux containers. This is available from docker context menu and then selecting Switch to Linux Containers if not already selected.

From a command window (CMD) with administrator privileges Perform the following steps to create an MSSQL container with name "sql_2019_1436"

- [1] Run the following command to download the mssql server:2019-latest image

```
docker pull mcr.microsoft.com/mssql/server:2019-latest
```

- [2] Run the following command to create the mssql container with name "sql_209_1436"

```
docker run --name sql_2019_1436 -v "C:\JBA:/JBADatabase:rw" -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MSSQLServerPassword#123" -e "MSSQL_PID=Enterprise" -p 1436:1433 -d mcr.microsoft.com/mssql/server:2019-latest

```

- [3] Run the following command to ensure the container is running and can be connected to.

```
docker exec -it sql_2019_1436 /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C

```

This will prompt for a Password. Please type in the MSSQLServerPassword#123

```
Password: MSSQLServerPassword#123
```

Once successful the container shell will be entered. Type exit to exit

```
1> exit
```

- [4] Check the Docker Desktop and confirm the container with name sql_2019_1436 is up and running.

- [5] To connect to the sql_2019_1436 from MSSQL Management studio perform the following steps:

  - Run MSS Management studio
  - From the Object Explorer view click connect
  - On the dialogue box select \
    Server type: Datebase Engine \
    Server: localhost,1436 \
    Authentication: SQL Server Authentication \
    Login: sa \
    Password: MSSQLServerPassword#123 \
    Click Connect

## [2] The .Net 8 Precipitation Data Transofrm Application (Development Phase)

The Precipitation Data Transform application is developed with .NET 8. It suppoert Docker Container for Linux containers. Also, it can be run withouth the container support.

The application requires a container running an MSSQL Server instance and configured as in [1] above.

To run the application open the application in visual studion 2022 with administration privileges.

Compile the application in the non container support and ensure it compile successfully. The application can be run in debug mode to import the JBA data located in the folder C:\JBAData

Initially the MSSQL server will run without the "Precipitation" database attached. The application will ensure a connection can be made to the server first. Then a check for the "Precipitation" database with files located at the local host "C:/JBA" folder is made. When the database not found an attempt to attach the database to the MSSQL server is made.

On successful setup of the MSSQL server database, the application will import the precipitation data from all the files (\*.pre) located at the local host folder "C:/JBAData".

To Build the docker image from command window run the following command:

```
docker build -f "C:\SC\JBA\JBAPDT8L\pdtapp\Dockerfile" --force-rm -t pdtapp  "C:\SC\JBA\JBAPDT8L"
```

Find the generated IMAGE ID for the built image (pdtapp) by running the following command:

```
docker images
```

To Run the pdtapp container run the following command using the IMAGE ID from the above sterp:

```
docker run --name pdtapp --rm -v "C:\JBAData:/JBAData:rw"  -d IMAGE-ID
```

For example:

```
docker run --name pdtapp --rm -d -v "C:\JBAData:/JBAData:rw"  5c3339cdf589
```
