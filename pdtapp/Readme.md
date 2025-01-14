# Local Deployment

To deploy the application on the local host machine, a Dokcer Desktop should \ be installed and running in Linux container mode. Open a cmd command window with administrator privileges. Navigate to the C:\JBA\JBAPDT8L folder and perform the following steps.

Perform the following to run the application:

- Copy the following command to the cmd console window and execute the command.

```
C:\SC\JBA\JBAPDT8L>docker-compose -f "C:/SC/JBA/JBAPDT8L/docker-compose.yaml"  up --force-recreate --detach
```

This should run the application and produces an output to indicate the successful compilation and running of the application. Similar to the following output

```
CC:\SC\JBA\JBAPDT8L>docker-compose -f "C:/SC/JBA/JBAPDT8L/docker-compose.yaml"  up --force-recreate --detach
time="2025-01-11T14:00:39Z" level=warning msg="C:\\SC\\JBA\\JBAPDT8L\\docker-compose.yaml: `version` is obsolete"
2025/01/11 14:00:39 http2: server: error reading preface from client //./pipe/docker_engine: file has already been closed
[+] Building 0.0s (0/0)  docker:default
[+] Building 42.2s (18/18) FINISHED                                                                                           docker:default
 => [pdtapp internal] load build definition from PDTAppDockerfile                                                                       1.0s
 => => transferring dockerfile: 1.45kB                                                                                                  0.0s
 => [pdtapp internal] load metadata for mcr.microsoft.com/dotnet/sdk:8.0                                                                1.7s
 => [pdtapp internal] load metadata for mcr.microsoft.com/dotnet/runtime:8.0                                                            0.0s
 => [pdtapp internal] load .dockerignore                                                                                                1.0s
 => => transferring context: 464B                                                                                                       0.0s
 => [pdtapp build 1/7] FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:f25e4f51fa06e3b14af1a1135013a3e96055b76caa0e76afc0096d64a77879fd    0.0s
 => [pdtapp internal] load build context                                                                                                0.6s
 => => transferring context: 2.52kB                                                                                                     0.0s
 => [pdtapp base 1/2] FROM mcr.microsoft.com/dotnet/runtime:8.0                                                                         0.0s
 => CACHED [pdtapp build 2/7] WORKDIR /src                                                                                              0.0s
 => CACHED [pdtapp build 3/7] COPY [pdtapp/pdtapp.csproj, pdtapp/]                                                                      0.0s
 => CACHED [pdtapp build 4/7] RUN dotnet restore "./pdtapp/pdtapp.csproj"   --interactive                                               0.0s
 => [pdtapp build 5/7] COPY . .                                                                                                         3.7s
 => [pdtapp build 6/7] WORKDIR /src/pdtapp                                                                                              3.3s
 => [pdtapp build 7/7] RUN dotnet build "./pdtapp.csproj" -c %BUILD_CONFIGURATION% -o /app/build                                       13.7s
 => [pdtapp publish 1/1] RUN dotnet publish "./pdtapp.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false             11.1s
 => CACHED [pdtapp base 2/2] WORKDIR /app                                                                                               0.0s
 => CACHED [pdtapp final 1/2] WORKDIR /app                                                                                              0.0s
 => CACHED [pdtapp final 2/2] COPY --from=publish /app/publish .                                                                        0.0s
 => [pdtapp] exporting to image                                                                                                         1.0s
 => => exporting layers                                                                                                                 0.0s
 => => writing image sha256:d75ec83dbe3251665a2469cacfaf3ddfeea766c9415d65794690cb26d85bfd14                                            0.3s
 => => naming to docker.io/library/jbapdt8l-pdtapp                                                                                      0.3s
[+] Running 3/3
 ✔ Network jbapdt8l_pdtconnection  Created                                                                                              2.3s
 ✔ Container sql_2019_1436         Started                                                                                             12.1s
 ✔ Container pdtapp                Started                                                                                             14.4s
```

- Open the Docker Desktop Console and confirm the appliation is running. This should show the composed solution with a pdtapp and sal_2019_1436 containers running. Navigate to the Logs view of each container to see the logged messages.

