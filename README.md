# Guess Number Game API

A robust, enterprise-grade REST API for the "Guess the Number" game built with .NET 9.0, featuring JWT authentication, API versioning, health checks, and comprehensive error handling.

## 🚀 Features

### Core Functionality
- **Number Guessing Game** - Interactive gameplay with difficulty levels
- **User Authentication** - JWT-based authentication and authorization
- **Game Sessions** - Track game attempts and scores
- **Leaderboard** - Real-time user rankings and statistics  (planned)

### Enterprise Features
- **API Versioning** - Multiple API versions with backward compatibility
- **Global Exception Handling** - Comprehensive error handling middleware
- **Health Checks** - Database and application health monitoring
- **Background Services** - Automated leaderboard updates  (planned)
- **Security Middleware** - CORS, rate limiting, and security headers
- **Configuration Validation** - Startup configuration validation
- **Swagger Documentation** - Interactive API documentation

## 🏗️ Architecture

### Clean Architecture Pattern
```
├── Guess.Domain/          # Core business entities and interfaces
├── Guess.Application/     # Use cases and application logic
├── Guess.Infrastructure/  # Data access and external services  
└── Guess.Api/            # Web API controllers and configuration
```

### Technology Stack
- **Framework**: .NET 9.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Documentation**: Swagger/OpenAPI
- **Validation**: FluentValidation
- **Logging**: Microsoft.Extensions.Logging
- **Testing**: MSTest (planned)

## 🛠️ Setup and Installation

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL 12+
- Visual Studio 2022 or VS Code

### Quick Start

```bash
# Clone the repository
git clone https://github.com/sarahramadan/guess-number-app.git
cd guess-number-app

# Configure database connection
# Update appsettings.Development.json with your PostgreSQL connection string

# Restore dependencies and build
dotnet restore
dotnet build

# Run the application
cd Guess.Api
dotnet run
```

### Configuration

The application uses strongly-typed configuration with validation:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=GuessNumberAppDb_Dev;Username=postgres;Password=your_password;Port=5432"
  },
  "JwtSettings": {
    "SecretKey": "YourSecretKeyMustBeAtLeast32CharactersLong",
    "Issuer": "GuessNumberApp.Dev",
    "Audience": "GuessNumberApp.Dev",
    "AccessTokenExpiryMinutes": 120,
    "RefreshTokenExpiryDays": 30
  }
}
```

## 📚 API Documentation

The API includes comprehensive Swagger documentation available at:
- **Development**: `https://localhost:5001` (Swagger UI)
- **Health Checks**: `https://localhost:5001/health`

### API Endpoints

#### Authentication
- `POST /api/v1/auth/register` - User registration
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh` - Token refresh
- `POST /api/v1/auth/logout` - User logout

#### Game
- `GET /api/v1/game` - Get user's game sessions
- `POST /api/v1/game/start` - Start new game session
- `POST /api/v1/game/guess` - Submit a guess
- `GET /api/v1/game/{id}` - Get specific game session

#### Leaderboard
- `GET /api/v1/leaderboard` - Get top scores  (planned)

#### Health Monitoring
- `GET /health` - Basic health check
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe

## 🔧 Development

### Project Structure
```
Guess.Api/
├── Configuration/        # Service configuration extensions
├── Controllers/         # API controllers (v1, v2)
├── Middleware/         # Custom middleware components
└── Services/          # Application services

Guess.Domain/
├── Entities/          # Domain entities
├── Interfaces/        # Domain interfaces
└── Common/           # Shared domain logic

Guess.Application/
├── DTOs/             # Data transfer objects
├── Interfaces/       # Application interfaces
├── Services/         # Application services
└── Validators/       # FluentValidation validators

Guess.Infrastructure/
├── Data/             # Entity Framework context
├── Repositories/     # Data access repositories
├── Services/         # Infrastructure services
└── UnitOfWork/      # Unit of Work pattern
```

### Code Quality Features
- ✅ **Global Exception Handling** - Structured error responses
- ✅ **Input Validation** - FluentValidation integration
- ✅ **Configuration Validation** - Startup validation
- ✅ **Security Headers** - CORS, HSTS, and custom headers
- ✅ **Rate Limiting** - Request throttling middleware
- ✅ **Request Logging** - Structured request/response logging
- ✅ **Health Monitoring** - Database and service health checks


## 🎨 Angular Frontend

### Overview
The frontend is a modern Angular 18+ application providing an intuitive interface for the number guessing game with responsive design and real-time user feedback.

### Technology Stack
- **Framework**: Angular 18+ with Standalone Components
- **UI Components**: Angular Material Design
- **Styling**: SCSS with responsive design
- **State Management**: RxJS and Angular Services
- **HTTP Client**: Angular HttpClient with interceptors
- **Authentication**: JWT token management
- **Build Tools**: Angular CLI with Webpack

### Frontend Features
- **Responsive Design** - Mobile-first responsive layout
- **Real-time Game Interface** - Interactive number guessing with hints
- **User Authentication** - Login/Register with JWT tokens
- **Game History** - View past games with detailed statistics
- **User Statistics** - Personal performance metrics and scores
- **Game Details Modal** - Comprehensive game session viewing
- **Automatic Logout** - Session management with token expiration

### Prerequisites
- Node.js 18+ and npm
- Angular CLI 18+

### Frontend Setup

```bash
# Navigate to frontend directory
cd frontend

# Install dependencies
npm install

# Development server (runs on http://localhost:4200)
npm start
# or
ng serve

# Build for production
ng build

# Build for development
ng build --configuration development
```

### Frontend Project Structure
```
frontend/
├── src/
│   ├── app/
│   │   ├── components/           # Feature components
│   │   │   ├── auth/            # Authentication components
│   │   │   ├── game/            # Game-related components
│   │   │   ├── history/         # Game history display
│   │   │   └── stats/           # User statistics
│   │   ├── models/              # TypeScript interfaces
│   │   ├── services/            # Angular services
│   │   ├── guards/              # Route guards
│   │   └── interceptors/        # HTTP interceptors
│   ├── assets/                  # Static assets
│   └── environments/            # Environment configurations
```

### Key Frontend Components

#### Authentication System
- **Login/Register** - User authentication with form validation
- **JWT Token Management** - Automatic token handling and refresh
- **Route Protection** - Auth guards for protected routes
- **User Menu** - Profile management and logout functionality

#### Game Interface
- **Interactive Guessing** - Number input with range validation
- **Real-time Feedback** - Hints and attempt tracking
- **Game Status** - Visual indicators for game state
- **Attempts History** - Live display of guess attempts

#### Game Management
- **Game History** - List of all played games with filtering
- **Game Details** - Modal view with comprehensive game information
- **Statistics Dashboard** - User performance metrics
- **Responsive Tables** - Mobile-friendly data display

### Frontend Configuration

#### Environment Files
```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:5001/api/v1'
};

// src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://your-api-domain.com/api/v1'
};
```

#### API Integration
- **HTTP Interceptors** - Automatic JWT token injection
- **Error Handling** - Global error handling with user notifications
- **Response Models** - TypeScript interfaces for type safety
- **Service Architecture** - Separation of concerns with dedicated services

### Development Workflow

```bash
# Start both API and Frontend
# Terminal 1 - API
cd Guess.Api
dotnet run

# Terminal 2 - Frontend
cd frontend
npm start
```

### Frontend Build and Deployment

```bash
# Production build
ng build --configuration production

# Development build
ng build --configuration development

# Serve built files
ng serve --prod
```

The built files will be in `frontend/dist/` directory, ready for deployment to any static hosting service.
