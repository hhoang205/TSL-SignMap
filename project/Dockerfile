# ====== STAGE 1: BUILD ======
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file .csproj và restore dependencies
COPY WebAppTrafficSign.csproj ./
RUN dotnet restore "WebAppTrafficSign.csproj"

# Copy toàn bộ mã nguồn và build
COPY . .
RUN dotnet publish "WebAppTrafficSign.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ====== STAGE 2: RUNTIME ======
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Cho phép truy cập từ ngoài container
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WebAppTrafficSign.dll"]
