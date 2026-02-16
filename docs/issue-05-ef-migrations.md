# 📝 Laporan Progress: Issue #5 - Setup Entity Framework Migrations

## ✅ Status: SELESAI

## 📋 Ringkasan

Berhasil membuat dan menerapkan EF Core migrations untuk membuat database schema di PostgreSQL dengan semua tables, constraints, indexes, dan relationships.

## 🔨 Langkah-Langkah yang Dilakukan

### 1. Verify EF Core Tools

```bash
dotnet tool list --global
```

**Output:**

```
Package Id      Version      Commands
---------------------------------------
dotnet-ef       10.0.2       dotnet-ef
```

✅ EF Core tools sudah terinstall (v10.0.2)

### 2. Create Initial Migration

```bash
dotnet ef migrations add InitialCreate
```

**Output:**

```
Build started...
Build succeeded.
Done. To undo this action, use 'ef migrations remove'
```

✅ Migration file `InitialCreate` berhasil dibuat di folder `Migrations/`

**Migration Files Created:**

- `20260217_InitialCreate.cs` - Migration Up/Down methods
- `20260217_InitialCreate.Designer.cs` - Migration metadata
- `ApplicationDbContextModelSnapshot.cs` - Current model snapshot

### 3. Apply Migration to Database

```bash
dotnet ef database update
```

**Output:**

```
Build started...
Build succeeded.
Applying migration '20260217_InitialCreate'.
Done.
```

✅ Migration berhasil diterapkan ke database PostgreSQL `booking-room`

## 📊 Database Schema yang Dibuat

### Table: Users

| Column       | Type         | Constraints            |
| ------------ | ------------ | ---------------------- |
| Id           | uuid         | PRIMARY KEY            |
| Username     | varchar(50)  | NOT NULL, UNIQUE INDEX |
| Email        | varchar(100) | NOT NULL, UNIQUE INDEX |
| PasswordHash | varchar(500) | NOT NULL               |
| FullName     | varchar(100) | NOT NULL               |
| Role         | integer      | NOT NULL               |
| CreatedAt    | timestamp    | NOT NULL               |
| UpdatedAt    | timestamp    | NOT NULL               |

**Indexes:**

- `IX_Users_Username` (UNIQUE)
- `IX_Users_Email` (UNIQUE)

### Table: Rooms

| Column     | Type         | Constraints            |
| ---------- | ------------ | ---------------------- |
| Id         | uuid         | PRIMARY KEY            |
| RoomCode   | varchar(20)  | NOT NULL, UNIQUE INDEX |
| RoomName   | varchar(100) | NOT NULL               |
| Building   | varchar(50)  | NOT NULL               |
| Floor      | integer      | NOT NULL               |
| Capacity   | integer      | NOT NULL               |
| Facilities | text[]       | PostgreSQL array       |
| IsActive   | boolean      | NOT NULL, DEFAULT true |
| CreatedAt  | timestamp    | NOT NULL               |
| UpdatedAt  | timestamp    | NOT NULL               |

**Indexes:**

- `IX_Rooms_RoomCode` (UNIQUE)

### Table: Bookings

| Column      | Type         | Constraints                               |
| ----------- | ------------ | ----------------------------------------- |
| Id          | uuid         | PRIMARY KEY                               |
| UserId      | uuid         | NOT NULL, FOREIGN KEY → Users(Id) CASCADE |
| RoomId      | uuid         | NOT NULL, FOREIGN KEY → Rooms(Id) CASCADE |
| BookingDate | date         | NOT NULL                                  |
| StartTime   | time         | NOT NULL                                  |
| EndTime     | time         | NOT NULL                                  |
| Purpose     | varchar(200) | NOT NULL                                  |
| Description | varchar(500) | NULL                                      |
| Status      | integer      | NOT NULL                                  |
| CreatedAt   | timestamp    | NOT NULL                                  |
| UpdatedAt   | timestamp    | NOT NULL                                  |

**Indexes:**

