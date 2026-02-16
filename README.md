# 🏢 Room Booking System - Backend API

Sistem Peminjaman Ruangan Kampus menggunakan ASP.NET Core Web API dengan PostgreSQL.

## 📋 Tech Stack

- **Framework**: ASP.NET Core 10 Web API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Architecture**: Clean Architecture
- **Documentation**: Swagger/OpenAPI

## 🏗️ Project Structure

```
RoomBookingSystem/
├── src/
│   ├── RoomBooking.API/              # Web API Layer (Controllers, Middleware)
│   ├── RoomBooking.Application/      # Business Logic (Services, DTOs, Validators)
│   ├── RoomBooking.Domain/           # Domain Models (Entities, Enums)
│   └── RoomBooking.Infrastructure/   # Data Access (DbContext, Repositories)
└── tests/                            # Unit & Integration Tests (Coming soon)
```

## 📦 Current Progress

### ✅ Milestone 1: Setup Proyek & Infrastruktur

- [x] Issue #1: Inisialisasi Proyek ASP.NET Core
- [ ] Issue #2: Setup Koneksi Database PostgreSQL
- [ ] Issue #3: Konfigurasi Swagger/OpenAPI
- [ ] Issue #4: Buat Model Domain
- [ ] Issue #5: Setup Entity Framework Migrations
- [ ] Issue #6: Setup Repository Pattern

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL 14+
- Visual Studio 2022 / VS Code / Rider

### Build & Run

```bash
# Build solution
dotnet build

# Run API
dotnet run --project src/RoomBooking.API

# Access Swagger
https://localhost:5001/swagger
```

## 📝 License

This project is part of academic assignment.

---

**Last Updated**: 2026-02-16
**Status**: In Development - Milestone 1
