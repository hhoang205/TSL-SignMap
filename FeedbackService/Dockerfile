# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy SharedLibrary first (if exists)
COPY ["SharedLibrary/SharedLibrary.csproj", "SharedLibrary/"]
RUN dotnet restore "SharedLibrary/SharedLibrary.csproj"

# Copy service csproj and restore dependencies
COPY ["FeedbackService/FeedbackService.csproj", "FeedbackService/"]
RUN dotnet restore "FeedbackService/FeedbackService.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/FeedbackService"
RUN dotnet build "FeedbackService.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "FeedbackService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 5007

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:5007/health || exit 1

# Run the app
ENTRYPOINT ["dotnet", "FeedbackService.dll"]

