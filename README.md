# Task Management System - Backend API

A task management system built with ASP.NET Core 8.0 that I designed to handle complex project workflows with proper separation of concerns and it's built with scalability and maintainability in mind.
I used Cursor + Visual Studio code, Developed a strategy with Gemini + Claude, then was able to speed up the process further by offloading simple tasks to ai agents while developing the core components + architecture / creations etc personally. 
Please refer to my "Context-referer" Spelled without the referrer "r" was just as speed typing issue that I found amusing to keep using. I used this system to help me with the development processes (I made this context referring system as if its a local MCP enabler.) Ask AI how cool it really is :D



## Why Clean Architecture?

I chose Clean Architecture because I've been burned too many times by tightly coupled codebases. When you mix business logic with framework concerns, you end up with a mess that's impossible to test and maintain.

The dependency rule is simple: dependencies point inward. The Domain layer has zero dependencies on external frameworks, which means:
- I can unit test business logic without mocking EF Core or ASP.NET
- I can swap out the database (SQL Server → PostgreSQL) without touching business logic
- I can change the UI framework without breaking the core domain

This might seem like overkill for a task management system, but trust me - when requirements change (and they always do), you'll thank me for this structure.

## Authentication: JWT vs Sessions

I went with JWT tokens over server-side sessions for a few reasons:

**Sessions suck for APIs** - They require server-side storage, which becomes a bottleneck as you scale. With JWT, the token contains everything needed to authenticate the user.

**Stateless is better** - No need to worry about session storage, session replication, or session cleanup. Each request is self-contained.

**Microservices ready** - If you ever need to split this into microservices, JWT tokens work across service boundaries without shared state.

The JWT implementation includes proper validation, configurable expiration, and environment variable support for the secret key (never hardcode secrets!).

## User Management - Why Bother?

You might wonder why I added user registration when this is "just a task manager." Here's the thing - tasks don't exist in a vacuum. They need to be assigned to someone, and that someone needs to be able to log in and see their tasks.

Without user management, you'd have to:
- Hardcode user IDs in your frontend
- Manually create users in the database
- Build user management later (which is always harder)

I built it right from the start with ASP.NET Core Identity, which handles all the security concerns like password hashing, account lockout, and email confirmation.

## Task Design Decisions

### Why Parent-Child Relationships?

Real projects aren't linear. You have main tasks that break down into sub-tasks. I could have used a simple flat structure, but that would limit the system's usefulness.

The parent-child relationship allows for:
- Complex project breakdowns
- Independent sub-task assignment
- Granular progress tracking
- Follow-up task creation

### Status Workflow

I kept the status workflow simple but extensible:
- **Open** → **In Progress** → **Completed**
- **Cancelled** for abandoned tasks

This covers 95% of use cases without overcomplicating things. If you need more statuses, just add them to the enum.

### Due Dates and Validation

I added validation to prevent past due dates because that's just bad UX. The validation happens at the service layer, not just the API layer, so it's enforced regardless of how the data gets in.

## Event-Driven Notifications

I used MediatR for the notification system because I wanted loose coupling. When a task gets assigned, the assignment logic shouldn't need to know about notifications.

**Without events**: Task assignment code would need to know about notification creation, email sending, Slack integration, etc.

**With events**: Task assignment just publishes an event. Any number of handlers can respond - create a notification, send an email, update a dashboard, whatever.

This makes the code much more maintainable and testable.

## Authorization Strategy

I implemented role-based authorization with two roles:
- **Admin**: Can do everything
- **User**: Can view tasks and manage their own

This is simple but effective. The authorization happens at the controller level with `[Authorize]` attributes, and I use custom policies for fine-grained control.

I could have gone with claims-based authorization, but for this use case, roles are simpler and more intuitive.

## Database Design

### Entity Framework Core with SQL Server

I chose EF Core because:
- It's the standard for .NET applications
- Great LINQ support
- Automatic migration generation
- Good performance with proper configuration

SQL Server LocalDB for development, but it's easy to switch to PostgreSQL or MySQL if needed.

### Repository Pattern

I implemented a generic repository pattern, which some people think is overkill with EF Core. Here's why I did it:

- **Abstraction**: Business logic doesn't depend on EF Core
- **Testability**: Easy to mock the repository for unit tests
- **Flexibility**: Can swap implementations if needed

The repository pattern gets a bad rap because people implement it wrong. I kept it simple - just basic CRUD operations with async support.

## Configuration Management

I moved sensitive configuration to environment variables and appsettings.json. The JWT secret key can be set via environment variable, which is the standard approach for containerized deployments.

I also added separate development and production configurations because you don't want to accidentally use production settings in development.

## Error Handling

I implemented comprehensive error handling because APIs without proper error responses are a nightmare to debug. Each controller method includes try-catch blocks that return appropriate HTTP status codes and meaningful error messages.

I also added validation attributes to all entities and DTOs. This catches bad data early and provides clear feedback to API consumers.

## Performance Considerations

- **Async/Await**: Everything is async to avoid blocking threads
- **EF Core Includes**: Proper eager loading to avoid N+1 queries
- **Background Services**: Notifications are processed asynchronously
- **Connection Pooling**: EF Core handles this automatically

I didn't add caching yet because premature optimization is the root of all evil. Add it when you actually need it.

## Testing Strategy

The Clean Architecture makes testing straightforward. I can unit test business logic without touching the database or web framework. The repository pattern makes it easy to mock data access.

I didn't include actual test files in this repo, but the structure is there for easy testing.

## Security Features

