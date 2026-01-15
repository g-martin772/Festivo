FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Festivo.LoggingService/Festivo.LoggingService.csproj", "Festivo.LoggingService/"]
RUN dotnet restore "Festivo.LoggingService/Festivo.LoggingService.csproj"
COPY . .
WORKDIR "/src/Festivo.LoggingService"
RUN dotnet build "./Festivo.LoggingService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Festivo.LoggingService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Festivo.LoggingService.dll"]
