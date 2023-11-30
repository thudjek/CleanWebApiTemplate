# Clean .NET Web API Solution

This is .NET Web API solution template with Clean Architecture design which includes out of the box authentication features using .NET Identity Framework.

Solution is by clean arthitecture design divided into:


* Domain Layer
* Application Layer
* Infrastructure Layer
* REST API (Presentation) Layer

Docker compose is included to run containers. Containers are running API itself, local database (SQL Server or PostreSQL) and SEQ for logging.

## Getting started


Prerequisites: .NET 8 SDK or higher and Docker installed (you can still install this template with lower version of .NET but you have to manually edit .csproj files and downgrade specific packages that are installed by default)

Run `dotnet new install TH.CleanWebApiTemplate` to install the solution template.

### Create new project with Visual Studio 2022


* Open Visual Studio and in "Create New Project" window search for ".NET Clean Web API" (should be marked as "new" if template was just installed)
* Give your project a name and make sure "Place solution and project in the same directory" is checked
* In next window select database you want to use (SQL Server or PostgreSQL)
    - Template conditionaly pulls code, nuget packages and configuration based on your choice

### Create new project with dotnet CLI


* In folder in which you want to create your project run `dotnet run webapi-cl`
    - add `-o "{ProjectName}"` parameter to name your project
    - add `--database` or `-db` parameter with either "SQL Server" or "PostgreSQL" value to choose database (SQL Server is default if no database parameter is provided)


Projects/Namespaces in solution will be named based on name you entered (through Visual Studio or through -o parameter in CLI). Project names are of format {NameYouEntered}.Domain, {NameYouEntered}.Application etc.

### Setup before running
After project is created you can modify code, environment variables, configuration etc. to your needs, but this setup is just to get default solution up and running.


* Add first migration to the project by opening package manager console and running `add-migration "{NameOfFirstMigration}" -Project {ProjectName}.Infrastructure -StartupProject {ProjectName}.Infrastructure -args "{DatabaseConnectionString}"` 
    - first migration is needed so database container can pick it up and run
    - migrations are created in "Infrastructure/Migrations" folder
* Set "docker-compose" project as startup project

Now you can run project using docker-compose and it should open swagger page when it starts.


NOTE: .env file with some default environment variables/settings is provided with the template, if you plan to use it for storing sensitive information like secrets, api keys etc. for your local environment, you'll wanna put .env file in .gitignore

## Troubleshooting


Sometimes some errors might occurr which prevent api/containers to start. It can be because of missing first migration, environment variables beeing incorrect, docker caching or any other reason.

I found that most of the time some (or most likely all) of the following things help:
* delete and restart docker containers
* close solution and delete .vs, bin and obj folders
* delete docker volumes used by this project and allow them to be recreated