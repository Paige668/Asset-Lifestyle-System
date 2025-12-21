# IT Asset Manager (Demo)

A "SEB-style" Professional IT Asset Management System built with **ASP.NET Core 8** and **Angular 18**.
This project demonstrates a premium enterprise application architecture with Asset management, Transaction tracking, and Audit logging.

## Features

- **Asset Management**: Full CRUD for IT Assets (Name, Serial Number, Status).
- **Transaction System**: Check-out and Check-in workflows with history tracking.
- **Audit Logging**: Immutable log of all system changes.
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
    - **Frontend**: [http://localhost:4200](http://localhost:4200) (or mapped port, usually 80/4200 depending on setup, check `docker ps`)
    - **Backend Swagger**: [http://localhost:8080/swagger](http://localhost:8080/swagger)

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
  - `Data`: EF Core DbContext.
  - `Models`: Domain entities.
- **Src/Frontend**: Contains the Angular project.
  - `src/app/pages`: UI features (Login, Assets, Audit).
  - `src/app/services`: API integration.

## License

MIT
