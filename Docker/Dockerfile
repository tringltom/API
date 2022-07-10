FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ARG TokenKey_arg
ARG Cloudinary_CloudName_arg
ARG Cloudinary_APIKey_arg
ARG Cloudinary_APISecret_arg
ARG EmailSettings_Sender_arg
ARG EmailSettings_Password_arg
ARG EmailSettings_Server_arg
ARG EmailSettings_Port_arg
ARG AllowedHosts_arg
ARG Environment_arg
ARG EnvJson="./EquitesSolution/appsettings.${Environment_arg}.json"

ENV TokenKey=$TokenKey_arg
ENV Cloudinary:CloudName=$Cloudinary_CloudName_arg
ENV Cloudinary:APIKey=$Cloudinary_APIKey_arg
ENV Cloudinary:APISecret=$Cloudinary_APISecret_arg
ENV EmailSettings:Sender=$EmailSettings_Sender_arg
ENV EmailSettings:Password=$EmailSettings_Password_arg
ENV EmailSettings:Server=$EmailSettings_Server_arg
ENV EmailSettings:Port=$EmailSettings_Port_arg
ENV AllowedHosts=$AllowedHosts_arg
ENV ASPNETCORE_ENVIRONMENT=$Environment_arg

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
RUN true
COPY $EnvJson .
RUN true
COPY ["./EquitesSolution/NLog.config", "."]
RUN true
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]

RUN curl -ks 'https://cert.host.server/ssl_certs/EnterpriseRootCA.crt' -o '/usr/local/share/ca-certificates/EnterpriseRootCA.crt'
RUN /usr/sbin/update-ca-certificates