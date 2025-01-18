[![Board Status](https://danbowker.visualstudio.com/5df69691-7dcf-4009-8166-6e69c4715f85/4057cef7-a02c-4d79-b5f5-028c3e1549ae/_apis/work/boardbadge/72e7a0b8-d5d2-4129-8d9b-2d7e2b9ae9bb)](https://danbowker.visualstudio.com/5df69691-7dcf-4009-8166-6e69c4715f85/_boards/board/t/4057cef7-a02c-4d79-b5f5-028c3e1549ae/Microsoft.RequirementCategory)

# comaiz-admin

An application for managing a small consultancy business.

## Installation

Currently, check it out and build it.

It uses [Entity Framework](https://learn.microsoft.com/en-us/ef/) with [PostgresSQL](https://www.postgresql.org/).

This is designed to work with CockroachDB (or any PostgresSQL as a service provider) or with a local PostgresSQL for development.

To work with CockroadDB set up an accout and get a connection string. For local, install PostgresSQL.

To set up the DB:

1. Create a database. Default name is 'comaiz' but you could override in the connection string.
2. Set a "PostgresSQL" connection string in the "comaiz" project. Recommended approaches:
   + For local development, set the "PostgresSQL" connection string in appSettings.Development.json
   + For testing on production, create a user-secret from the command line in the project with:

        ```text
        dotnet user-secrets set ConnectionStrings:PostgresSQL <connection string>
        ```
    + Or set an environment variable

3. Once you have a database and the connection strings setup up, create tables by running the following command from terminal in the solution folder:

    ```text
    dotnet ef database update -p comaiz.data -s comaiz
    ```

Configure authentication and authorization:

1. Setup oauth2 with a provider such as Google
2. Create appsettings, dotnet user-secrets or environment variables for:
   + Jwt.Authority (EnvVar Jwt__Authority) : e.g. "https://accounts.google.com"
   + Jwt.Audience (EnvVar Jwt__Audience) : e.g. <Your Google oauth2 client ID>

## Usage

To test from Swagger:

1. Run comaiz.api with the Swagger launch settings
2. Obtain a valid id token from, for example the Google oath2 playground, having first set your clientID and clientSecret from your oauth provider
3. On the Swagger page, click the authenticate button, paste in the id token (test on JWT.io)
4. Test one of the API calls

## Contributing
