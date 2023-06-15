# Clean .NET Web API Solution

This is .NET Web API solution template with Clean Architecture design with out of the box authentication features.

Solution is by clean arthitecture design divided into:
<br />

* Domain Layer
* Application Layer
* Infrastructure Layer
* API (Presentation) Layer

Docker compose is included to run containers locally. Containers are running API itself, local database (SQL Server or PostreSQL) and SEQ for logging.

## Getting started
<br />

Prerequisites: .NET 7 SDK or higher and Docker installed.

Run `dotnet new install TH.CleanWebApiSolution` to install the solution template.

### Create new project with Visual Studio 2022
<br />

* Open Visual Studio and in "Create New Project" window search for ".NET Clean Web API" (should be marked as "new" if just installed)
* Give your project a name and make sure "Place solution and project in the same directory" is checked
* In next window select database you want to use (SQL Server or PostgreSQL)
    - Template conditionaly pulls code, nuget packages and configuration based on your choice

### Create new project with dotnet CLI
<br />

* In folder in which you want to create your project run `dotnet run webapi-cl`
    - add `-o "{ProjectName}"` parameter to name your project
    - add `--database` or `-db` parameter with either "SQL Server" or "PostgreSQL" value to chose databse (SQL Server is default if no database parameter is provided)

### Setup before running
After project is created you can modify code, environment variables, configuration etc. to your needs, but this setup is just to get default project up and running.
<br />

* .env file with some default environment variables is provided, make sure you first uncomment variables for chosen database (this is so database docker container can run)
	- if you will be using .env file for storing settings like secrets and keys, you'll wanna put .env file in .gitignore
* Set "Infrastructure" project as startup project and open package console manager and in console manager also set "Infrastructure" as deafult project
* In console manager run `add-migration "{NameOfFirstMigration}" -args "{DatabaseConnectionString}}"` to create first migration (models are picked up from EntityFramework and Identity configurations)
    - first migration is needed so database container can pick it up and run
    - migrations are created in "Infrastructure/Migrations" folder
* Set "docker-compose" project as startup project

Now you can run project which should show swagger page when it starts.

## Troubleshooting
<br />

Sometimes some error occurres which prevent api/containers to start. It can be because of missing first migration, environment variables beeing incorrect, docker caching or any other reason.

I found that most of the time some (or most likely all) of the next things help:
* delete and restart docker containers
* close project and delete .vs, bin and obj folders
* delete docker volumes used by this project and allow them to be recreated