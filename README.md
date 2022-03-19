# Official repo of the School Account Sync project

## Setup

1. build the docker file
2. run the docker file with the following connection strings set by environment variables
   - LocalDatabase - the env variable for local PostgreSQL server (create the table if not exists yet with the db_init.sql file)
   - LibraryDatabase
   - CopiersDatabase
   - EntranceDatabase
   - BakalariDatabase
3. that's it, you're ready to go!
