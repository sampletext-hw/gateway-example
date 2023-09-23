FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["weather-api/weather-api.csproj", "weather-api/"]
RUN dotnet restore "weather-api/weather-api.csproj"
COPY . .
WORKDIR "/src/weather-api"
RUN dotnet build "weather-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "weather-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "weather-api.dll"]
