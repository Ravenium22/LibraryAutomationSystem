# 📚 Kütüphane Yönetim Sistemi - Backend

> **Internship Learning Project** - A comprehensive library management system backend built with ASP.NET Core to demonstrate modern web API development practices.

## 🎯 Project Goals

This project was developed as part of an internship learning path to gain hands-on experience with:

- **Modern Web API Development** using ASP.NET Core 8
- **Database Design** and Entity Framework Core implementation
- **Authentication & Authorization** with JWT tokens
- **Repository Pattern** and clean architecture principles
- **RESTful API Design** following industry best practices
- **Logging & Monitoring** with Serilog integration
- **API Documentation** using Swagger/OpenAPI

## 🛠️ Tech Stack

### **Framework & Runtime**
- **ASP.NET Core 8.0** - Modern web framework for building APIs
- **.NET 8** - Latest LTS version of .NET

### **Database & ORM**
- **SQL Server 2022** - Relational database management system
- **Docker** - Ran SQL on docker
- **Entity Framework Core 9.0** - Object-relational mapping (ORM)
- **Code-First Migrations** - Database schema management

### **Authentication & Security**
- **JWT Bearer Tokens** - Stateless authentication
- **ASP.NET Core Identity concepts** - User management patterns
- **Role-based Authorization** - Admin/User access control
- **Password Hashing** - HMACSHA256 secure password storage

### **Architecture & Patterns**
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Built-in IoC container
- **Clean Architecture** - Separation of concerns
- **DTO Pattern** - Data transfer objects for API responses

### **Logging & Monitoring**
- **Serilog** - Structured logging framework
- **Console & File Sinks** - Multiple logging outputs
- **Seq Integration** - Centralized log management

### **API Documentation**
- **Swagger/OpenAPI 3.0** - Interactive API documentation
- **XML Documentation** - Code comments for API endpoints

### **Development Tools**
- **Visual Studio Code** - Primary IDE
- **SQL Server Management Studio** - Database management
- **Postman** - API testing during development

## 🏗️ Architecture Overview

```
📁 Kutuphane.Backend/
├── 📁 Controllers/           # API endpoints and request handling
├── 📁 Models/               # Entity models and DTOs
├── 📁 Data/                 # DbContext and database configuration
├── 📁 Repositories/         # Data access layer with interfaces
├── 📁 Services/             # Business logic and JWT services
├── 📁 Migrations/           # EF Core database migrations
└── 📄 Program.cs           # Application startup and configuration
```

## 📊 Database Schema

### **Core Entities**
- **Kullanici** (Users) - User management with roles
- **Kitap** (Books) - Book catalog with stock tracking
- **Yazar** (Authors) - Author information
- **Kategori** (Categories) - Book categorization
- **Odunc** (Loans) - Borrowing transactions with late fees

### **Key Features**
- **Stock Management** - Available vs total stock tracking
- **Late Fee Calculation** - Automatic penalty calculation (50₺/day)
- **Loan Status Tracking** - Real-time borrowing status
- **User Role Management** - Admin and regular user permissions

## 🚀 API Endpoints

### **Authentication**
- `POST /api/Auth/login` - User authentication
- `POST /api/Auth/register` - User registration

### **Books Management**
- `GET /api/Kitap` - List all books
- `GET /api/Kitap/{id}` - Get book details
- `POST /api/Kitap/admin/create-kitap` - Add new book (Admin)
- `PUT /api/Kitap/admin/update-kitap/{id}` - Update book (Admin)
- `DELETE /api/Kitap/admin/delete-kitap/{id}` - Delete book (Admin)
- `GET /api/Kitap/search` - Search books

### **Loan Management**
- `GET /api/Odunc` - List all loans
- `POST /api/Odunc/admin/odunc-ver` - Create loan (Admin)
- `POST /api/Odunc/admin/{id}/iade` - Process return (Admin)
- `GET /api/Odunc/suresi-dolan` - Get overdue loans
- `GET /api/Odunc/admin/borç-raporu` - Debt report (Admin)

### **Dashboard & Analytics**
- `GET /api/Dashboard/public/stats` - Public statistics
- `GET /api/Dashboard/admin/full` - Full admin dashboard (Admin)
- `GET /api/Dashboard/public/popular-books` - Popular books
- `GET /api/Dashboard/public/trends` - Category trends

*[Complete API documentation available via Swagger UI]*

## 🎓 Learning Outcomes & Skills Gained

### **Backend Development Skills**
- ✅ **RESTful API Design** - Created 40+ endpoints following REST principles
- ✅ **Entity Framework Core** - Database modeling, migrations, and querying
- ✅ **Authentication Systems** - JWT implementation with role-based access
- ✅ **Repository Pattern** - Clean separation of data access logic
- ✅ **Error Handling** - Comprehensive exception handling and logging
- ✅ **API Versioning** - Prepared for future API versions
- ✅ **CORS Configuration** - Cross-origin resource sharing setup

### **Database Skills**
- ✅ **Relational Database Design** - Normalized schema with proper relationships
- ✅ **Complex Queries** - Advanced LINQ queries for reporting
- ✅ **Performance Optimization** - Database indexing strategies
- ✅ **Data Validation** - Entity validation and business rules
- ✅ **Migration Management** - Schema versioning and updates

### **Security & Best Practices**
- ✅ **Secure Authentication** - JWT token implementation
- ✅ **Password Security** - Proper hashing and salting
- ✅ **Authorization Patterns** - Role-based access control
- ✅ **Input Validation** - DTO validation and sanitization
- ✅ **HTTPS Configuration** - SSL/TLS setup

### **DevOps & Monitoring**
- ✅ **Structured Logging** - Serilog implementation with multiple sinks
- ✅ **Health Checks** - Application monitoring endpoints
- ✅ **Configuration Management** - Environment-based settings
- ✅ **Dependency Injection** - IoC container usage
- ✅ **API Documentation** - Swagger integration

## 📈 Project Achievements

### **Technical Accomplishments**
- 🎯 **40+ API Endpoints** implemented with full CRUD operations
- 🎯 **5 Core Entities** with complex relationships and business logic
- 🎯 **JWT Authentication** with role-based authorization
- 🎯 **Advanced Reporting** including overdue books and financial data
- 🎯 **Real-time Calculations** for late fees and stock management
- 🎯 **Comprehensive Logging** for debugging and monitoring

### **Learning Milestones**
- 🏆 **Mastered ASP.NET Core** pipeline and middleware
- 🏆 **Database First Approach** with Entity Framework Core
- 🏆 **Clean Architecture** implementation
- 🏆 **API Testing** with Postman and Swagger
- 🏆 **Error Handling** and exception management
- 🏆 **Performance Optimization** techniques

## 🚀 Getting Started

### **Prerequisites**
- .NET 8 SDK
- SQL Server 2019+
- Visual Studio Code or Visual Studio 2022



*This project was developed as part of an internship learning program to gain practical experience with enterprise-level web API development using the .NET ecosystem.*