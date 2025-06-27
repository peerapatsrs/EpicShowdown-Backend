# EpicShowdown Backend

Backend service for EpicShowdown application using .NET 9 with both REST API and gRPC endpoints.

## Features

- REST API (Port 8080)
- gRPC (Port 8101)
- PostgreSQL Database
- Redis Cache
- JWT Authentication
- Swagger Documentation (Development only)

## Prerequisites

- .NET 9 SDK
- PostgreSQL
- Redis

## Configuration

Update the connection strings in `appsettings.json` and `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=epicshowdown;Username=postgres;Password=your_password",
    "Redis": "localhost:6379"
  }
}
```

## Running the Application

1. Development mode:

```bash
dotnet run --environment Development
```

2. Production mode:

```bash
dotnet run --environment Production
```

## API Documentation

- REST API: http://localhost:8080/swagger
- gRPC: http://localhost:8101

## Project Structure

```
EpicShowdown.Api/
├── Controllers/     # REST API Controllers
├── Data/           # Database Context and Migrations
├── Models/         # Domain Models
├── Services/       # Business Logic
├── Repositories/   # Data Access Layer
├── Interfaces/     # Service Interfaces
├── Extensions/     # Extension Methods
├── Middlewares/    # Custom Middlewares
└── Protos/         # gRPC Protocol Buffers
```
