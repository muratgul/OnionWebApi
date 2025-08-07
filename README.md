# OnionWebApi

OnionWebApi ( Onion Architecture ) is a modular, layered web API project built with **.NET 9** and C# 13, designed for scalable, maintainable, and modern web service development. The project leverages best practices such as dependency injection, modular service registration, and OpenAPI/Scalar documentation.

## Technologies Used

- **.NET 9** and **C# 13**: Modern, high-performance platform for web API development.
- **ASP.NET Core Web API**: Framework for building RESTful services.
- **ASP.NET Core Health Checks**: For monitoring the health of the application.
- **Entity Framework Core**: Object-relational mapping (ORM) for data access.
- **AutoMapper**: Automatic mapping between DTOs and entities.
- **MassTransit**: Distributed application framework for .NET.
- **RabbitMQ**: Message queue infrastructure.
- **Scalar / OpenAPI**: Interactive API documentation and testing.
- **JWT Bearer Authentication**: Secure authentication and authorization.
- **Redis**: Caching layer (via Infrastructure).
- **Otp.NET & QRCoder**: For Two-Factor Authentication (2FA) using Time-Based One-Time Passwords (TOTP).
- **Newtonsoft.Json & System.Text.Json**: JSON serialization and reference handling.
- **OData (optional)**: Advanced querying support (available in code comments).
- **CORS**: Cross-Origin Resource Sharing configuration.
- **Layered Architecture**: Application, Persistence, Infrastructure, Mapper, and API layers.

## Features

- **Layered Architecture:** Clean separation of concerns with Application, Persistence, Infrastructure, and API layers.
- **Modular Service Registration:** Uses registrar classes for environment, third-party, MVC, and layer-specific service configuration.
- **OpenAPI & Scalar:** Integrated API documentation and JWT Bearer authentication support.
- **Authentication & Authorization:** Built-in JWT authentication.
- **Pagination Helper:** Utilities for paginated API responses.
- **OData (Optional):** Ready for OData integration (commented in code).
- **CORS Support:** Configurable CORS policy.
- **MassTransit Integration:** Distributed application framework for .NET.
- **AutoMapper:** Object mapping for DTOs and entities.
- **OTP Service:** Generates and validates Time-Based One-Time Passwords (TOTP) for Two-Factor Authentication (2FA), including QR code generation for easy setup in authenticator apps.
- **Email Service:** A robust email service for sending transactional and bulk emails. Features include:
  - **Single & Bulk Sending:** Send individual or mass emails efficiently.
  - **Templating:** Use dynamic templates for personalized emails.
  - **Attachments & Custom Headers:** Supports sending files and custom email headers.
  - **Scheduled & Background Sending:** Queue emails for later delivery or send them in the background to avoid blocking application threads.
- **Idempotency:** Prevents duplicate operations for POST and PATCH requests using a middleware and an `[Idempotent]` attribute. It ensures that retried requests with the same `Idempotency-Key` header are processed only once, returning a cached response for subsequent attempts.
- **Health Checks:** Provides a health check endpoint to monitor the status of the application and its dependencies (e.g., database, Redis).

### Idempotency Usage

To make an endpoint idempotent, add the `[Idempotent]` attribute to a controller action and provide a unique `Idempotency-Key` in the request header.

**Example:**

```csharp
[HttpPost]
[Idempotent] // Enable idempotency for this endpoint
public async Task<IActionResult> CreateBrand(CreateBrandCommandRequest request)
{
    var response = await _mediator.Send(request);
    return StatusCode(StatusCodes.Status201Created, response);
}
```

When calling this endpoint, the client must include the header:

`Idempotency-Key: 0d8f3839-9357-4e42-8093-10d6f7c3652c`

### Health Check Usage

The health check endpoint is available at `/health`. It provides the status of the application and its dependencies.

**Example:**

To check the health of the application, navigate to `https://localhost:<port>/health` in your browser or use a tool like `curl`:

```bash
curl https://localhost:<port>/health
```

The response will indicate the status of the application and its dependencies (e.g., "Healthy", "Unhealthy").
- **File Service:** A service for managing file uploads and downloads. It supports local file storage and can be extended for cloud storage providers. It handles file validation, and storage, and provides a secure way to access files.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (for persistence)
- RabbitMQ (for CAP integration)

### Installation

1. **Clone the repository:**
  git clone https://github.com/muratgul/OnionWebApi.git cd OnionWebApi

2. **Configure the database and RabbitMQ:**
   - Update `appsettings.json` with your SQL Server and RabbitMQ/MassTransit connection details.

3. **Restore dependencies:**
  dotnet restore

4. **Build the project:**
  dotnet run --project src/Presentation/OnionWebApi.Api


6. **Access Scalar UI:**
   - Navigate to `https://localhost:<port>/scalar` in your browser.

## Usage

- The API is organized with controllers under the `src/Presentation/OnionWebApi.Api` project.
- JWT Bearer authentication is required for protected endpoints.
- Use the Scalar UI for interactive API exploration and testing.

## Project Structure

## Extending

- **Add new services:** Implement and register via the appropriate registrar class.
- **Add new endpoints:** Create controllers in the API project.
- **Customize serialization:** Modify `MvcRegistrar` for JSON options.

## License

Distributed under the MIT License. See `LICENSE` for more information.

---

*This project is actively maintained and welcomes contributions.*
