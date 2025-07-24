# OnionWebApi

UnionWebApi is a modular, layered web API project built with **.NET 9** and C# 13, designed for scalable, maintainable, and modern web service development. The project leverages best practices such as dependency injection, modular service registration, and OpenAPI/Swagger documentation.

## Technologies Used

- **.NET 9** and **C# 13**: Modern, high-performance platform for web API development.
- **ASP.NET Core Web API**: Framework for building RESTful services.
- **Entity Framework Core**: Object-relational mapping (ORM) for data access.
- **AutoMapper**: Automatic mapping between DTOs and entities.
- **CAP (EventBus)**: Distributed transaction and event bus integration (with SQL Server and RabbitMQ).
- **RabbitMQ**: Message queue infrastructure.
- **Scalar / OpenAPI**: Interactive API documentation and testing.
- **JWT Bearer Authentication**: Secure authentication and authorization.
- **Redis**: Caching layer (via Infrastructure).
- **Newtonsoft.Json & System.Text.Json**: JSON serialization and reference handling.
- **OData (optional)**: Advanced querying support (available in code comments).
- **CORS**: Cross-Origin Resource Sharing configuration.
- **Layered Architecture**: Application, Persistence, Infrastructure, Mapper, and API layers.

## Features

- **Layered Architecture:** Clean separation of concerns with Application, Persistence, Infrastructure, and API layers.
- **Modular Service Registration:** Uses registrar classes for environment, third-party, MVC, and layer-specific service configuration.
- **OpenAPI & Swagger:** Integrated API documentation and JWT Bearer authentication support.
- **Authentication & Authorization:** Built-in JWT authentication.
- **Pagination Helper:** Utilities for paginated API responses.
- **OData (Optional):** Ready for OData integration (commented in code).
- **CORS Support:** Configurable CORS policy.
- **CAP Integration:** Distributed transaction support with CAP, SQL Server, and RabbitMQ.
- **AutoMapper:** Object mapping for DTOs and entities.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (for persistence)
- RabbitMQ (for CAP integration)

### Installation

1. **Clone the repository:**
  git clone https://github.com/your-org/UnionWebApi.git cd UnionWebApi

2. **Configure the database and RabbitMQ:**
   - Update `appsettings.json` with your SQL Server and RabbitMQ connection details.

3. **Restore dependencies:**
  dotnet restore

4. **Build the project:**
  dotnet run --project src/Presentation/UnionWebApi.Api


6. **Access Scalar UI:**
   - Navigate to `https://localhost:<port>/scalar` in your browser.

## Usage

- The API is organized with controllers under the `src/Presentation/UnionWebApi.Api` project.
- JWT Bearer authentication is required for protected endpoints.
- Use the Swagger UI for interactive API exploration and testing.

## Project Structure

## Extending

- **Add new services:** Implement and register via the appropriate registrar class.
- **Add new endpoints:** Create controllers in the API project.
- **Customize serialization:** Modify `MvcRegistrar` for JSON options.

## License

Distributed under the MIT License. See `LICENSE` for more information.

---

*This project is actively maintained and welcomes contributions.*
