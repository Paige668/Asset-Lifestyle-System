# IT Asset Manager (Demo)

An enterprise-style IT Asset Management system built with ASP.NET Core 8 and Angular 18, featuring asset lifecycle management, transactional check-in/check-out workflows, and immutable audit logging. Includes unit tests and a Docker-based AWS EC2 deployment for demonstrating production-like delivery.

## Features

- **Asset Management**: Full CRUD for IT Assets (Name, Serial Number, Status).
- **Transaction System**: Check-out and Check-in workflows with history tracking.
- **Audit Logging**: Immutable, append-only log of all system changes.
- **Role-Based Access**: Admin and User roles (simulated).
- **Premium UI**: Responsive, modern interface using TailwindCSS.

## Tech Stack

- **Backend**: ASP.NET Core 8 Web API, EF Core, SQL Server 2022.
- **Frontend**: Angular 18 (Standalone Components), TailwindCSS.
- **Infrastructure**: Docker Compose, GitHub Actions.

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js](https://nodejs.org/) (for local frontend dev)
- [.NET 8 SDK](https://dotnet.microsoft.com/) (optional, for local backend dev)

### Quick Start (Docker)

1. Clone the repository (or navigate to the folder).
2. Start the application:

    ```bash
    docker-compose up -d --build
    ```

3. Access the application:
    - **Frontend**: [http://localhost:4200](http://localhost:4200)
    - **Backend Swagger**: [http://localhost:8080/swagger]

### User Guide

1. **Login**:
    - Select **"Login as Admin"** to manage assets.
    - Select **"Login as User"** to view read-only data (demo mode permissions).
2. **Dashboard**:
    - View all assets.
    - Click **"Check Out"** to assign an item to a user.
    - Click **"Return"** to check an item back in.
3. **Audit Logs**:
    - Click "Audit Logs" in the navigation bar to view system activity.

## Architecture

- **Src/Backend**: Contains the C# WebAPI project.
  - `Controllers`: REST API endpoints.
  - `Services`: Business logic layer with validation and audit logging.
  - `Data`: EF Core DbContext.
  - `Models`: Domain entities.
- **Src/Frontend**: Contains the Angular project.
  - `src/app/pages`: UI features (Login, Assets, Audit).
  - `src/app/services`: API integration.

---

## Testing

This project includes unit tests focused on service-layer business logic using an isolated InMemory database setup.

### Running Tests

```bash
cd src/ITAssetManager.Tests
dotnet test --logger "console;verbosity=detailed"
```

### Test Coverage Matrix

| Test Scenario | Description | Target |
|---------------|-------------|----------|
| **Validation Tests** | Empty name validation | ✅ AssetService |
| **Validation Tests** | Duplicate serial number check | ✅ AssetService |
| **Business Logic** | Check-out availability validation | ✅ TransactionService |
| **State Management** | Status update on check-out | ✅ TransactionService |
| **Business Logic** | Check-in validation (prevent invalid returns) | ✅ TransactionService |
| **Audit Logging** | Audit log creation on operations | ✅ AssetService |
| **Data Integrity** | Transaction record creation | ✅ TransactionService |
| **Authorization** | Permission-based delete validation | ✅ AssetService |

### Key Test Features

- **xUnit**: Modern testing framework for .NET
- **Moq**: Mock dependencies for isolated testing
- **FluentAssertions**: Readable assertion syntax
- **InMemory Database**: Fast, isolated database testing

**Expected Output**:

```plaintext
Passed!  - Failed:     0, Passed:     8, Skipped:     0, Total:     8
```

---

## AWS Deployment

Deploy this application to AWS EC2 for cloud hosting.

### Quick Start

1. **Create EC2 Instance** (Ubuntu 22.04, t3.small recommended)
2. **Configure Security Groups** (ports 22, 8080, 4200)
3. **Upload project files** via SCP or Git
4. **Run deployment script**:

   ```bash
   ./deployment/deploy-ec2.sh
   ```

**Full Guide**: See [DEPLOYMENT.md](./DEPLOYMENT.md) for detailed step-by-step instructions.

### Prerequisites on EC2

1. Docker + Docker Compose installed
2. Ports configured (or behind reverse proxy)

   For development and evaluation, you may open ports 22, 8080, and 4200 for direct access. For production-like setups, expose only 80/443 and route traffic via a reverse proxy (e.g., Nginx).

### Recommended networking

1. Expose 80/443 via reverse proxy (Nginx)
2. Keep API and DB internal to the instance/network when possible

### Architecture Diagram

```
┌─────────────────────────────────────────────┐
│           AWS EC2 Instance                  │
│  ┌─────────────────────────────────────┐   │
│  │     Docker Compose Environment      │   │
│  │  ┌──────────────┐  ┌─────────────┐ │   │
│  │  │   Backend    │  │  SQL Server │ │   │
│  │  │  (.NET 8)    │  │   (2022)    │ │   │
│  │  │  Port: 8080  │  │Port: 1433   │ │   │
│  │  └──────────────┘  └─────────────┘ │   │
│  └─────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
         ↑                    ↑
    Port 8080            Port 4200
    (API/Swagger)        (Frontend - separate)
```

### Production Checklist

- [ ] Changed default SQL Server password
- [ ] Configured HTTPS/SSL certificates
- [ ] Set up automated backups
- [ ] Enabled CloudWatch monitoring
- [ ] Configured proper security groups
- [ ] Implemented authentication/authorization

---

## License

MIT
