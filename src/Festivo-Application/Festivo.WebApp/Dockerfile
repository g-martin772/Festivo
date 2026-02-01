# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Copy csproj and restore dependencies
COPY ["Festivo.WebApp/Festivo.WebApp.csproj", "Festivo.WebApp/"]
RUN dotnet restore "Festivo.WebApp/Festivo.WebApp.csproj"
# Copy everything else and build
COPY . .
WORKDIR "/src/Festivo.WebApp"
RUN dotnet build "Festivo.WebApp.csproj" -c Release -o /app/build
# Publish stage
FROM build AS publish
RUN dotnet publish "Festivo.WebApp.csproj" -c Release -o /app/publish
# Final stage - serve with nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
# Copy published app
COPY --from=publish /app/publish/wwwroot .
# Copy nginx configuration
COPY Festivo.WebApp/nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
