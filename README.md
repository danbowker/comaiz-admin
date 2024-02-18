[![Board Status](https://danbowker.visualstudio.com/5df69691-7dcf-4009-8166-6e69c4715f85/4057cef7-a02c-4d79-b5f5-028c3e1549ae/_apis/work/boardbadge/72e7a0b8-d5d2-4129-8d9b-2d7e2b9ae9bb)](https://danbowker.visualstudio.com/5df69691-7dcf-4009-8166-6e69c4715f85/_boards/board/t/4057cef7-a02c-4d79-b5f5-028c3e1549ae/Microsoft.RequirementCategory)

# comaiz-admin

An application for managing a small consultancy business.

## Installation

Currently, check it out and build it.

It uses [Entity Framework](https://learn.microsoft.com/en-us/ef/) with [PostgresSQL](https://www.postgresql.org/). 

To get a development database up:
1. [Install PostgresSQL](https://www.postgresql.org/download/)
1. ....

This is designed to work with CockroachDB for production (or any PostgresSQL as a service provider). To work with CockroadDB:
1. Get an account (or ask me for mine, colleague)
1. Create a database called "comaiz"
2. Create a user-secret in the "comaiz" project with:
```
dotnet user-secrets add CockroachDB <connection string>
```

Once you have a database, configure it by running the following command from terminal in the solution folder:
```
dotnet ef database update -p comaiz.data -s comaiz
```

## Usage

There's a web page

## Contributing
