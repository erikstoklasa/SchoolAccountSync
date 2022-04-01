# Official repo of the School Account Sync project

## Setup

1. Build the docker file in the root of the repo

```bash
docker build -f .\SchoolAccountSync\Dockerfile -t schoolaccountsync .
```

2. Create the local PostreSQL server and create a table with the `db_init.sql` file, either with docker or via an installer [here](https://www.postgresql.org/download/)

   - docker command:

   ```bash
   docker run --name SchoolAccountSyncPostgres -e POSTGRES_PASSWORD=<PASSWORD> -d postgres
   ```

   - if you use docker then use this host for the PostreSQL server: `host.docker.internal`

3. Run the docker image with the following connection strings set by environment variables
   - `LocalDatabase=Host=<DB_HOST>;Username=<USERNAME>;Password=<PASSWORD>;Database=<DB_NAME>`
   - `LibraryDatabase=DataSource=<DB_HOST>;Database=<DB_NAME>;user=<USER>;password=<PASSWORD>`
   - `CopiersDatabase=Host=<DB_HOST>;Username=<USER>;Password=<PASSWORD>;Database=<DB_NAME>`
   - `EntranceDatabase=Server=<DB_HOST>;Database=<DB_NAME>;User Id=<USER>;Password=<PASSWORD>`
   - `BakalariDatabase=Server=<DB_HOST>;Database=<DB_NAME>;User Id=<USER>;Password=<PASSWORD>`

```bash
docker run -d -p 8080:80 --name SchoolAccountSync -e "LocalDatabase=<CONNECTIONSTRING_POSTGRESQL>" -e "LibraryDatabase=<CONNECTIONSTRING_FIREBIRD>" -e "BakalariDatabase=<CONNECTIONSTRING_MSSQL>" -e "EntranceDatabase=<CONNECTIONSTRING_MSSQL>" -e "CopiersDatabase=<CONNECTIONSTRING_POSTGRESQL>" schoolaccountsync
```

4. That's it, you're ready to go! It should be live at http://localhost:8080

## Figma prototype

[Here](https://www.figma.com/file/mFCFlVOrO4ITENEnLbXY3G/School-Account-Sync?node-id=0%3A1) is a link to a Figma prototype which I used for designing the mockups of this project. There are also some features which I would like to implement in the future.

## Credits

Copyright (c) 2002-2021, Npgsql
