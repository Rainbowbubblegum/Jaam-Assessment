---
description: Context file for JaamJCSAssessment project - Task Management System with Clean Architecture
globs: ["**/*.cs", "**/*.csproj", "**/*.sln", "**/*.json"]
alwaysApply: false
---
<SystemRule>
    ASK BEFORE ADDING FILES OR FUNCTIONALITY, SHOW CLEAR UNDERSTANDING OF WHAT DEVELOPER WANTS YOU TO DO AND CONFIRM BEFORE PROCEEDING 
</SystemRule>

# 🏗️ JaamJCSAssessment - Task Management System Context

## 📋 **PROJECT OVERVIEW**

**Project Name**: JaamJCSAssessment  
**Architecture**: Clean Architecture with ASP.NET Core 8.0  
**Purpose**: Task Management System with JWT Authentication  
**Development Approach**: AI-assisted development with context-referer system  

## 🏛️ **ARCHITECTURE PATTERNS**

### **Clean Architecture Implementation**
```
┌─────────────────────────────────────┐
│           Presentation              │  ← Controllers, DTOs
├─────────────────────────────────────┤
│           Application               │  ← Services, Handlers, Events
├─────────────────────────────────────┤
│            Domain                   │  ← Entities, Business Logic
├─────────────────────────────────────┤
│         Infrastructure             │  ← Database, External Services
└─────────────────────────────────────┘
```

**Dependency Rule**: Dependencies point inward. Domain has zero external dependencies.

