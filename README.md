# 🏢 Room Booking System - Backend API

Backend API untuk sistem peminjaman ruangan kampus menggunakan ASP.NET Core Web API dengan PostgreSQL.

## 🛠️ Tech Stack

- **Framework**: ASP.NET Core Web API (.NET 10)
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 9.0
- **API Documentation**: Swagger/Swashbuckle
- **Architecture**: Monolith (Single Project)

## 📁 Struktur Proyek

```
booking-room-be/
├── Controllers/              # API Controllers
├── Data/                     # DbContext & Entity Configurations
│   └── ApplicationDbContext.cs
├── Models/                   # Domain Models (Entities & Enums)
│   ├── User.cs              # User entity
│   ├── Room.cs              # Room entity
│   ├── Booking.cs           # Booking entity
│   ├── UserRole.cs          # Enum: Student, Staff, Admin
│   └── BookingStatus.cs     # Enum: Pending, Approved, Rejected, Cancelled
├── Properties/
├── docs/                     # Dokumentasi
│   ├── issue-01-inisialisasi-project.md
│   ├── issue-02-setup-postgresql.md
│   ├── issue-04-domain-models.md
│   └── restructuring-to-monolith.md
├── Program.cs               # Application entry point
├── appsettings.json         # Configuration
├── appsettings.Development.json
└── RoomBookingApi.csproj    # Project file
```

## 📦 Installed Packages

| Package                               | Version | Purpose             |
| ------------------------------------- | ------- | ------------------- |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.2   | PostgreSQL provider |
| Microsoft.EntityFrameworkCore         | 9.0.2   | EF Core runtime     |
| Microsoft.EntityFrameworkCore.Design  | 9.0.1   | Migration tools     |
| Swashbuckle.AspNetCore                | 10.1.3  | Swagger/OpenAPI     |

## 🗄️ Database Models

### User

- **Fields**: Username, Email, PasswordHash, FullName
- **Role**: Student, Staff, Admin (enum)
- **Relationship**: One-to-Many dengan Booking

### Room

- **Fields**: RoomCode, RoomName, Building, Floor, Capacity
- **Facilities**: Array of strings (PostgreSQL text[])
- **Relationship**: One-to-Many dengan Booking

### Booking

- **Fields**: BookingDate, StartTime, EndTime, Purpose, Description
- **Status**: Pending, Approved, Rejected, Cancelled (enum)
- **Relationship**: Many-to-One dengan User dan Room

## ⚙️ Setup & Run

### Prerequisites

- .NET 10 SDK
- PostgreSQL 14+
- pgAdmin (optional)

### Database Configuration

1. Pastikan PostgreSQL sudah running
2. Buat database `booking-room` di pgAdmin atau CLI:

   ```sql
   CREATE DATABASE "booking-room";
   ```

3. Update password di `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=booking-room;Username=postgres;Password=your_password"
     }
   }
   ```

### Build & Run

```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Run application
dotnet run
```

Aplikasi akan berjalan di:

- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `https://localhost:5001/swagger`

### Create Database Schema (Migrations)

```bash
# Install EF Core tools (jika belum)
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate

# Apply migration to database
dotnet ef database update
```

## 📊 Current Progress

### ✅ Milestone 1: Setup Proyek & Infrastruktur (4/6 Completed)

- ✅ **Issue #1**: Inisialisasi Proyek ASP.NET Core
- ✅ **Issue #2**: Setup Koneksi Database PostgreSQL
- ✅ **Issue #3**: Konfigurasi Swagger/OpenAPI (Partial)
- ✅ **Issue #4**: Buat Model Domain
- ⏳ **Issue #5**: Setup Entity Framework Migrations (Next)
- ⏳ **Issue #6**: Setup Repository Pattern

### 🔜 Milestone 2: Autentikasi & Otorisasi

- [ ] JWT Authentication
- [ ] Login/Register endpoints
- [ ] Role-based authorization

### 🔜 Milestone 3: Fitur Inti - Ruangan & Peminjaman

- [ ] Room Management (CRUD)
- [ ] Booking Management (CRUD)
- [ ] Booking approval workflow
- [ ] Room availability checking

## 🔥 Quick Start Commands

```bash
# Clone repository
git clone https://github.com/Fairanova/2026-room-booking-backend.git
cd 2026-room-booking-backend

# Checkout develop branch
git checkout develop

# Restore & build
dotnet restore
dotnet build

# Create database (first time only)
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run application
dotnet run

# Open Swagger in browser
start https://localhost:5001/swagger
```

## 📝 API Endpoints (Coming Soon)

### Authentication

- `POST /api/auth/register` - Register user baru
- `POST /api/auth/login` - Login dan dapatkan JWT token

### Rooms

- `GET /api/rooms` - List semua ruangan
- `GET /api/rooms/{id}` - Detail ruangan
- `POST /api/rooms` - Buat ruangan baru (Admin only)
- `PUT /api/rooms/{id}` - Update ruangan (Admin only)
- `DELETE /api/rooms/{id}` - Hapus ruangan (Admin only)

### Bookings

- `GET /api/bookings` - List booking user
- `GET /api/bookings/{id}` - Detail booking
- `POST /api/bookings` - Buat booking baru
- `PUT /api/bookings/{id}` - Update booking
- `DELETE /api/bookings/{id}` - Cancel booking
- `POST /api/bookings/{id}/approve` - Approve booking (Admin only)
- `POST /api/bookings/{id}/reject` - Reject booking (Admin only)

## 🤝 Contributing

This project is part of academic assignment.

## 📄 License

Academic Project - 2026

---

**Repository**: [github.com/Fairanova/2026-room-booking-backend](https://github.com/Fairanova/2026-room-booking-backend)  
**Last Updated**: 2026-02-17  
**Status**: In Development - Milestone 1 (67% Complete)
