# ActivityClub — ASP.NET Core API + MVC Web (IDS Internship Project)

A full-stack **Activity Club Management System** built during the IDS .NET internship using **enterprise-style layered architecture**:
- **ASP.NET Core Web API** (backend)
- **ASP.NET Core MVC Web App** (frontend UI)
- **SQL Server + Entity Framework Core**
- **JWT Authentication + Role-Based Authorization**
- **AdminLTE UI Template**

This repository contains the complete solution (API + Web), designed using clean architecture principles and production-level security practices.

---

## Solution Overview

The system manages an Activity Club domain with:

- **Users** (authentication + profile management)
- **Roles** (Admin / Member / Guide)
- **Members** (profile linked to a user)
- **Guides** (profile linked to a user)
- **Events**
- **Lookups** (Category, Status, Gender, Profession, …)
- **EventMember** (member self-join)
- **EventGuide** (admin assigns/unassigns guides to events)

---

## Key Features

### Authentication & Authorization
- JWT-based authentication
- Role-based authorization:
  - **Admin**: full management (CRUD + activation)
  - **Member**: self-service profile + join events
  - **Guide**: self-service profile
- Web app stores JWT in an **HttpOnly cookie**
- Web forwards JWT to API automatically via an HTTP handler

### Admin Dashboard (Web UI)
- Manage **Events** (CRUD + deactivate/reactivate)
- Manage **Guides** (CRUD + deactivate/reactivate)
- Manage **Event-Guide assignments** inside the Event Edit page:
  - View assigned guides (admin sees active + inactive)
  - Assign active guides
  - Unassign guides

### Public Pages (Web UI)
- Browse **Events**
- View **Event Details**
- Browse **Guides**
- View **Guide Details**

### Member Features
- Member profile view/edit
- **Join an event** (self-join) with server-side rule:
  - cannot join past events

### Guide Features
- Guide profile view/edit

---

##  Solution Structure

```text
ActivityClub
|
|__ ActivityClub.API          // Web API controllers, middleware, & authentication setup
|__ ActivityClub.Contracts    // DTOs, constants, & ApiErrorResponse
|__ ActivityClub.Data         // EF Core DbContext & Database Entities
|__ ActivityClub.Repositories // Generic repository abstraction
|__ ActivityClub.Services     // Business logic layer
|-- ActivityClub.Web          // MVC UI, AdminLTE, API clients, & JWT cookie auth
|
|-- ActivityClub.sln
|-- README.md
|-- .gitignore


```


---

## Technologies Used

### Backend (API)
- ASP.NET Core Web API
- Entity Framework Core (EF Core)
- SQL Server
- ASP.NET Core Identity Password Hasher
- JWT Bearer Authentication
- Role-Based Authorization
- AutoMapper
- Swagger / OpenAPI
- Dependency Injection
- Global Exception Handling Middleware

### Frontend (Web MVC)
- ASP.NET Core MVC (Razor Views)
- AdminLTE Dashboard Template (UI layout + components)
- Bootstrap (via AdminLTE)
- jQuery (via AdminLTE)
- jQuery Validation + Unobtrusive Validation

### Authentication Bridge (Web ↔ API)
- HttpClientFactory (Typed API Clients)
- Secure JWT storage in HttpOnly Cookie
- Custom Authentication Scheme in MVC (JwtCookieAuthenticationHandler)
- Outgoing request handler to attach token to API calls (JwtForwardingHandler)

### Dev / Tooling
- Git & GitHub

---

## Architecture Highlights

### Layered Architecture (Backend API)

The API follows a strict layered architecture:

Controller -> Service -> Repository -> DbContext

- Controllers handle HTTP concerns only.
- Services contain business logic and enforce domain rules.
- Repositories abstract database access via a generic repository pattern.
- DTOs prevent entity exposure across boundaries.
- AutoMapper handles mapping between entities and DTOs.
- Contracts project defines all request/response DTOs to enforce clean API boundaries.
- No EF entities are exposed directly to clients.

### Layered Architecture (Web MVC)

