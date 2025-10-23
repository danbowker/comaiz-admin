#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project files and restore dependencies
COPY ["comaiz.api/comaiz.api.csproj", "comaiz.api/"]
COPY ["comaiz.data/comaiz.data.csproj", "comaiz.data/"]
RUN dotnet restore "comaiz.api/comaiz.api.csproj"

# Copy the rest of the application code
COPY . .

# Build the project
WORKDIR "/src/comaiz.api"
RUN dotnet build "comaiz.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "comaiz.api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Build frontend
FROM node:20 AS frontend-build
WORKDIR /frontend
COPY frontend/package*.json ./
RUN npm ci
COPY frontend/ ./
RUN npm run build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Copy frontend build to wwwroot
COPY --from=frontend-build /frontend/build ./wwwroot
ENTRYPOINT ["dotnet", "comaiz.api.dll"]