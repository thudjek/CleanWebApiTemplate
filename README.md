# Clean .NET Web API Solution

This is .NET Web API solution template with Clean Architecture design which includes out of the box authentication features using .NET Identity Framework.

Solution is by clean arthitecture design divided into:


* Domain Layer
* Application Layer
* Infrastructure Layer
* REST API (Presentation) Layer

Docker compose is included to run containers. Containers are running API itself, local database (SQL Server or PostreSQL) and SEQ for logging.

## Getting started


Prerequisites: .NET 7 SDK or higher and Docker installed.

Run `dotnet new install TH.CleanWebApiTemplate` to install the solution template.

### Create new project with Visual Studio 2022


* Open Visual Studio and in "Create New Project" window search for ".NET Clean Web API" (should be marked as "new" if template was just installed)
* Give your project a name and make sure "Place solution and project in the same directory" is checked
* In next window select database you want to use (SQL Server or PostgreSQL)
    - Template conditionaly pulls code, nuget packages and configuration based on your choice

### Create new project with dotnet CLI


* In folder in which you want to create your project run `dotnet run webapi-cl`
    - add `-o "{ProjectName}"` parameter to name your project
    - add `--database` or `-db` parameter with either "SQL Server" or "PostgreSQL" value to chose database (SQL Server is default if no database parameter is provided)

### Setup before running
After project is created you can modify code, environment variables, configuration etc. to your needs, but this setup is just to get default solution up and running.


* .env file with some default environment variables is provided, make sure you first uncomment variables for chosen database (this is so database docker container can run)
	- if you will be using .env file for storing settings like secrets and keys, you'll wanna put .env file in .gitignore
* Set "Infrastructure" project as startup project and open package console manager and in console manager also set "Infrastructure" as deafult project
* In console manager run `add-migration "{NameOfFirstMigration}" -args "{DatabaseConnectionString}}"` to create first migration (models are picked up from EntityFramework and Identity configurations)
    - first migration is needed so database container can pick it up and run
    - migrations are created in "Infrastructure/Migrations" folder
* Set "docker-compose" project as startup project

Now you can run project which should show swagger page when it starts.

## Troubleshooting


Sometimes some errors might occurr which prevent api/containers to start. It can be because of missing first migration, environment variables beeing incorrect, docker caching or any other reason.

I found that most of the time some (or most likely all) of the next things help:
* delete and restart docker containers
* close project and delete .vs, bin and obj folders
* delete docker volumes used by this project and allow them to be recreated