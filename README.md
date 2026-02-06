# Emergency Dispatcher & Hospital Pre-Notification System

A lightweight emergency coordination system that enables dispatchers to capture emergency data, store it securely, and pre-notify hospitals using standardized SBAR communication.

## Tech Stack

- **.NET 10** / Blazor Server
- **MudBlazor 8** (Material Design UI)
- **PostgreSQL 16** via EF Core
- **ASP.NET Identity** (cookie auth)
- **XUnit** + Moq + bUnit (testing)
- **Docker** + docker-compose

## Quick Start

### Docker (recommended)

```bash
docker-compose up
```

Visit `http://localhost:8080`

### Local Development

Requires .NET 10 SDK and a PostgreSQL instance.

```bash
# Update connection string in appsettings.Development.json
cd src/EmergencyDispatcher.Web
dotnet run
```

### Run Tests

```bash
dotnet test
```

## Default Credentials

| Role       | Email                    | Password     |
|------------|--------------------------|--------------|
| Admin      | admin@dispatch.com       | Admin123!    |
| Dispatcher | dispatcher@dispatch.com  | Dispatch123! |

## Features

- Emergency case intake with member lookup
- SBAR message generation (call script, message, email)
- Case status tracking (Open > Notified > En Route > Arrived > Closed)
- Hospital directory management
- Member medical profiles
- Notification logging
- Full audit trail
- Role-based access (Admin, Dispatcher)
- Dark theme optimized for dispatch environments

## Project Structure

```
src/EmergencyDispatcher.Web/    # Blazor Server application
tests/EmergencyDispatcher.Tests/ # XUnit test suite
docker-compose.yml               # Local development stack
```
