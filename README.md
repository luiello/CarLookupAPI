# CarLookup API

[![.NET](https://github.com/luiello/CarLookupAPI/actions/workflows/dotnet.yml/badge.svg)](https://github.com/luiello/CarLookupAPI/actions/workflows/dotnet.yml)

## Overview
CarLookup is a RESTful API for managing and querying car makes and models.

Built with .NET 9, it features robust authentication, role-based authorization, and a clean layered architecture.

The API provides comprehensive endpoints for CRUD operations on car makes and models with proper validation and error handling.

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) 8.0 or higher
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or another IDE of your choice

### Setup Instructions
1. Clone the repository:
```
git clone https://github.com/luiello/CarLookupAPI.git
cd carlookup
```

2. Install and configure MySQL:
   - Install MySQL Server 8.0+
   - Run the setup script: `scripts/setup-mysql.sql`
   - Or create the database manually:
   ```sql
   CREATE DATABASE CarLookupDb CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   CREATE USER 'carlookup_user'@'localhost' IDENTIFIED BY 'CarLookup2024!';
   GRANT ALL PRIVILEGES ON CarLookupDb.* TO 'carlookup_user'@'localhost';
   ```

3. Restore dependencies:
```
dotnet restore
```

4. Configure connection string in `appsettings.json` or using user secrets:
```
dotnet user-secrets set "ConnectionStrings:Default" "Server=localhost;Port=3306;Database=CarLookupDb;Uid=carlookup_user;Pwd=CarLookup2024!;CharSet=utf8mb4;SslMode=None;" --project src/CarLookup.Host
```

5. Create and run migrations:
```
dotnet ef migrations add InitialMySqlMigration --project src/CarLookup.Access --startup-project src/CarLookup.Host
dotnet ef database update --project src/CarLookup.Access --startup-project src/CarLookup.Host
```

6. Run the application:
```
dotnet run --project src/CarLookup.Host/CarLookup.Host.csproj
```

### Database Setup
The database is automatically created and seeded with sample data during development when the application starts. This behavior is controlled by the `Data:SeedOnStartup` setting in your configuration (defaults to `true` in development).

**Important:** The application now uses MySQL instead of SQL Server. Make sure your MySQL server is running and accessible.

## API Endpoints

### Authentication
- `POST /api/v1/auth/token` - Obtain JWT token for authentication

### Car Makes
- `GET /api/v1/carmakes` - List all car makes (paginated)
- `GET /api/v1/carmakes/{id}` - Get car make by ID
- `POST /api/v1/carmakes` - Create a new car make (Editor+ role)
- `PUT /api/v1/carmakes/{id}` - Update a car make (Editor+ role)
- `DELETE /api/v1/carmakes/{id}` - Delete a car make (Admin role)

### Car Models
- `GET /api/v1/carmakes/{makeId}/models` - List all models for a make (paginated)
- `GET /api/v1/carmakes/{makeId}/models/{id}` - Get car model by ID
- `POST /api/v1/carmakes/{makeId}/models` - Create a new car model (Editor+ role)
- `PUT /api/v1/carmakes/{makeId}/models/{id}` - Update a car model (Editor+ role)
- `DELETE /api/v1/carmakes/{makeId}/models/{id}` - Delete a car model (Admin role)

## Authentication & Authorization

The API uses JWT Bearer token authentication. Tokens can be obtained from the `/api/v1/auth/token` endpoint. The system implements role-based authorization with three levels:

- **Admin**: Full access to all endpoints
- **Editor**: Can read all data and create/update makes and models
- **Reader**: Read-only access to all data

## Testing
The project includes testing:
- **Unit Tests**: Fast, isolated component testing
- **Integration Tests**: Database integration with MySQL containers
- **Acceptance Tests**: End-to-end API workflow testing

To run tests:
```
dotnet test
```
### Postman Testing
Postman collections are available in the `/postman` folder:

- **`CarLookupAPI.postman_collection.json`** - Complete API test collection with automated testing scripts
- **`Develop Env.postman_environment.json`** - Development environment configuration (localhost:57484)
  
## Code Style
- Uses modern C# 13 features
- Follows SOLID principles
- Implements clean architecture with clear separation of concerns
- Uses comprehensive exception handling with custom middleware
- Includes XML documentation comments

## Database Migration Notes

### MySQL-Specific Considerations
- **Character Set**: Uses `utf8mb4` for full Unicode support
- **Collation**: Uses `utf8mb4_unicode_ci` for case-insensitive searches
- **GUIDs**: Stored as `char(36)` for optimal performance
- **DateTime**: Uses `datetime(6)` for microsecond precision
- **Boolean**: Uses `tinyint(1)` for MySQL compatibility

### Migration Commands
See `scripts/mysql-migration-commands.ps1` for helpful EF Core commands.

## Planned Improvements
- **Containerization**: Docker support with MySQL containers
- **Real Authentication**: Identity Server or Azure AD integration
- **Caching**: Redis with cache tags and distributed invalidation
- **Database Scaling**: Read replicas and query optimization
- **Monitoring**: Application Performance Monitoring (APM)
- **Soft Deletes**: Audit trail with soft delete implementation

## Architecture

```mermaid
---
config:
  theme: neutral
---
graph TD
    A["CarLookup.Host<br>(API Controllers)"] --> B["CarLookup.Manager<br>(Business Logic)"]
    A --> C["CarLookup.Infrastructure<br>(Cross-cutting concerns)"]
    B --> D["CarLookup.Domain<br>(Domain Models, Interfaces)"]
    B --> E["CarLookup.Contracts<br>(DTOs, Requests, Responses)"]
    C --> D
    B --> F["CarLookup.Access<br>(Data Access)"]
    F --> D
    F --> G["MySQL Database"]