- To shut down the solution application run the following command from the cmd consle window:

```
docker-compose -f "C:/SC/JBA/JBAPDT8L/docker-compose.yaml"  down
```

The output of performing the above command should be similar to the following:

```
C:\SC\JBA\JBAPDT8L>docker-compose -f "C:/SC/JBA/JBAPDT8L/docker-compose.yaml"  down
time="2025-01-11T14:10:58Z" level=warning msg="C:\\SC\\JBA\\JBAPDT8L\\docker-compose.yaml: `version` is obsolete"
[+] Running 3/3
 ✔ Container pdtapp                Removed                                                                                             11.1s
 ✔ Container sql_2019_1436         Removed                                                                                             20.1s
 ✔ Network jbapdt8l_pdtconnection  Removed

```

To build the application from the command window, perfrom the follwoing Docker command:

```
docker compose -f "C:/SC/JBA/JBAPDT8L/docker-compose.yaml" --project-directory "C:/SC/JBA/JBAPDT8L" build
```

The output show be similar to the following:

```
C:\SC\JBA\JBAPDT8L>docker compose -f "C:/SC/JBA/JBAPDT8L/docker-compose.yaml" --project-directory "C:/SC/JBA/JBAPDT8L" build
time="2025-01-12T21:28:22Z" level=warning msg="C:\\SC\\JBA\\JBAPDT8L\\docker-compose.yaml: `version` is obsolete"
[+] Building 0.0s (0/0)  docker:default
[+] Building 57.0s (18/18) FINISHED                                                                                                                          docker:default
 => [pdtapp internal] load build definition from pdtapp-dockerfile                                                                                                     0.4s
 => => transferring dockerfile: 1.45kB                                                                                                                                 0.0s
 => [pdtapp internal] load metadata for mcr.microsoft.com/dotnet/sdk:8.0                                                                                               1.2s
 => [pdtapp internal] load metadata for mcr.microsoft.com/dotnet/runtime:8.0                                                                                           0.0s
 => [pdtapp internal] load .dockerignore                                                                                                                               0.9s
 => => transferring context: 464B                                                                                                                                      0.0s
 => [pdtapp build 1/7] FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:f25e4f51fa06e3b14af1a1135013a3e96055b76caa0e76afc0096d64a77879fd                                   0.0s
 => [pdtapp base 1/2] FROM mcr.microsoft.com/dotnet/runtime:8.0                                                                                                        0.0s
 => [pdtapp internal] load build context                                                                                                                               0.4s
 => => transferring context: 26.48kB                                                                                                                                   0.1s
 => CACHED [pdtapp build 2/7] WORKDIR /src                                                                                                                             0.0s
 => CACHED [pdtapp build 3/7] COPY [pdtapp/pdtapp.csproj, pdtapp/]                                                                                                     0.0s
 => CACHED [pdtapp build 4/7] RUN dotnet restore "./pdtapp/pdtapp.csproj"   --interactive                                                                              0.0s
 => [pdtapp build 5/7] COPY . .                                                                                                                                        2.7s
 => [pdtapp build 6/7] WORKDIR /src/pdtapp                                                                                                                             2.9s
 => [pdtapp build 7/7] RUN dotnet build "./pdtapp.csproj" -c %BUILD_CONFIGURATION% -o /app/build                                                                      15.8s
 => [pdtapp publish 1/1] RUN dotnet publish "./pdtapp.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false                                             8.8s
 => CACHED [pdtapp base 2/2] WORKDIR /app                                                                                                                              0.0s
 => CACHED [pdtapp final 1/2] WORKDIR /app                                                                                                                             0.0s
 => [pdtapp final 2/2] COPY --from=publish /app/publish .                                                                                                              3.9s
 => [pdtapp] exporting to image                                                                                                                                        5.1s
 => => exporting layers                                                                                                                                                3.5s
 => => writing image sha256:49fd33d2b240fed22a57b6b2c2b3e12f6f3bb94f082ea637a31dceb7934259c1                                                                           0.2s
 => => naming to docker.io/library/jbapdt8l-pdtapp
```

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