### **Project Structure**
- **Domain.Clean/**: Core business entities (User, TaskItem, Notification)
- **Application.Clean/**: Application services, DTOs, events, handlers
- **Infrastructure.Clean/**: Database context, repositories, external services
- **Controllers/**: API endpoints and HTTP handling
- **AuthorizationHandlers/**: Custom authorization logic

## 🎯 **CORE ENTITIES & RELATIONSHIPS**

### **User Entity** (`Domain.Clean/User.cs`)
```csharp
public class User : IdentityUser<int>
{
    public string Name { get; set; }           // Display name
    public string Role { get; set; }           // "Admin" or "User"
    public ICollection<TaskItem> AssignedTasks { get; set; }
    public ICollection<Notification> Notifications { get; set; }
}
```

**Key Points**:
- Extends `IdentityUser<int>` for authentication
- Uses int primary keys for better performance
- Role-based authorization (Admin/User)
- Navigation properties for EF Core relationships

### **TaskItem Entity** (`Domain.Clean/TaskItem.cs`)
```csharp
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }          // Required, 1-200 chars
    public string Description { get; set; }     // Optional, max 2000 chars
    public string Status { get; set; }          // "Open", "In Progress", "Completed", "Cancelled"
    public DateTime DueDate { get; set; }       // Required, future date validation
    public int? AssigneeId { get; set; }        // Nullable foreign key
    public User? Assignee { get; set; }         // Navigation property
    public int? ParentTaskId { get; set; }      // Hierarchical structure
    public TaskItem? ParentTask { get; set; }   // Navigation property
    public ICollection<TaskItem> SubTasks { get; set; } // Child tasks
}
```

**Key Points**:
- Hierarchical task structure (parent-child relationships)
- Status workflow: Open → In Progress → Completed
- Due date validation prevents past dates
- Flexible status system (string-based, not enum)

### **Notification Entity** (`Domain.Clean/Notification.cs`)
```csharp
public class Notification
{
    public int Id { get; set; }
    public string Message { get; set; }         // Notification content
    public string Type { get; set; }            // "TaskAssigned", "TaskCompleted", etc.
    public bool IsRead { get; set; }            // Read status
    public int UserId { get; set; }             // Target user
    public User User { get; set; }              // Navigation property
    public DateTime CreatedAt { get; set; }     // Timestamp
}
```

## 🔄 **EVENT-DRIVEN PATTERNS**

### **MediatR Implementation**
- **Events**: `TaskAssignedEvent`, `TaskCompletedEvent`
- **Handlers**: `TaskAssignedEventHandler`, `TaskCompletedEventHandler`
- **Purpose**: Loose coupling between business logic and notifications

### **Event Flow**
1. Business operation occurs (task assignment, completion)
2. Event is published via MediatR
3. Handlers respond (create notifications, send emails, etc.)
4. No direct coupling between business logic and side effects

## 🛡️ **AUTHENTICATION & AUTHORIZATION**

### **JWT Authentication**
- **Stateless**: No server-side session storage
- **Scalable**: Works across microservices
- **Secure**: Environment variable for secret key
- **Configurable**: Expiration, validation, etc.

### **Authorization Strategy**
- **Role-based**: Admin and User roles
- **Controller-level**: `[Authorize]` attributes
- **Custom policies**: Fine-grained control
- **Resource-based**: Custom authorization handlers

## 🗄️ **DATA ACCESS PATTERNS**

### **Repository Pattern**
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

**Benefits**:
- Abstraction over EF Core
- Easy unit testing with mocks
- Flexibility to swap implementations

### **Entity Framework Core**
- **SQL Server LocalDB** for development
- **Code-first** approach with migrations
- **Eager loading** with Include() to avoid N+1 queries
- **Async/await** throughout for performance

## 📊 **SERVICE LAYER PATTERNS**

### **Application Services**
- **ITaskService**: Task business logic
- **IUserService**: User management
- **IAuthService**: Authentication operations
- **INotificationService**: Notification handling

### **DTO Pattern**
- **Input DTOs**: `CreateTaskDto`, `CreateUserDto`, `LoginDto`
- **Response DTOs**: `AuthResponseDto`
- **Purpose**: Separate API contracts from domain entities

## 🔧 **DEVELOPMENT PATTERNS**

### **Validation Strategy**
- **Data Annotations**: Entity-level validation
- **Service Layer**: Business rule validation
- **Controller**: API-level validation
- **Comprehensive**: Multiple layers of validation

### **Error Handling**
- **Try-catch blocks**: Controller-level exception handling
- **HTTP Status Codes**: Appropriate responses
- **Meaningful Messages**: Clear error feedback
- **Validation**: Early error detection

### **Configuration Management**
- **Environment Variables**: Sensitive data (JWT secret)
- **appsettings.json**: Application configuration
- **Development/Production**: Separate configurations
- **Security**: Never hardcode secrets

## 🚀 **PERFORMANCE CONSIDERATIONS**

### **Async/Await Pattern**
- **All operations**: Database, HTTP, file I/O
- **Non-blocking**: Avoid thread pool exhaustion
- **Scalable**: Better resource utilization

### **Database Optimization**
- **Eager Loading**: Include() for related data
- **Connection Pooling**: EF Core handles automatically
- **Indexing**: Proper database indexes
- **Query Optimization**: Avoid N+1 queries

## 🧪 **TESTING STRATEGY**

### **Clean Architecture Benefits**
- **Unit Testing**: Business logic in isolation
- **Mocking**: Easy to mock dependencies
- **Repository Pattern**: Mock data access
- **Service Layer**: Test business rules

### **Test Structure** (Not included but designed for)
- **Domain Tests**: Business logic validation
- **Service Tests**: Application layer logic
- **Integration Tests**: Database operations
- **API Tests**: End-to-end functionality

## 🔒 **SECURITY FEATURES**

### **Authentication**
- **JWT Tokens**: Stateless authentication
- **Password Policies**: ASP.NET Identity handles
- **Account Lockout**: Brute force protection
- **HTTPS**: Required in production

### **Authorization**
- **Role-based**: Admin/User roles
- **Resource-based**: Custom authorization handlers
- **Fine-grained**: Controller and action level
- **Secure by Default**: Explicit permissions required

## 📈 **SCALABILITY CONSIDERATIONS**

### **Architecture Benefits**
- **Microservices Ready**: JWT works across services
- **Database Agnostic**: Easy to switch providers
- **Framework Independent**: Domain has no framework dependencies
- **Event-driven**: Loose coupling for scaling

### **Performance Patterns**
- **Async Operations**: Non-blocking throughout
- **Background Services**: Notification processing
- **Connection Pooling**: Database optimization
- **Stateless Design**: Horizontal scaling ready

## 🎯 **DEVELOPMENT WORKFLOW**

### **AI-Assisted Development**
- **Context-Referer System**: Local MCP-like functionality
- **Pattern Recognition**: Consistent architecture patterns
- **Code Generation**: AI helps with repetitive tasks
- **Quality Assurance**: AI assists with best practices

### **Code Organization**
- **Clean Architecture**: Clear separation of concerns
- **Consistent Naming**: Clear, descriptive names
- **Documentation**: XML comments on public APIs
- **SOLID Principles**: Applied throughout

## 🔄 **API ENDPOINTS**

### **Authentication**
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - JWT token generation

### **Tasks**
- `GET /api/tasks` - List all tasks
- `POST /api/tasks` - Create new task (Admin only)
- `PUT /api/tasks/{id}` - Update task (Admin only)
- `DELETE /api/tasks/{id}` - Delete task (Admin only)
- `POST /api/tasks/{taskId}/assign/{userId}` - Assign task
- `GET /api/tasks/my-tasks` - User's assigned tasks
- `POST /api/tasks/{parentTaskId}/subtasks` - Create sub-task
- `GET /api/tasks/{parentTaskId}/subtasks` - Get sub-tasks
- `POST /api/tasks/{completedTaskId}/followup` - Create follow-up

### **Users**
- `GET /api/users` - List all users (Admin only)
- `GET /api/users/{userId}` - Get specific user
- `POST /api/users` - Create new user (Admin only)
- `GET /api/users/profile` - Current user profile

### **Notifications**
- `GET /api/notifications/user/{userId}` - User notifications
- `PUT /api/notifications/{notificationId}/read` - Mark as read
- `GET /api/notifications/user/{userId}/unread-count` - Unread count

## 🎨 **DESIGN PRINCIPLES**

### **KISS (Keep It Simple, Stupid)**
- **Simple Status Workflow**: Open → In Progress → Completed
- **Basic Roles**: Admin and User only
- **Clear Relationships**: Straightforward entity relationships
- **Minimal Complexity**: Add complexity only when needed

### **SOLID Principles**
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Extensible through events and handlers
- **Liskov Substitution**: Proper inheritance hierarchies
- **Interface Segregation**: Focused interfaces
- **Dependency Inversion**: Dependencies point inward

### **Clean Code Practices**
- **Meaningful Names**: Clear, descriptive identifiers
- **Small Functions**: Single purpose, readable methods
- **Comments**: XML documentation on public APIs
- **Consistent Formatting**: Standard C# conventions

## 🚀 **DEPLOYMENT CONSIDERATIONS**

### **Environment Setup**
- **JWT_SECRET_KEY**: Environment variable required
- **Database Migration**: EF Core migrations
- **HTTPS**: Required in production
- **Environment Variables**: Configuration management

### **Production Readiness**
- **Structured Logging**: Add Serilog
- **Health Checks**: Application monitoring
- **Performance Monitoring**: Application Insights
- **Security Headers**: HTTPS enforcement

## 🔮 **FUTURE ENHANCEMENTS** For reference to make sure anything we add is Open to extension and will support these future features.

### **Planned Features**
- **Email Notifications**: Send emails on task events
- **File Attachments**: Attach files to tasks
- **Task Comments**: Discussion threads
- **Task Templates**: Predefined structures
- **Bulk Operations**: Multi-task management
- **Task Dependencies**: Prerequisite relationships
- **Time Tracking**: Hours logging
- **Reporting**: Completion analytics
- **API Rate Limiting**: Abuse prevention
- **Audit Logging**: Change tracking

### **Architecture Extensions**
- **Caching**: Redis for performance
- **Message Queues**: Background processing
- **Microservices**: Service decomposition
- **Event Sourcing**: Complete audit trail
- **CQRS**: Command/Query separation

## 🎯 **DEVELOPMENT GUIDELINES**

### **When Adding Features**
1. **Start with Domain**: Define entities and business rules
2. **Add Application Services**: Business logic and DTOs
3. **Implement Infrastructure**: Database and external services
4. **Create Controllers**: API endpoints
5. **Add Tests**: Unit and integration tests

### **When Modifying Code**
1. **Follow Clean Architecture**: Respect dependency direction
2. **Use Events**: For side effects and notifications
3. **Validate Input**: Multiple layers of validation
4. **Handle Errors**: Comprehensive error handling
5. **Document Changes**: Update XML comments

### **When Debugging**
1. **Check Domain Logic**: Business rules first
2. **Verify Data Flow**: Entity → Service → Controller
3. **Review Validation**: Input and business rule validation
4. **Examine Events**: Event-driven side effects
5. **Test API Endpoints**: Use Swagger UI

## 🏆 **SUCCESS METRICS**

### **Code Quality**
- **Clean Architecture**: Proper dependency direction
- **Test Coverage**: Comprehensive unit tests
- **Documentation**: Clear API documentation
- **Performance**: Async operations and optimization

### **User Experience**
- **API Consistency**: Standard HTTP patterns
- **Error Handling**: Clear error messages
- **Validation**: Early error detection
- **Security**: Proper authentication/authorization

### **Maintainability**
- **Separation of Concerns**: Clear layer boundaries
- **Event-Driven**: Loose coupling
- **Repository Pattern**: Data access abstraction
- **SOLID Principles**: Extensible design

---

**Remember**: This system is designed for scalability, maintainability, and clean code practices. Always follow the established patterns and architecture principles when making changes or adding features. 