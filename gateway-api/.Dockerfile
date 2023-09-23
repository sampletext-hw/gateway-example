FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["gateway-api/gateway-api.csproj", "gateway-api/"]
RUN dotnet restore "gateway-api/gateway-api.csproj"
COPY . .
WORKDIR "/src/gateway-api"
RUN dotnet build "gateway-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "gateway-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gateway-api.dll"]
