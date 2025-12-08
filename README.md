# MovieVault - Movie Management System

A robust ASP.NET Core 8.0 MVC application for managing movies, customers, and memberships. This project demonstrates modern .NET development practices, including Docker containerization, Entity Framework Core with PostgreSQL, and comprehensive exception handling.

## 🚀 Features

### Functional Features

-   **Movie Management**: Create, read, update, and delete movies with image upload support.
-   **Customer Management**: Manage customer profiles and their subscriptions.
-   **Membership System**: Define membership types with different fees, durations, and discount rates.
-   **Genre Management**: Categorize movies into genres.
-   **Statistics Dashboard**: View insights like available action movies, total movie counts, and popular genres.
-   **Pagination & Sorting**: Efficiently browse large lists of data.

### Technical Features

-   **Containerization**: Fully dockerized application and database using Docker Compose.
-   **Database**: PostgreSQL integration using Entity Framework Core.
-   **Architecture**: Clean separation of concerns using **Repository** and **Service** patterns.
-   **Global Exception Handling**: Centralized middleware to catch and handle errors gracefully.
-   **Audit Logging**: Automatic tracking of database changes (Added/Modified/Deleted entities) using EF Core Interceptors.
-   **Logging**: Structured logging with **Serilog**.
-   **Database Seeding**: Automated data population for development and testing.

## 🛠️ Tech Stack

-   **Framework**: ASP.NET Core 8.0 (MVC)
-   **Language**: C#
-   **Database**: PostgreSQL 16
-   **ORM**: Entity Framework Core 8.0
-   **Logging**: Serilog
-   **Containerization**: Docker & Docker Compose
-   **Frontend**: Razor Views, Bootstrap

## 📋 Prerequisites

-   [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.
-   (Optional) [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) for local development without Docker.

## 🐳 Getting Started (Docker)

The easiest way to run the application is using Docker Compose.

1.  **Clone the repository** (if applicable) or navigate to the project folder.

2.  **Start the application**:

    ```bash
    docker-compose up --build -d
    ```

    This command builds the application image, starts the PostgreSQL container, and launches the app.

3.  **Access the application**:
    Open your browser and go to: **[http://localhost:8080](http://localhost:8080)**

4.  **Stop the application**:
    ```bash
    docker-compose down
    ```

### ⚠️ Troubleshooting Docker

If you encounter database errors or need to reset the data:

```bash
# Stop containers and remove volumes (clears database)
docker-compose down -v

# Restart and rebuild
docker-compose up --build -d
```

## 💻 Getting Started (Local Development)

If you prefer to run the application locally without Docker for the app itself (but still using a database):

1.  **Update Connection String**:
    Ensure `appsettings.Development.json` points to a valid PostgreSQL instance.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Host=localhost;Database=tp3;Username=postgres;Password=postgres"
    }
    ```

2.  **Run the Database**:
    You can still use Docker for just the database:

    ```bash
    docker run --name tp3-db -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=tp3 -p 5432:5432 -d postgres:16
    ```

3.  **Apply Migrations**:

    ```bash
    dotnet ef database update
    ```

4.  **Seed the Database**:

    ```bash
    dotnet run --seed
    ```

5.  **Run the App**:
    ```bash
    dotnet run
    ```
    Access at `http://localhost:5000`.

## 📂 Project Structure

```
tp3/
├── Controllers/          # Handles HTTP requests (Movies, Customers, etc.)
├── Data/                 # DbContext, Seeder, and Audit Interceptor
├── Middleware/           # Global Exception Handler
├── Migrations/           # EF Core database migrations
├── Models/               # Domain entities (Movie, Customer, etc.)
├── Repositories/         # Data access layer interfaces and implementations
├── Services/             # Business logic layer interfaces and implementations
├── ViewModels/           # Data transfer objects for Views
├── Views/                # Razor pages (UI)
├── wwwroot/              # Static files (CSS, JS, Images)
├── Dockerfile            # Docker build instructions
├── docker-compose.yml    # Docker orchestration config
└── Program.cs            # App entry point and configuration
```

## 🔑 Key Implementation Details

### Exception Handling

The application uses a custom `GlobalExceptionHandlerMiddleware` to catch unhandled exceptions.

-   **HTML Requests**: Redirects users to a user-friendly error page (`/Home/Error`) or a specific "Not Found" page.
-   **API Requests**: Returns a standardized JSON error response.

### Image Upload

Images are handled in the `MovieService`. When a movie is created or updated:

1.  The file is validated.
2.  A unique filename is generated (GUID).
3.  The file is saved to `wwwroot/images`.
4.  The file path is stored in the database.

### Audit Logging

An `AuditInterceptor` is registered with the `DbContext`. It intercepts `SaveChanges` calls to:

1.  Identify added, modified, or deleted entities.
2.  Create an `AuditLog` record for each change.
3.  Store details like the table name, action type, and timestamp.