- **JWT Authentication**: Stateless, scalable authentication
- **Role-based Authorization**: Simple but effective access control
- **Password Policies**: Complexity requirements enforced by Identity
- **Account Lockout**: Protection against brute force attacks
- **HTTPS Enforcement**: Required in production
- **Input Validation**: Comprehensive validation on all inputs
- **SQL Injection Protection**: EF Core parameterized queries
- **XSS Protection**: ASP.NET Core built-in protections

I didn't add rate limiting or CORS configuration yet - add those based on your specific requirements.

## Getting Started

### Prerequisites

- .NET 8.0 SDK (latest LTS version)
- SQL Server LocalDB (comes with Visual Studio)
- Your favorite IDE (VS Code, Visual Studio, Rider)

### Quick Start

1. **Clone and navigate**
   ```bash
   git clone <your-repo>
   cd JaamJCSAssessment
   ```

2. **Set up the database**
   ```bash
   dotnet ef migrations add InitialCreate --project Infrastructure.Clean
   dotnet ef database update --project Infrastructure.Clean
   ```

3. **Set JWT secret** (required for authentication)
   ```bash
   set JWT_SECRET_KEY=your_secure_secret_key_at_least_32_characters_long
   ```

4. **Run the app**
   ```bash
   dotnet run
   ```

The app will start on `https://localhost:7001` (or similar). Swagger UI will be available at `/swagger` for API exploration.

### Testing the API

1. **Register a user**
   ```http
   POST /api/auth/register
   Content-Type: application/json
   
   {
     "name": "John Doe",
     "email": "john@example.com",
     "password": "SecurePass123!",
     "role": "Admin"
   }
   ```

2. **Login to get token**
   ```http
   POST /api/auth/login
   Content-Type: application/json
   
   {
     "email": "john@example.com",
     "password": "SecurePass123!"
   }
   ```

3. **Create a task** (use the token from login)
   ```http
   POST /api/tasks
   Authorization: Bearer <your-jwt-token>
   Content-Type: application/json
   
   {
     "title": "Implement Authentication",
     "description": "Add JWT authentication to the API",
     "status": "Open",
     "dueDate": "2024-02-15T10:00:00Z",
     "assigneeId": 1
   }
   ```

## API Reference

### Authentication
- `POST /api/auth/register` - Create new user account
- `POST /api/auth/login` - Authenticate and get JWT token

### Tasks
- `GET /api/tasks` - List all tasks (Admin/User)
- `GET /api/tasks/{id}` - Get specific task
- `POST /api/tasks` - Create new task (Admin only)
- `PUT /api/tasks/{id}` - Update task (Admin only)
- `DELETE /api/tasks/{id}` - Delete task (Admin only)
- `POST /api/tasks/{taskId}/assign/{userId}` - Assign task to user (Admin only)
- `GET /api/tasks/my-tasks` - Get current user's assigned tasks
- `POST /api/tasks/{parentTaskId}/subtasks` - Create sub-task (Admin only)
- `GET /api/tasks/{parentTaskId}/subtasks` - Get sub-tasks
- `POST /api/tasks/{completedTaskId}/followup` - Create follow-up task (Admin only)

### Users
- `GET /api/users` - List all users (Admin only)
- `GET /api/users/{userId}` - Get specific user
- `POST /api/users` - Create new user (Admin only)
- `GET /api/users/profile` - Get current user's profile

### Notifications
- `GET /api/notifications/user/{userId}` - Get user's notifications
- `PUT /api/notifications/{notificationId}/read` - Mark notification as read
- `GET /api/notifications/user/{userId}/unread-count` - Get unread count

## Production Deployment

### Environment Setup

1. **Set environment variables**
   ```bash
   JWT_SECRET_KEY=your_very_secure_secret_key
   ASPNETCORE_ENVIRONMENT=Production
   ```

2. **Database migration**
   ```bash
   dotnet ef database update --project Infrastructure.Clean
   ```

3. **HTTPS configuration**
   - Configure SSL certificates
   - Set `Security:RequireHttps=true` in appsettings.json

### Monitoring

Add these for production:
- Structured logging (Serilog)
- Health checks
- Application Insights
- Performance monitoring

## What's Missing (Future Enhancements)

This is a solid foundation, but here are some things I'd add for a production system:

- **Email notifications** - Send emails when tasks are assigned
- **File attachments** - Allow files to be attached to tasks
- **Task comments** - Discussion threads on tasks
- **Task templates** - Predefined task structures
- **Bulk operations** - Assign multiple tasks at once
- **Task dependencies** - Block tasks until prerequisites are done
- **Time tracking** - Log hours spent on tasks
- **Reporting** - Generate reports on task completion
- **API rate limiting** - Prevent abuse
- **Audit logging** - Track who changed what and when

## Architecture Benefits

- **Testable**: Business logic can be unit tested in isolation
- **Maintainable**: Clear separation of concerns
- **Scalable**: Easy to add new features without breaking existing code
- **Flexible**: Can swap implementations (database, authentication, etc.)
- **Secure**: Proper authentication and authorization

## Lessons Learned

Building this system taught me a few things:

1. **Start with the right architecture** - It's better to decide on architectures earlier, I made small adjustment to my design and ended up making ".Clean" versions of some heleper services as a result.
2. **Don't over-engineer** - Keep it simple until you need complexity .. KISS -> Keep it simple stupid
3. **Think about testing** - Always good to add some simple unit tests / test driven development, I thought it would be quick to just code out without these and I'm pretty sure even for something smaller like this they would have helped.


---

This system represents what I consider good practices for a .NET Web API. It's not perfect, but it's solid, maintainable, and a solid infrastructure to extend the task management system into something more elaborate like Azure Devops in the future.