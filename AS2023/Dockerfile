FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT="Production"

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
RUN bash -E $(curl -fsSL https://deb.nodesource.com/setup_18.x | bash - ); apt install -y nodejs

FROM build AS build-node
WORKDIR /src
COPY ["AS2023.csproj", "."]
RUN dotnet restore "AS2023.csproj"

FROM build AS publish
COPY . .
RUN dotnet publish "AS2023.csproj" -c Release -o /app/build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AS2023.dll"]
