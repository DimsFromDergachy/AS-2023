FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT="Production"

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
RUN curl --silent --location https://deb.nodesource.com/setup_20.x | bash -
RUN apt-get install --yes nodejs

FROM build AS build-node
WORKDIR /src
COPY ["AS-2023.csproj", "."]
RUN dotnet restore "AS-2023.csproj"

FROM build-node AS publish
COPY . .
RUN dotnet publish "AS-2023.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AS-2023.dll"]
