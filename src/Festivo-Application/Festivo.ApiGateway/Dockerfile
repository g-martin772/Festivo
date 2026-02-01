FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Festivo.ApiGateway/Festivo.ApiGateway.csproj", "Festivo.ApiGateway/"]
RUN dotnet restore "Festivo.ApiGateway/Festivo.ApiGateway.csproj"
COPY . .
WORKDIR "/src/Festivo.ApiGateway"
RUN dotnet build "./Festivo.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Festivo.ApiGateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Generate self-signed certificate for development HTTPS
RUN mkdir -p /app/https && \
    dotnet dev-certs https -ep /app/https/aspnetapp.pfx -p festivo123 && \
    chmod 644 /app/https/aspnetapp.pfx

FROM base AS final
USER root
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/https ./https
RUN chown -R $APP_UID:$APP_UID /app/https

USER $APP_UID
ENTRYPOINT ["dotnet", "Festivo.ApiGateway.dll"]
