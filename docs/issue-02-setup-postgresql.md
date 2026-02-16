# 📝 Laporan Progress: Issue #2 - Setup Koneksi Database PostgreSQL

## ✅ Status: SELESAI

## 📋 Ringkasan

Berhasil mengkonfigurasi koneksi database PostgreSQL dengan Entity Framework Core dan menyelesaikan dependency injection ke aplikasi ASP.NET Core.

## 🔨 Langkah-Langkah yang Dilakukan

### 1. Install Packages Yang Diperlukan

**Npgsql Entity Framework Core Provider:**

```bash
dotnet add src/RoomBooking.Infrastructure/RoomBooking.Infrastructure.csproj package Npgsql.EntityFrameworkCore.PostgreSQL
```

✅ Package Version: `9.0.2`

**EF Core Design (untuk Migrations):**

```bash
dotnet add src/RoomBooking.Infrastructure/RoomBooking.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design
```

✅ Package Version: `9.0.1`

**Swagger/OpenAPI:**

```bash
dotnet add src/RoomBooking.API/RoomBooking.API.csproj package Swashbuckle.AspNetCore
```

✅ Package Version: `10.1.3`

**EF Core di API (untuk mengatasi version conflict):**

```bash
dotnet add src/RoomBooking.API/RoomBooking.API.csproj package Microsoft.EntityFrameworkCore --version 9.0.2
```

✅ Package Version: `9.0.2`

### 2. Membuat ApplicationDbContext

File: `src/RoomBooking.Infrastructure/Data/ApplicationDbContext.cs`

```csharp
using Microsoft.EntityFrameworkCore;

namespace RoomBooking.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSet untuk entities akan ditambahkan nanti

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Entity configurations akan ditambahkan nanti
    }
}
```

✅ DbContext berhasil dibuat dengan struktur dasar

### 3. Konfigurasi Connection String

**File: `src/RoomBooking.API/appsettings.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=booking-room;Username=postgres;Password=your_password_here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**File: `src/RoomBooking.API/appsettings.Development.json`**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=booking-room;Username=postgres;Password=YourActualPassword"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

✅ Connection string terkonfigurasi untuk database `booking-room`
✅ Logging untuk EF Core diaktifkan di Development environment

### 4. Setup Dependency Injection di Program.cs

**File: `src/RoomBooking.API/Program.cs`**

```csharp
using Microsoft.EntityFrameworkCore;
using RoomBooking.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure PostgreSQL Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

✅ DbContext registered di DI container
✅ Swagger/OpenAPI terkonfigurasi
✅ Default weather forecast code dihapus

### 5. Mengatasi Package Version Conflicts

**Masalah yang Ditemui:**

```
error CS1705: Assembly 'RoomBooking.Infrastructure' uses 'Microsoft.EntityFrameworkCore, Version=9.0.1.0' which has a higher version than referenced assembly 'Microsoft.EntityFrameworkCore, Version=9.0.0.0'
```

**Solusi:**

1. Downgrade EF Core dari 10.0.x ke 9.0.x (lebih stable untuk .NET 10)
2. Hapus package `Microsoft.AspNetCore.OpenApi` yang conflict dengan Swashbuckle
3. Tambah explicit reference `Microsoft.EntityFrameworkCore` v9.0.2 di API project
4. Clean dan rebuild solution

**Commands yang Dijalankan:**

```bash
dotnet clean
dotnet restore
dotnet build
```

✅ Build BERHASIL tanpa error

### 6. Verifikasi Build

```bash
dotnet build
```

**Output:**

```
Build succeeded in 4.8s
  RoomBooking.Domain net10.0 succeeded
  RoomBooking.Infrastructure net10.0 succeeded
  RoomBooking.Application net10.0 succeeded
  RoomBooking.API net10.0 succeeded
```

✅ Semua 4 projects berhasil di-build

## 📦 Package Versions Final

| Package                               | Version | Project        |
| ------------------------------------- | ------- | -------------- |
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.2   | Infrastructure |
| Microsoft.EntityFrameworkCore.Design  | 9.0.1   | Infrastructure |
| Microsoft.EntityFrameworkCore         | 9.0.2   | API            |
| Swashbuckle.AspNetCore                | 10.1.3  | API            |

## 🗄️ Database Configuration

- **Database Name:** `booking-room`
- **Host:** `localhost`
- **Port:** `5432`
- **Username:** `postgres`
- **Password:** (user perlu set di appsettings.Development.json)

## ✅ Kriteria Selesai

- [x] Npgsql.EntityFrameworkCore.PostgreSQL package terinstall
- [x] ApplicationDbContext dibuat di Infrastructure layer
- [x] Connection string terkonfigurasi di appsettings.json
- [x] DbContext registered di DI container dengan UseNpgsql
- [x] Build berhasil tanpa error
- [x] Swagger terkonfigurasi dan siap digunakan

## 📝 Catatan

> **PENTING:** User perlu update password di `appsettings.Development.json` dengan password PostgreSQL yang sebenarnya sebelum menjalankan aplikasi.

## 🎯 Next Steps

Lanjut ke **Issue #3: Konfigurasi Swagger/OpenAPI** (Sebagian sudah selesai - Swashbuckle sudah terinstall dan terkonfigurasi)

Atau lanjut ke **Issue #4: Buat Model Domain**

- Buat entity User
- Buat entity Room
- Buat entity Booking
- Buat enum (UserRole, BookingStatus)

---

**Dikerjakan pada**: 2026-02-16  
**Durasi**: ~30 menit  
**Status**: ✅ SELESAI
