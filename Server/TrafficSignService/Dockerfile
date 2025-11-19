# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary first (if exists)
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]
RUN dotnet restore "SharedLibrary/SharedLibrary.csproj"

# Copy service csproj and restore dependencies
COPY ["Server/TrafficSignService/TrafficSignService.csproj", "Server/TrafficSignService/"]
RUN dotnet restore "Server/TrafficSignService/TrafficSignService.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Server/TrafficSignService"
RUN dotnet build "TrafficSignService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TrafficSignService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage - Using SDK for development (allows EF migrations)
# For production, consider using aspnet:8.0 and running migrations separately
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app

# Install EF Core tools (matching runtime version)
RUN dotnet tool install --global dotnet-ef --version 9.0.9

# Add .NET tools to PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy published app
COPY --from=publish /app/publish .

# Copy source code for migrations (needed for dotnet ef commands)
WORKDIR /src
COPY --from=build /src/Server/TrafficSignService /src/Server/TrafficSignService
COPY --from=build /src/SharedLibrary /src/SharedLibrary
WORKDIR /app

# Expose port
EXPOSE 5002

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:5002/health || exit 1

# Run the app
ENTRYPOINT ["dotnet", "TrafficSignService.dll"]

