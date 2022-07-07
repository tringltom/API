FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

ARG TokenKey_arg
ARG Cloudinary__0__CloudName_arg
ARG Cloudinary__0__APIKey_arg
ARG Cloudinary__0__APISecret_arg
ARG EmailSettings__0__Sender_arg
ARG EmailSettings__0__Password_arg
ARG EmailSettings__0__Server_arg
ARG EmailSettings__0__Port_arg
ARG AllowedHosts_arg
ARG ASPNETCORE_ENVIRONMENT_arg

ENV TokenKey=$TokenKey_arg
ENV Cloudinary:CloudName=$Cloudinary__0__CloudName_arg
ENV Cloudinary:APIKey=$Cloudinary__0__APIKey_arg
ENV Cloudinary:APISecret=$Cloudinary__0__APISecret_arg
ENV EmailSettings:Sender=$EmailSettings__0__Sender_arg
ENV EmailSettings:Password=$EmailSettings__0__Password_arg
ENV EmailSettings:Server=$EmailSettings__0__Server_arg
ENV EmailSettings:Port=$EmailSettings__0__Port_arg
ENV AllowedHosts=$AllowedHosts_arg
ENV ASPNETCORE_ENVIRONMENT=ASPNETCORE_ENVIRONMENT_arg

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["./EquitesSolution/API.csproj", "."]
COPY ["./Infrastructure/Infrastructure.csproj", "../Infrastructure/"]
COPY ["./Application/Application.csproj", "../Application/"]
COPY ["./DAL/DAL.csproj", "../DAL/"]
COPY ["./Persistence/Persistence.csproj", "../Persistence/"]
COPY ["./Domain/Domain.csproj", "../Domain/"]
RUN dotnet restore "./API.csproj"
COPY . .
WORKDIR "/src/."
COPY ["./EquitesSolution/appsettings.${ASPNETCORE_ENVIRONMENT}.json", "."]
COPY ["./EquitesSolution/NLog.config", "."]
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]