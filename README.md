# IDS Activity Club API

A secure, layered ASP.NET Core Web API built as part of the IDS Internship assignments, designed using enterprise-grade architecture principles and production-level security practices.

---

## Overview

This backend system manages an Activity Club domain including:

- Users (authentication & profile management)
- Members (profile linked to users)
- Guides
- Events
- Roles
- Lookups
- Event-Member assignments
- Event-Guide assignments

The system follows a clean architecture approach with strict separation of concerns and secure-by-default access rules.

---

##  Solution Structure

```text

ActivityClub
|
|__ ActivityClub.API          // Controllers, middleware, authentication setup
|__ ActivityClub.Contracts    // DTOs, constants, request/response models
|__ ActivityClub.Data         // EF Core DbContext & database entities
|__ ActivityClub.Repositories // Generic repository abstraction
|__ ActivityClub.Services     // Business logic layer
|
|-- ActivityClub.sln
|-- README.md
|-- .gitignore


```


---

## Technologies Used

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity Password Hasher
- JWT Bearer Authentication
- Role-Based Authorization
- AutoMapper
- Swagger / OpenAPI
- Dependency Injection
- Global Exception Handling Middleware
- Git & GitHub

---

## Architecture Highlights

Layered Architecture

- Controllers handle HTTP concerns only.
- Services contain business logic.
- Repositories abstract database access.
- DTOs prevent entity exposure.
- AutoMapper handles mapping between entities and DTOs.

---

# Authentication & Security

The API uses JWT-based authentication with:

- Token signature validation
- Issuer validation
- Audience validation
- Expiration validation
- Role-based claims

Security configuration includes:

- UseAuthentication() before UseAuthorization()
- Secure-by-default controllers ([Authorize])
- Role-based access control ([Authorize(Roles = "Admin")])
- Public endpoints explicitly marked with [AllowAnonymous]

---

# Authorization Model

The system implements:

- Admin-only write operations (POST/PUT/DELETE)
- Admin-only access to sensitive user data
- Public read-only endpoints (Events, Roles, Lookups)
- Self-service endpoints (/me) for:
  - Viewing own profile
  - Viewing own roles
  - Updating own email
  - Changing own password

---

# Password Security

- All passwords are hashed using IPasswordHasher<User> (ASP.NET Core Identity standard).
- No plaintext passwords stored.
- Password rehashing is supported when needed.
- Secure password change flow verifies current password before updating.

---

# Registration Flow

The API supports:

``` arduino
POST /api/auth/register
```
Registration:

- Validates email uniqueness (case-insensitive)
- Validates Gender lookup
- Assigns default role = Member
- Creates:
  - User record
  - Member profile (minimal record)
- Wrapped in a database transaction (atomic operation)
- Returns JWT token (auto-login)

---

# Global Exception Handling

A custom middleware ensures:

- Centralized error handling
- Consistent JSON error responses
- No stack traces leaked in production

---

# API Security Rules (Current State)

| Area                    | Access             |
| ----------------------- | ------------------ |
| Login                   | Public             |
| Register                | Public             |
| Read Events             | Public             |
| Read Roles              | Public             |
| Read Lookups            | Public             |
| Users Management        | Admin Only         |
| Member/Guide Management | Admin Only         |
| Event Writing           | Admin Only         |
| Self `/me` Endpoints    | Authenticated User |


---

## How to Run the Project

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Ensure database is created
4. Run the API:
   ```bash
   dotnet run 
   ```
5. open Swagger:
   ```bash
   https://localhost:<port>/swagger
   
   ```
   Use the Authorize button in Swagger to paste your JWT token.

---

# Design Principles Applied

- Secure by default
- Explicit public endpoints
- Atomic registration workflow
- Single hashing strategy
- Minimal duplication
- No entity leakage
- Proper separation of concerns
- Production-style JWT implementation