The Web application follows a separate layered flow:

MVC Controller -> UI Service -> API Client (HttpClientFactory) -> API

- MVC Controllers handle UI routing and model binding.
- UI Services orchestrate API calls and apply UI-specific transformations.
- API Clients use HttpClientFactory to communicate with the API.
- JWT token is automatically forwarded to API via a custom HTTP handler.
- ViewModels are used to prevent DTO leakage directly into Razor views.


This ensures:
- Clear separation between UI logic and backend logic.
- No direct database access from Web.
- Clean, testable boundaries between layers.

---

## Authentication & Security

### API Authentication

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


### Web Authentication (JWT Cookie Bridge)

The Web application does not re-implement authentication logic.
Instead, it securely delegates authentication to the API:

- JWT returned from API login/register is stored in an HttpOnly cookie
- Custom MVC authentication scheme:
   - JwtCookieAuthenticationHandler
   - Builds HttpContext.User from the cookie token
- Outgoing API calls automatically attach:
   - Authorization: Bearer <token>
     via JwtForwardingHandler


This design:
- Avoids storing tokens in localStorage (reduces XSS risk)
- Maintains a stateless API
- Keeps authentication centralized in the API layer

---

## Authorization Model

The system implements role-based authorization across both API and Web:

### API Rules
- Admin-only write operations (POST/PUT/DELETE)
- Admin-only access to sensitive user data
- Public read-only endpoints (Events, Roles, Lookups)
- Public event details show only active assigned guides
- Admin endpoints expose extended data (including inactive assignments)
- Self-service endpoints (/me) for:
  - Viewing own profile
  - Viewing own roles
  - Updating own email
  - Changing own password

### Web UI Enforcement
- Admin dashboard routes restricted to Admin role
- Member self-join event restricted to Member role
- Guide profile editing restricted to Guide role
- Role-based rendering in Razor views (User.IsInRole)

---

## Activation & Soft-State Model

The system supports activation control for domain entities:

- Events can be Activated / Deactivated by Admin
- Guides can be Activated / Deactivated by Admin
- Public pages show only Active entities
- Admin views can display inactive records for management

This allows safe administrative control without physical deletion.

---

## Password Security

- All passwords are hashed using IPasswordHasher<User> (ASP.NET Core Identity standard).
- No plaintext passwords stored.
- Password rehashing is supported when needed.
- Secure password change flow verifies current password before updating.
- Password comparison uses the framework’s secure verification method.

---

## Registration Flow

The API supports:

