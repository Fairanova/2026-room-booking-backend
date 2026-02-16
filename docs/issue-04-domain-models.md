# 📝 Laporan Progress: Issue #4 - Buat Model Domain

## ✅ Status: SELESAI

## 📋 Ringkasan

Berhasil membuat semua domain models (entities dan enums) dengan entity configurations lengkap untuk sistem peminjaman ruangan.

## 🔨 Langkah-Langkah yang Dilakukan

### 1. Membuat Enums

#### UserRole Enum

File: `src/RoomBooking.Domain/Enums/UserRole.cs`

```csharp
public enum UserRole
{
    Student = 1,
    Staff = 2,
    Admin = 3
}
```

✅ Enum untuk role-based authorization

#### BookingStatus Enum

File: `src/RoomBooking.Domain/Enums/BookingStatus.cs`

```csharp
public enum BookingStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}
```

✅ Enum untuk tracking workflow status peminjaman

### 2. Membuat Entity User

File: `src/RoomBooking.Domain/Entities/User.cs`

**Properties:**

- `Guid Id` - Primary key
- `string Username` - Username unique
- `string Email` - Email unique
- `string PasswordHash` - Hashed password
- `string FullName` - Nama lengkap user
- `UserRole Role` - Role untuk authorization
- `DateTime CreatedAt` - Timestamp creation
- `DateTime UpdatedAt` - Timestamp update
- `ICollection<Booking> Bookings` - Navigation property

✅ User entity dengan authentication fields lengkap

### 3. Membuat Entity Room

File: `src/RoomBooking.Domain/Entities/Room.cs`

**Properties:**

- `Guid Id` - Primary key
- `string RoomCode` - Kode ruangan unique (e.g., "A101")
- `string RoomName` - Nama ruangan
- `string Building` - Nama gedung
- `int Floor` - Nomor lantai
- `int Capacity` - Kapasitas orang
- `string[] Facilities` - Array fasilitas (AC, Projector, etc.)
- `bool IsActive` - Status aktif ruangan
- `DateTime CreatedAt` - Timestamp creation
- `DateTime UpdatedAt` - Timestamp update
- `ICollection<Booking> Bookings` - Navigation property

✅ Room entity dengan semua detail ruangan

### 4. Membuat Entity Booking

File: `src/RoomBooking.Domain/Entities/Booking.cs`

**Properties:**

- `Guid Id` - Primary key
- `Guid UserId` - Foreign key ke User
- `Guid RoomId` - Foreign key ke Room
- `DateOnly BookingDate` - Tanggal peminjaman
- `TimeOnly StartTime` - Jam mulai
- `TimeOnly EndTime` - Jam selesai
- `string Purpose` - Tujuan peminjaman
- `string? Description` - Deskripsi optional
- `BookingStatus Status` - Status peminjaman
- `DateTime CreatedAt` - Timestamp creation
- `DateTime UpdatedAt` - Timestamp update
- `User User` - Navigation property
- `Room Room` - Navigation property

✅ Booking entity dengan semua relasi yang diperlukan

### 5. Membuat Entity Configurations

#### UserConfiguration

File: `src/RoomBooking.Infrastructure/Data/Configurations/UserConfiguration.cs`

**Konfigurasi:**

- Primary key: `Id`
- Required fields dengan max length
- Unique index pada `Username` dan `Email`
- One-to-Many relationship dengan Booking (Cascade delete)

#### RoomConfiguration

File: `src/RoomBooking.Infrastructure/Data/Configurations/RoomConfiguration.cs`

**Konfigurasi:**

- Primary key: `Id`
- Required fields dengan max length
- `Facilities` menggunakan PostgreSQL array type (`text[]`)
- Unique index pada `RoomCode`
- One-to-Many relationship dengan Booking (Cascade delete)
- Default value `IsActive = true`

#### BookingConfiguration

File: `src/RoomBooking.Infrastructure/Data/Configurations/BookingConfiguration.cs`

**Konfigurasi:**

