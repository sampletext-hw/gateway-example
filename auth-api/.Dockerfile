FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["auth-api/auth-api.csproj", "auth-api/"]
RUN dotnet restore "auth-api/auth-api.csproj"
COPY . .
WORKDIR "/src/auth-api"
RUN dotnet build "auth-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "auth-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "auth-api.dll"]
