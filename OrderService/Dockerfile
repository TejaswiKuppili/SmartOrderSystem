# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["OrderService/OrderService.csproj", "OrderService/"]
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]

RUN dotnet restore "OrderService/OrderService.csproj"

# Copy everything
COPY . .

WORKDIR /src/OrderService
RUN dotnet publish "OrderService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderService.dll"]