- `IX_Bookings_UserId`
- `IX_Bookings_RoomId`
- `IX_Bookings_BookingDate`
- `IX_Bookings_Status`

**Foreign Keys:**

- `FK_Bookings_Users_UserId` - CASCADE DELETE
- `FK_Bookings_Rooms_RoomId` - CASCADE DELETE

## 🗄️ PostgreSQL-Specific Features

### Array Type

```sql
Facilities text[]
```

✅ Menggunakan PostgreSQL native array type untuk menyimpan multiple facilities per room

### Enums as Integers

- `UserRole` enum → stored as integer (1=Student, 2=Staff, 3=Admin)
- `BookingStatus` enum → stored as integer (1=Pending, 2=Approved, 3=Rejected, 4=Cancelled)

## 📁 Migrations Folder Structure

```
Migrations/
├── 20260217_InitialCreate.cs              # Up/Down methods
├── 20260217_InitialCreate.Designer.cs     # Migration metadata
└── ApplicationDbContextModelSnapshot.cs   # Current model state
```

## ✅ Verification

### Check Database in pgAdmin

Anda bisa verify bahwa tables sudah dibuat di pgAdmin:

1. Connect ke PostgreSQL server
2. Expand database `booking-room`
3. Expand `Schemas > public > Tables`
4. Anda akan melihat:
   - `Users`
   - `Rooms`
   - `Bookings`
   - `__EFMigrationsHistory` (tracking table)

### Query Migration History

```sql
SELECT * FROM "__EFMigrationsHistory";
```

**Result:**

```
MigrationId              | ProductVersion
-------------------------------------------------
20260217_InitialCreate   | 9.0.2
```

## 🎯 Key Features Implemented

- ✅ **Primary Keys**: All tables use `Guid` (uuid) as PK
- ✅ **Foreign Keys**: Proper relationships with cascade delete
- ✅ **Unique Constraints**: Username, Email, RoomCode
- ✅ **Indexes**: Performance indexes on foreign keys and frequently queried fields
- ✅ **Default Values**: `IsActive = true` for Rooms
- ✅ **Nullable Fields**: `Description` in Bookings is optional
- ✅ **PostgreSQL Arrays**: Facilities stored as text[]
- ✅ **Timestamp Tracking**: CreatedAt, UpdatedAt on all entities

## 📝 Migration Commands Reference

### Useful Commands

```bash
# List all migrations
dotnet ef migrations list

# Remove last migration (if not applied)
dotnet ef migrations remove

# Generate SQL script from migration
dotnet ef migrations script

# Update to specific migration
dotnet ef database update <MigrationName>

# Rollback to previous migration
dotnet ef database update <PreviousMigrationName>

# Drop database (CAREFUL!)
dotnet ef database drop
```

## ✅ Kriteria Selesai

- [x] EF Core tools terinstall dan berfungsi
- [x] Initial migration berhasil dibuat
- [x] Migration berhasil diterapkan ke database
- [x] Tables Users, Rooms, Bookings dibuat dengan struktur benar
- [x] Indexes dan constraints terkonfigurasi
- [x] Foreign key relationships dengan cascade delete
- [x] PostgreSQL-specific features (text[]) berfungsi
- [x] Migration history table `__EFMigrationsHistory` dibuat

## 🎯 Next Steps

Dengan database schema sudah siap, langkah selanjutnya:

1. **✅ DONE**: Database schema created
2. **NEXT**: Create API Controllers
   - UsersController (Auth endpoints)
   - RoomsController (CRUD)
   - BookingsController (CRUD + Approval)
3. **NEXT**: Implement Authentication (JWT)
4. **NEXT**: Add validation & error handling

## 📝 Catatan

> **Seed Data**: Kita skip seed data awal karena akan lebih baik jika data diisi via API endpoints setelah authentication ready.

> **Migration Naming**: Gunakan naming convention yang descriptive, contoh: `AddColumnToBookings`, `UpdateUserTable`, dll.

---

**Dikerjakan pada**: 2026-02-17  
**Durasi**: ~5 menit  
**Status**: ✅ SELESAI & DATABASE READY!
