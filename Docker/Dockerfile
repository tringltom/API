FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["EquitesSolution/API.csproj", "."]
COPY ["Infrastructure/Infrastructure.csproj", "../Infrastructure/"]
COPY ["Application/Application.csproj", "../Application/"]
COPY ["DAL/DAL.csproj", "../DAL/"]
COPY ["Persistence/Persistence.csproj", "../Persistence/"]
COPY ["Domain/Domain.csproj", "../Domain/"]
RUN dotnet restore "./API.csproj"
COPY . .
WORKDIR "/src/."
COPY ["./EquitesSolution/appsettings.Development.json", "."]
COPY ["./EquitesSolution/NLog.config", "."]
RUN dotnet build "API.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]