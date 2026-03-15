# ECashMeUp API

Backend REST API for the ECashMeUp financial platform developed for Sebaka NPO.

The API handles authentication, transactions, user management, and communication with the MySQL database.

---

## Technology Stack

* C#
* .NET Web API
* MySQL
* MySQL Workbench
* Entity Framework Core

---

## Project Structure

```
ECashMeUp.API
 ├── Controllers/        # API endpoints
 ├── Services/           # Business logic
 ├── Models/             # Data models
 ├── DTOs/               # Data transfer objects
 ├── Repositories/       # Database access layer
 ├── Data/               # Database configuration
 ├── Middleware/         # Custom middleware
 └── Program.cs
```

---

## Prerequisites

Before running the API ensure you have:

* .NET SDK (version 7 or later)
* MySQL Server
* MySQL Workbench

---

## Installation

Clone the repository

```
git clone https://github.com/sebaka/ecashmeup-api.git
```

Navigate to the project directory

```
cd ecashmeup-api
```

Restore dependencies

```
dotnet restore
```

Run the API

```
dotnet run
```

The API will run at:

```
http://localhost:5000
```

---

## Database Setup

1. Install MySQL Server
2. Open MySQL Workbench
3. Create a database

```
CREATE DATABASE ecashmeup;
```

Update the connection string in `appsettings.json`.

Example:

```
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=ecashmeup;user=root;password=yourpassword"
}
```

---

## API Endpoints

Example endpoints:

```
POST /api/auth/login
POST /api/auth/register
GET /api/users
POST /api/transactions
```

---

## Running Migrations (Entity Framework)

```
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Contribution Guidelines

1. Create a feature branch
2. Follow naming convention

```
feature/new-feature
bugfix/issue-name
```

3. Submit Pull Request for review.

---

## Maintainers

Sebaka Development Team
