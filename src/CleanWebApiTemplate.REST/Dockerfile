#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/CleanWebApiTemplate.REST/CleanWebApiTemplate.API.csproj", "src/CleanWebApiTemplate.API/"]
COPY ["src/CleanWebApiTemplate.Application/CleanWebApiTemplate.Application.csproj", "src/CleanWebApiTemplate.Application/"]
COPY ["src/CleanWebApiTemplate.Domain/CleanWebApiTemplate.Domain.csproj", "src/CleanWebApiTemplate.Domain/"]
COPY ["src/CleanWebApiTemplate.Infrastructure/CleanWebApiTemplate.Infrastructure.csproj", "src/CleanWebApiTemplate.Infrastructure/"]
RUN dotnet restore "src/CleanWebApiTemplate.REST/CleanWebApiTemplate.REST.csproj"
COPY . .
WORKDIR "/src/src/CleanWebApiTemplate.REST"
RUN dotnet build "CleanWebApiTemplate.REST.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CleanWebApiTemplate.REST.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CleanWebApiTemplate.REST.dll"]