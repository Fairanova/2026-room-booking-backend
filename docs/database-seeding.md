# 📝 Database Seeding Documentation

## ✅ Seed Data Created

### 👥 Users (3)

| Username   | Email                     | Role    | Password (Plain) |
| ---------- | ------------------------- | ------- | ---------------- |
| admin      | admin@university.edu      | Admin   | Admin123         |
| staff001   | staff001@university.edu   | Staff   | Staff123         |
| student001 | student001@university.edu | Student | Student123       |

> **Note**: Passwords are hashed using BCrypt. The plain passwords above are for **testing purposes only**.

### 🏢 Rooms (5)

| Code | Name               | Building | Floor | Capacity | Facilities                                     |
| ---- | ------------------ | -------- | ----- | -------- | ---------------------------------------------- |
| A101 | Ruang Kuliah A101  | Gedung A | 1     | 50       | Projector, AC, Whiteboard, Sound System        |
| A201 | Ruang Kuliah A201  | Gedung A | 2     | 40       | Projector, AC, Whiteboard                      |
| B101 | Lab Komputer B101  | Gedung B | 1     | 30       | Projector, AC, 30 Computers, Whiteboard        |
| C301 | Aula C301          | Gedung C | 3     | 100      | Projector, AC, Sound System, Stage, Microphone |
| D201 | Ruang Meeting D201 | Gedung D | 2     | 20       | TV Screen, AC, Whiteboard, Video Conference    |

### 📅 Sample Bookings (2)

| User       | Room        | Date     | Time        | Purpose                | Status   |
| ---------- | ----------- | -------- | ----------- | ---------------------- | -------- |
| student001 | A101        | Tomorrow | 10:00-12:00 | Presentasi Tugas Akhir | Pending  |
| staff001   | C301 (Aula) | +7 days  | 13:00-16:00 | Seminar Nasional IT    | Approved |

## 🔧 Implementation

### File: `Data/DbSeeder.cs`

```csharp
public static class DbSeeder
{
    public static void SeedData(ApplicationDbContext context)
    {
        // Check if data already exists
        if (context.Users.Any() || context.Rooms.Any())
        {
            return; // Database already seeded
        }

        // Seed users, rooms, and bookings...
    }
}
```

### Integration in `Program.cs`

```csharp
// Seed database in Development environment
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created and migrations are applied
        context.Database.Migrate();

        // Seed data
        DbSeeder.SeedData(context);
    }
}
```

## ✅ Features

- **Idempotent**: Only seeds if database is empty (checks `Users` and `Rooms` tables)
- **Development Only**: Seeding only runs in Development environment
- **Auto Migration**: Ensures migrations are applied before seeding
- **Console Logging**: Outputs seeding summary to console

## 🧪 Testing Credentials

Use these credentials to test the API:

### Admin Login

```json
{
  "username": "admin",
  "password": "Admin123"
}
```

### Staff Login

```json
{
  "username": "staff001",
  "password": "Staff123"
}
```

### Student Login

```json
{
  "username": "student001",
  "password": "Student123"
}
```

## 📊 SQL Queries untuk Verify

```sql
-- Check users
SELECT "Username", "Email", "Role" FROM "Users";

-- Check rooms
SELECT "RoomCode", "RoomName", "Building", "Capacity" FROM "Rooms";

-- Check bookings
SELECT
    b."Purpose",
    u."Username",
    r."RoomCode",
    b."BookingDate",
    b."Status"
FROM "Bookings" b
JOIN "Users" u ON b."UserId" = u."Id"
JOIN "Rooms" r ON b."RoomId" = r."Id";
```

## 🔄 Re-seeding

To reseed the database:

```bash
# Drop all data
dotnet ef database drop

# Recreate and seed
dotnet ef database update
dotnet run
```

---

**Created**: 2026-02-17  
**Status**: ✅ Seeding Complete