- Primary key: `Id`
- Required fields dengan max length
- Indexes pada `UserId`, `RoomId`, `BookingDate`, `Status` untuk performance
- Foreign key relationships

### 6. Update ApplicationDbContext

File: `src/RoomBooking.Infrastructure/Data/ApplicationDbContext.cs`

**Perubahan:**

```csharp
// Menambahkan DbSets
public DbSet<User> Users { get; set; }
public DbSet<Room> Rooms { get; set; }
public DbSet<Booking> Bookings { get; set; }

// Apply configurations
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
}
```

✅ DbContext updated dengan semua entities dan configurations

### 7. Build & Verify

```bash
dotnet build
```

**Output:**

```
Build succeeded in 1.6s
  RoomBooking.Domain net10.0 succeeded
  RoomBooking.Infrastructure net10.0 succeeded
  RoomBooking.Application net10.0 succeeded
  RoomBooking.API net10.0 succeeded
```

✅ Build BERHASIL dengan 1 warning (nullable reference - expected)

## 📂 Struktur File yang Dibuat

```
src/
├── RoomBooking.Domain/
│   ├── Entities/
│   │   ├── User.cs ✅
│   │   ├── Room.cs ✅
│   │   └── Booking.cs ✅
│   └── Enums/
│       ├── UserRole.cs ✅
│       └── BookingStatus.cs ✅
│
└── RoomBooking.Infrastructure/
    └── Data/
        ├── ApplicationDbContext.cs (updated) ✅
        └── Configurations/
            ├── UserConfiguration.cs ✅
            ├── RoomConfiguration.cs ✅
            └── BookingConfiguration.cs ✅
```

## 🎯 Entity Relationship Diagram (ERD)

```
┌──────────────┐         ┌──────────────┐
│    User      │         │    Room      │
├──────────────┤         ├──────────────┤
│ Id (PK)      │         │ Id (PK)      │
│ Username *   │         │ RoomCode *   │
│ Email *      │         │ RoomName     │
│ PasswordHash │         │ Building     │
│ FullName     │         │ Floor        │
│ Role         │         │ Capacity     │
│ CreatedAt    │         │ Facilities[] │
│ UpdatedAt    │         │ IsActive     │
└──────┬───────┘         └──────┬───────┘
       │                        │
       │  1:N                N:1│
       │                        │
       └────────┬───────────────┘
                │
        ┌───────▼────────┐
        │   Booking      │
        ├────────────────┤
        │ Id (PK)        │
        │ UserId (FK)    │
        │ RoomId (FK)    │
        │ BookingDate    │
        │ StartTime      │
        │ EndTime        │
        │ Purpose        │
        │ Description    │
        │ Status         │
        │ CreatedAt      │
        │ UpdatedAt      │
        └────────────────┘
```

## ✅ Kriteria Selesai

- [x] Enum `UserRole` dengan 3 roles (Student, Staff, Admin)
- [x] Enum `BookingStatus` dengan 4 statuses (Pending, Approved, Rejected, Cancelled)
- [x] Entity `User` dengan authentication fields dan navigation properties
- [x] Entity `Room` dengan detail lengkap termasuk array facilities
- [x] Entity `Booking` dengan foreign keys, date/time, dan status
- [x] Entity configurations dengan constraints, indexes, dan relationships
- [x] DbContext updated dengan DbSets dan configuration auto-apply
- [x] Build berhasil tanpa error

## 📝 Catatan

> **PostgreSQL Features:** Menggunakan PostgreSQL-specific features seperti `text[]` untuk array Facilities di Room entity.

> **Cascade Delete:** Relasi User dan Room ke Booking menggunakan Cascade delete, artinya jika User atau Room dihapus, semua Booking terkait akan ikut terhapus.

## 🎯 Next Steps

Lanjut ke **Issue #5: Setup Entity Framework Migrations**

- Konfigurasi DbContext (sudah selesai ✅)
- Buat migration awal
- Terapkan migration ke database
- Seed data awal (opsional)

---

**Dikerjakan pada**: 2026-02-16  
**Durasi**: ~20 menit  
**Status**: ✅ SELESAI
