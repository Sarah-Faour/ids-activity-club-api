# IDS Activity Club API

A clean, layered ASP.NET Core Web API built as part of the IDS Internship assignments, following best practices expected in enterprise-grade systems.

---

## Overview

This project implements a backend API for an Activity Club system, handling:
- Members
- Users
- Roles
- Events
- Guides
- Lookups

The architecture follows a clean separation of concerns using:
- Repository pattern
- Service layer
- DTOs
- AutoMapper
- Global exception handling

---

##  Solution Structure

'''text'''
...structure...

ActivityClub
|
|__ ActivityClub.API // API layer (controllers, middleware, DI)
|__ ActivityClub.Contracts // DTOs, constants, shared contracts
|__ ActivityClub.Data // EF Core DbContext & entities
|__ ActivityClub.Repositories // Generic & specific repositories
|__ ActivityClub.Services // Business logic (services)
|
|-- ActivityClub.sln
|-- README.md
|-- .gitignore




---

## Technologies Used

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- AutoMapper
- Swagger / OpenAPI
- Dependency Injection
- Git & GitHub

---

## Architecture Highlights

- Controllers handle HTTP requests only
- Services contain all business logic
- Repositories abstract database access
- DTOs prevent entity leakage
- AutoMapper is used for clean object mapping
- Global exception middleware ensures consistent error responses

---

## How to Run the Project

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run database migrations if needed
4. Run the API:
   '''bash'''
   dotnet run
5. open Swagger: https://localhost:<port>/swagger


