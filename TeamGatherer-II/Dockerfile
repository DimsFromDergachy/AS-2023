#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT="Production"

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-node
RUN curl --silent --location https://deb.nodesource.com/setup_20.x | bash -
RUN apt-get install --yes nodejs

FROM build-node AS build
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Server/TeamGatherer.Server.csproj", "Server/"]
COPY ["Shared/TeamGatherer.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/TeamGatherer.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "TeamGatherer.Server.csproj" -c Release -o /app/build

FROM build AS publish
COPY ["Server/teamgatherer.pfx", "/app/publish/"]
RUN dotnet publish "TeamGatherer.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM build-node AS build-client
WORKDIR /src
COPY ["Client/AS-2023.csproj", "."]
RUN dotnet restore "AS-2023.csproj"

FROM build-client AS publish-client
COPY ["Client", "."]
RUN dotnet publish "AS-2023.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
#COPY --from=publish /app/publish .
COPY --from=publish-client /app/publish client
ENTRYPOINT ["dotnet", "client/AS-2023.dll"]
#ENTRYPOINT ["dotnet", "TeamGatherer.Server.dll"]