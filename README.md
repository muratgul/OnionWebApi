# OnionWebApi

OnionWebApi ( Onion Architecture ) is a modular, layered web API project built with **.NET 9** and C# 13, designed for scalable, maintainable, and modern web service development. The project leverages best practices such as dependency injection, modular service registration, and OpenAPI/Scalar documentation.

## Technologies Used

- **.NET 9** and **C# 13**: Modern, high-performance platform for web API development.
- **ASP.NET Core Web API**: Framework for building RESTful services.
- **ASP.NET Core Health Checks**: For monitoring the health of the application.
- **Entity Framework Core**: Object-relational mapping (ORM) for data access.
- **Mapster**: A high-performance object-to-object mapper.
- **MassTransit**: Distributed application framework for .NET.
- **RabbitMQ**: Message queue infrastructure.
- **Scalar / OpenAPI**: Interactive API documentation and testing.
- **JWT Bearer Authentication**: Secure authentication and authorization.
- **Redis**: Caching layer (via Infrastructure).
- **Otp.NET & QRCoder**: For Two-Factor Authentication (2FA) using Time-Based One-Time Passwords (TOTP).
- **Newtonsoft.Json & System.Text.Json**: JSON serialization and reference handling.
- **OData (optional)**: Advanced querying support (available in code comments).
- **CORS**: Cross-Origin Resource Sharing configuration.
- **MediatR**: A high-performance implementation of the mediator pattern.
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
- **Mapster:** High-performance object mapping for DTOs and entities.
- **OTP Service:** Generates and validates Time-Based One-Time Passwords (TOTP) for Two-Factor Authentication (2FA), including QR code generation for easy setup in authenticator apps.
- **Email Service:** A robust email service for sending transactional and bulk emails. Features include:
  - **Single & Bulk Sending:** Send individual or mass emails efficiently.
  - **Templating:** Use dynamic templates for personalized emails.
  - **Attachments & Custom Headers:** Supports sending files and custom email headers.
  - **Scheduled & Background Sending:** Queue emails for later delivery or send them in the background to avoid blocking application threads.
- **Idempotency:** Prevents duplicate operations for POST and PATCH requests using a middleware and an `[Idempotent]` attribute. It ensures that retried requests with the same `Idempotency-Key` header are processed only once, returning a cached response for subsequent attempts.
- **Health Checks:** Provides a health check endpoint to monitor the status of the application and its dependencies (e.g., database, Redis).
- **API Versioning:** OnionWebApi supports route-based API versioning, allowing each version of the API to be clearly defined within the URL path (e.g., `/api/v1/products`). This approach simplifies version management and works seamlessly with tools like Scalar for interactive testing.
- **Rate Limiting:** Protects the API from excessive requests using a fixed window rate limiter. This feature is highly configurable through `appsettings.json`.
- **Real-time Notifications with SignalR:** Provides real-time communication capabilities using SignalR. A global hub is configured to push notifications to all connected clients.

### Rate Limiting Usage

Rate limiting is enabled and configured in `appsettings.Development.json` or `appsettings.Production.json`.

**Example Configuration:**

```json
"RateLimiting": {
  "Enabled": true,
  "PermitLimit": 5,
  "WindowSeconds": 10,
  "QueueLimit": 0
}
```

-   **Enabled**: Set to `true` to enable rate limiting.
-   **PermitLimit**: The maximum number of requests allowed in a time window.
-   **WindowSeconds**: The duration of the time window in seconds.
-   **QueueLimit**: The number of requests that can be queued when the limit is reached. A value of `0` means requests are rejected immediately.

When the limit is exceeded, the API will return a `429 Too Many Requests` status code.
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
curl https://localhost:<port>/healthapi
```

The response will indicate the status of the application and its dependencies (e.g., "Healthy", "Unhealthy").
- **File Service:** A service for managing file uploads and downloads. It supports local file storage and can be extended for cloud storage providers. It handles file validation, and storage, and provides a secure way to access files.

### Versioning Controller Structure
API versioning is configured in Program.cs using the AddApiVersioning method:

**V1 Example:**
```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("v1 product list");
}
```

**V2 Example:**
```csharp
[ApiController]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("v2 enhanced product list");
}
```

**Testing in Scalar**

With route-based versioning, no additional query string or header is required to specify the API version. Simply call endpoints like:
`GET https://localhost:<port>/api/v1/products`
`GET https://localhost:<port>/api/v2/products`

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (for persistence)
- RabbitMQ (for CAP integration)

### Installation

1. **Clone the repository:**
  ```bash
  git clone https://github.com/muratgul/OnionWebApi.git
  cd OnionWebApi
  ```

2. **Restore dependencies:**
  ```bash
  dotnet restore
  ```

3. **Apply Database Migrations:**
   Run the following command to apply the Entity Framework migrations and set up the database schema:
   ```bash
   dotnet ef database update --project src/Infrastructure/OnionWebApi.Persistence
   ```

4. **Build the project:**
  dotnet run --project src/Presentation/OnionWebApi.Api


5. **Access Scalar UI:**
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
