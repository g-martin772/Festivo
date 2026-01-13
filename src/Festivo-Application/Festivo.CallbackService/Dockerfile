FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Festivo.CallbackService/Festivo.CallbackService.csproj", "Festivo.CallbackService/"]
RUN dotnet restore "Festivo.CallbackService/Festivo.CallbackService.csproj"
COPY . .
WORKDIR "/src/Festivo.CallbackService"
RUN dotnet build "./Festivo.CallbackService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Festivo.CallbackService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Festivo.CallbackService.dll"]
