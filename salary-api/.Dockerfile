FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["salary-api/salary-api.csproj", "salary-api/"]
RUN dotnet restore "salary-api/salary-api.csproj"
COPY . .
WORKDIR "/src/salary-api"
RUN dotnet build "salary-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "salary-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "salary-api.dll"]
