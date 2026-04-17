# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["PaymentService/PaymentService.csproj", "PaymentService/"]
COPY ["BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/"]

RUN dotnet restore "PaymentService/PaymentService.csproj"

# Copy only required folders (IMPORTANT)
COPY PaymentService/. ./PaymentService/
COPY BuildingBlocks/. ./BuildingBlocks/

WORKDIR /src/PaymentService
RUN dotnet publish "PaymentService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PaymentService.dll"]