``` http
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
- Web stores token securely in HttpOnly cookie

---

## Event-Guide Assignment Model

The system supports controlled assignment between Events and Guides:

- Admin can assign a Guide to an Event
- Admin can unassign a Guide from an Event
- Public endpoint returns only active assigned guides
- Admin endpoint returns full assignment view (including inactive guides)
- Assignments managed inside Admin Event Edit page (Web UI)

This demonstrates many-to-many relationship handling with role-based control.
The implementation ensures proper separation between public exposure and administrative management endpoints.

---

## Global Exception Handling

A custom middleware ensures:

- Centralized error handling
- Consistent JSON error responses
- Structured ApiErrorResponse contract
- No stack traces leaked in production
- Separation of exception logic from controllers

---

## API Security Rules (Current State)

|         Area            |            Access           |
|-------------------------|-----------------------------|
| Login                   | Public                      |
| Register                | Public                      |
| Read Events             | Public                      |
| Read Roles              | Public                      |
| Read Lookups            | Public                      |
| Public Event Details    | Public (active guides only) |
| Users Management        | Admin Only                  |
| Member Management       | Admin Only                  |
| Guide Management        | Admin Only                  |
| Event Writing           | Admin Only                  |
| Event-Guide Assignments | Admin Only                  |
| Self `/me` Endpoints    | Authenticated User          |


---

## How to Run the Project

### Prerequisites:
 - .NET SDK (latest LTS)
 - SQL Server (local instance or SQL Server Express)
 - Visual Studio 2022 (recommended)

1. Clone the repository
   ```bash
   git clone https://github.com/Sarah-Faour/activityclub-dotnet.git
   cd activityclub-dotnet
   ```
2. Configure the Database
   Update the connection string inside: 
   ```text
   ActivityClub.API/appsettings.json
   ```
   Example:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=ActivityClubDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
3. Ensure SQL Server is running.
4. Run the API:
   ### Recommended Way (Visual Studio)

   1. Open the Solution:
      ```text
       ActivityClub.sln
       ```
   2. Set Multiple Startup Projects
      In Visual Studio:
      1. Right-click the Solution
      2. Select Set Startup Projects
      3. Choose Multiple startup projects
      4. Set:
          - ActivityClub.API -> Start
          - ActivityClub.Web -> Start
      Click OK
   3. Press Run (▶)
   
   Both:
    - API (Swagger)
    - Web MVC application
   will start simultaneously.
   
   ### Access Points:

   **API Swagger:**
   ```text
   https://localhost:<API_PORT>/swagger
   ```
   To test protected endpoints:
    1. Login or Register
    2. Copy JWT token
    3. Click Authorize
    4. Paste:
       ```text
        Bearer <your_token>
        ```
    
   **Web Application:**
   ```text 
    https://localhost:<WEB_PORT>/
    ```
    The Web application:
     - Handles login/register
     - Stores JWT in an HttpOnly cookie
     - Automatically forwards token to API
     - Enforces role-based UI behavior
     

   ### Alternative: Run via CLI
   Run API:
   ```bash
   cd ActivityClub.API
   dotnet run
   ```
   Run Web
   ```bash
   cd ActivityClub.Web 
   dotnet run
   ```
   
   **Suggested Test Flow:**

   Public User:
    - Browse events
    - View event details
    - See active assigned guides
    - Browse guides
    - View guides details

    Member:
     - Register
     - Login
     - Edit profile
     - Join upcoming events
     
    Admin:
     - Login as Admin
     - Manage Events (CRUD + Activation)
     - Manage Guides (CRUD + Activation)
     - View all active + inactive guides of an event
     - Assign/Unassign Guides to Events

---

## Design Principles Applied

- **Secure by default**  
  All API controllers are protected using [Authorize] unless explicitly marked [AllowAnonymous].
- **Explicit public endpoints**  
  Public access (e.g., Login, Register, public Events) is intentionally and explicitly defined.
- **Atomic registration workflow**  
  User creation + Member profile creation are wrapped in a database transaction to ensure consistency.
- **Single hashing strategy**  
  All passwords use ASP.NET Core Identity’s IPasswordHasher<User> implementation.
- **Minimal duplication**  
  Shared DTOs and layered services prevent business logic duplication.
- **No entity leakage**  
  EF Core entities are never exposed directly to clients.
  All communication happens via DTOs defined in the Contracts project.
- **Proper separation of concerns**  
   - API: Controller -> Service -> Repository
   - Web: MVC Controller -> UI Service -> API Client
   - Data access isolated from presentation logic.
- **Production-style JWT implementation**  
   - Token validation (issuer, audience, signature, expiration)
   - Role-based claims
   - Centralized authentication configuration
- **Stateless API design**  
  Authentication state is maintained via JWT; the API itself remains stateless.
- **Secure Web-to-API communication**  
   - JWT stored in HttpOnly cookie (mitigates XSS token theft risk)
   - Custom authentication handler builds HttpContext.User
   - Automatic Authorization: Bearer forwarding via HttpClient handler
- **Role-based UI enforcement**  
  Razor views render elements conditionally using User.IsInRole(...).
- **Soft activation model instead of hard deletes**  
  Events and Guides can be activated/deactivated instead of being physically deleted, preserving data integrity.
- **Clear boundary between Admin and Public data exposure**  
   - Public endpoints expose only active entities.
   - Admin endpoints provide extended visibility for management purposes.
- **Many-to-many relationship control with authorization**  
  Event–Guide assignments are managed via secured endpoints, enforcing proper administrative boundaries.


