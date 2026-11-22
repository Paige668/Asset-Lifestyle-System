# IT Asset Manager (Demo)

A small IT asset management demo built with ASP.NET Core 8, EF Core, SQL Server and Angular 21. It covers the asset lifecycle (create, update, delete), check-out and check-in with status rules, an audit log of asset changes, and unit tests for the service-layer rules. Docker Compose runs the API and SQL Server, and a script and guide describe running them on an AWS EC2 host.

## Features

- **Assets**: create, read, update and delete assets (name, serial number, status) through the REST API. A name is required and serial numbers must be unique: the service layer checks before saving, and the database has a unique index.
- **Check-out and check-in**: an asset can be checked out only when it is `InStock` and checked in only when it is `InUse`. Each status change is saved together with a transaction record, and `GET /api/transactions` returns the history.
- **Audit log**: creating, updating or deleting an asset writes an audit entry (action, details, user, time). The API only adds and reads entries; `GET /api/audit` returns the latest 100. Check-outs and check-ins are kept in the transaction history, not in the audit log.
- **Demo login**: "Login as Admin" or "Login as User" is stored in the browser only, and the Admin view adds the create-asset form. The API has no authentication yet (see Known limitations).

## Tech Stack

- **Backend**: ASP.NET Core 8 Web API, EF Core 8, SQL Server 2022, Swagger UI in the Development environment.
- **Frontend**: Angular 21 with standalone components, typed `HttpClient` services and Tailwind CSS 3.
- **Tests**: xUnit, Moq, FluentAssertions, EF Core InMemory provider.
- **Infrastructure**: Docker Compose (API and SQL Server) and a deployment script for AWS EC2.

## Getting Started

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js](https://nodejs.org/) for the Angular frontend
- [.NET 8 SDK](https://dotnet.microsoft.com/) for running the tests or the API outside Docker

### Quick Start

1. Clone the repository.
2. Create `.env` from the example and set a strong SQL Server password:

    ```bash
    cp .env.example .env
    ```

3. Start the API and the database:

    ```bash
    docker-compose up -d --build
    ```

    The API listens on <http://localhost:8080>, for example `GET http://localhost:8080/api/assets`. The Docker image runs in the Production environment, so Swagger UI is not served there.

4. Start the frontend in a second terminal:

    ```bash
    cd src/Frontend
    npm install
    npm start
    ```

    Then open <http://localhost:4200>. The frontend calls the API at `http://localhost:8080`.

### User Guide

1. **Login**: choose **"Login as Admin"** to see the create-asset form, or **"Login as User"** for the asset list only. This is a browser-side demo, not access control.
2. **Assets**: view all assets. **"Check Out"** asks for the name of the person who gets the item; **"Return"** checks it back in. The **Transaction History** tab lists every check-out and return.
3. **Audit Logs**: open "Audit Logs" in the navigation bar to see asset creations, updates and deletions.

## Architecture

- **src/Backend**: the ASP.NET Core Web API.
  - `Controllers`: REST endpoints for assets, transactions and the audit log.
  - `Services`: business rules (validation, status changes) and audit logging.
  - `Data`: EF Core `DbContext`; the database is created with `EnsureCreated()` on startup.
  - `Models`: domain entities.
- **src/Frontend**: the Angular project.
  - `src/app/pages`: login, asset list with transaction history, audit log.
  - `src/app/services`: API calls and the demo login.
- **src/ITAssetManager.Tests**: unit tests for the service layer.

---

## Testing

The unit tests cover the service-layer rules against an isolated EF Core InMemory database.

### Running Tests

```bash
cd src/ITAssetManager.Tests
dotnet test --logger "console;verbosity=detailed"
```

### Test Coverage Matrix

| Test Scenario | Description | Target |
|---------------|-------------|----------|
| **Validation** | Empty name is rejected | AssetService |
| **Validation** | Duplicate serial number is rejected | AssetService |
| **Business rule** | Check-out succeeds when the asset is in stock | TransactionService |
| **State change** | Check-out sets the status to `InUse` | TransactionService |
| **Business rule** | Check-in of an asset that is already in stock is rejected | TransactionService |
| **Audit log** | Creating an asset writes an audit entry | AssetService |
| **Data integrity** | Check-out creates a transaction record | TransactionService |
| **Delete rule** | Delete is refused unless the caller name is "Admin" (a demo stand-in; there is no authentication yet) | AssetService |

### Key Test Features

- **xUnit**: testing framework for .NET
- **Moq**: mocks for isolated tests
- **FluentAssertions**: readable assertions
- **InMemory database**: fast, isolated database tests

**Expected Output**:

```plaintext
Passed!  - Failed:     0, Passed:     8, Skipped:     0, Total:     8
```

---

## AWS Deployment

`deployment/deploy-ec2.sh` installs Docker on an Ubuntu EC2 host and starts the same Compose stack (API and SQL Server). The Angular frontend is not part of that stack.

### Quick Start

1. **Create an EC2 instance** (Ubuntu 22.04, t3.small recommended)
2. **Configure the security group** (ports 22 and 8080)
3. **Upload the project files** via SCP or Git
4. **Run the deployment script**:

   ```bash
   ./deployment/deploy-ec2.sh
   ```

**Full Guide**: see [DEPLOYMENT.md](./DEPLOYMENT.md) for step-by-step instructions.

### Recommended networking

1. Expose 80/443 through a reverse proxy (for example Nginx) instead of opening 8080
2. Keep the API and the database internal to the instance or network when possible

### Architecture Diagram

```
┌─────────────────────────────────────────────┐
│           AWS EC2 Instance                  │
│  ┌─────────────────────────────────────┐   │
│  │     Docker Compose Environment      │   │
│  │  ┌──────────────┐  ┌─────────────┐ │   │
│  │  │   Backend    │  │  SQL Server │ │   │
│  │  │  (.NET 8)    │  │   (2022)    │ │   │
│  │  │  Port: 8080  │  │ 127.0.0.1:  │ │   │
│  │  │              │  │   1433      │ │   │
│  │  └──────────────┘  └─────────────┘ │   │
│  └─────────────────────────────────────┘   │
└─────────────────────────────────────────────┘
         ↑
    Port 8080 (API)
```

### Production Checklist (not done in this demo)

- [ ] Configured HTTPS/SSL certificates
- [ ] Set up automated backups
- [ ] Enabled CloudWatch monitoring
- [ ] Restricted security groups to known IP ranges
- [ ] Implemented authentication/authorization

---

## Known limitations

- **No authentication or server-side authorization.** The controllers pass a fixed demo user name, and the Admin/User choice exists only in the browser. The JWT Bearer package is referenced but not configured.
- **Audit log scope.** Only asset creations, updates and deletions are logged; check-outs and check-ins are in the transaction history.
- **No CI workflow yet.**
- **Schema changes.** The database is created with `EnsureCreated()`; there are no EF Core migrations.

## License

MIT
