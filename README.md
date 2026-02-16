# 🏢 Room Booking API

Backend API untuk sistem peminjaman ruangan berbasis ASP.NET Core dengan PostgreSQL.

## 📋 Deskripsi

Aplikasi ini adalah REST API untuk mengelola peminjaman ruangan di institusi pendidikan. Sistem ini mendukung tiga role user (Student, Staff, Admin) dengan fitur autentikasi JWT, manajemen ruangan, dan approval workflow untuk booking.

## ✨ Fitur Utama

- 🔐 **Autentikasi & Otorisasi** - JWT Bearer authentication dengan role-based access control
- 👥 **Multi-Role System** - Student, Staff, dan Admin dengan hak akses berbeda
- 🏛️ **Manajemen Ruangan** - CRUD operations untuk data ruangan
- 📅 **Booking System** - Pembuatan booking dengan validasi overlap dan approval workflow
- ✅ **Approval Workflow** - Admin/Staff dapat approve/reject booking
- 🔍 **Search & Filter** - Pencarian ruangan, filtering, dan pagination
- 📊 **Status Tracking** - Tracking status booking (Pending, Approved, Rejected, Cancelled)

## 🛠️ Tech Stack

- **.NET 10.0** - Framework utama
- **ASP.NET Core Web API** - REST API framework
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL** - Database
- **JWT Bearer** - Authentication
- **BCrypt** - Password hashing
- **Swagger/OpenAPI** - API documentation

## 📦 Dependencies

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.0.3" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.2" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.1" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.2" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.8.1" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.3.1" />
```

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK atau .NET 9 SDK
- PostgreSQL 12+
- Git

### Installation

1. **Clone repository**

```bash
git clone https://github.com/Fairanova/2026-room-booking-backend.git
cd 2026-room-booking-backend
```

2. **Setup Database**

Buat database PostgreSQL:

```sql
CREATE DATABASE room_booking_db;
```

3. **Configure Connection String**

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=room_booking_db;Username=postgres;Password=your_password"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-characters-long",
    "Issuer": "RoomBookingApi",
    "Audience": "RoomBookingClient",
    "ExpiryInMinutes": "1440"
  }
}
```

4. **Run Migrations**

```bash
dotnet ef database update
```

5. **Run Application**

```bash
dotnet run
```

API akan berjalan di `http://localhost:5001`

### Seeded Data

Aplikasi akan otomatis seed data berikut saat pertama kali dijalankan:

**Users:**

- Admin: `username: admin`, `password: Admin123`
- Staff: `username: staff001`, `password: Staff123`
- Student: `username: student001`, `password: Student123`

**Rooms:**

- 3 ruangan contoh (Lab Komputer, Ruang Meeting, Auditorium)

## 📖 API Documentation

### Base URL

```
http://localhost:5001/api
```

### Authentication

Semua endpoint yang dilindungi memerlukan JWT token di header:

```
Authorization: Bearer {your-jwt-token}
```

---

## 🔐 Authentication Endpoints

### 1. Register

**POST** `/api/auth/register`

**Request Body:**

```json
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123!",
  "fullName": "John Doe",
  "role": 1
}
```

**Role Values:**

- `1` = Student
- `2` = Staff
- `3` = Admin

**Response:** `201 Created`

```json
{
  "id": "uuid",
  "username": "john_doe",
  "email": "john@example.com",
  "fullName": "John Doe",
  "role": 1
}
```

---

### 2. Login

**POST** `/api/auth/login`

**Request Body:**

```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Response:** `200 OK`

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "uuid",
    "username": "admin",
    "email": "admin@example.com",
    "fullName": "Administrator",
    "role": 3
  }
}
```

---

### 3. Get Profile

**GET** `/api/auth/profile`

**Headers:** `Authorization: Bearer {token}`

**Response:** `200 OK`

```json
{
  "id": "uuid",
  "username": "admin",
  "email": "admin@example.com",
  "fullName": "Administrator",
  "role": 3
}
```

---

## 🏛️ Rooms Endpoints

### 1. Get All Rooms (Public)

**GET** `/api/rooms?page=1&pageSize=10&search=Lab&location=Gedung A`

**Query Parameters:**

- `page` (optional, default: 1)
- `pageSize` (optional, default: 10, max: 50)
- `search` (optional) - Search by name or description
- `location` (optional) - Filter by location

**Response:** `200 OK`

```json
{
  "data": [
    {
      "id": "uuid",
      "name": "Lab Komputer 1",
      "location": "Gedung A Lt. 2",
      "capacity": 40,
      "description": "Lab dengan 40 komputer",
      "amenities": "Proyektor, AC, WiFi",
      "isAvailable": true
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 3,
  "totalPages": 1
}
```

---

### 2. Get Room by ID (Public)

**GET** `/api/rooms/{id}`

**Response:** `200 OK`

---

### 3. Check Room Availability (Public)

**GET** `/api/rooms/available?date=2026-02-20&startTime=09:00&endTime=11:00`

**Query Parameters:**

- `date` (required, format: YYYY-MM-DD)
- `startTime` (required, format: HH:mm)
- `endTime` (required, format: HH:mm)

**Response:** `200 OK`

```json
[
  {
    "id": "uuid",
    "name": "Ruang Meeting A",
    "location": "Gedung B Lt. 3",
    "capacity": 20
  }
]
```

---

### 4. Create Room (Admin Only)

**POST** `/api/rooms`

**Headers:** `Authorization: Bearer {admin-token}`

**Request Body:**

```json
{
  "name": "Lab Komputer 2",
  "location": "Gedung A Lt. 3",
  "capacity": 30,
  "description": "Lab programming",
  "amenities": "Proyektor, AC, WiFi"
}
```

**Response:** `201 Created`

---

### 5. Update Room (Admin Only)

**PUT** `/api/rooms/{id}`

**Headers:** `Authorization: Bearer {admin-token}`

---

### 6. Delete Room (Admin Only)

**DELETE** `/api/rooms/{id}`

**Headers:** `Authorization: Bearer {admin-token}`

**Response:** `204 No Content`

---

## 📅 Bookings Endpoints

### 1. Get Bookings

**GET** `/api/bookings?page=1&pageSize=10&status=1`

**Headers:** `Authorization: Bearer {token}`

**Query Parameters:**

- `page` (optional, default: 1)
- `pageSize` (optional, default: 10)
- `status` (optional) - Filter by status

**Access Control:**

- **Student**: Hanya booking milik sendiri
- **Admin/Staff**: Semua booking

**Response:** `200 OK`

```json
{
  "data": [
    {
      "id": "uuid",
      "bookingDate": "2026-02-20",
      "startTime": "09:00:00",
      "endTime": "11:00:00",
      "purpose": "Diskusi Kelompok",
      "description": "Diskusi project akhir",
      "status": 1,
      "rejectionReason": null,
      "createdAt": "2026-02-16T10:00:00Z",
      "room": {
        "id": "uuid",
        "name": "Lab Komputer 1",
        "location": "Gedung A Lt. 2"
      },
      "user": {
        "id": "uuid",
        "username": "student1",
        "fullName": "Student One"
      }
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 5,
  "totalPages": 1
}
```

---

### 2. Create Booking

**POST** `/api/bookings`

**Headers:** `Authorization: Bearer {token}`

**Request Body:**

```json
{
  "roomId": "uuid",
  "bookingDate": "2026-02-20",
  "startTime": "09:00",
  "endTime": "11:00",
  "purpose": "Diskusi Kelompok",
  "description": "Diskusi project akhir"
}
```

**Validations:**

- Booking date harus hari ini atau masa depan
- Start time harus sebelum end time
- Minimum durasi 30 menit
- Tidak boleh overlap dengan booking lain yang approved

**Response:** `201 Created`

---

### 3. Update Booking Status (Admin/Staff Only)

**PUT** `/api/bookings/{id}/status`

**Headers:** `Authorization: Bearer {admin-or-staff-token}`

**Request Body (Approve):**

```json
{
  "status": 2
}
```

**Request Body (Reject):**

```json
{
  "status": 3,
  "rejectionReason": "Waktu bentrok dengan kegiatan lain"
}
```

**Status Values:**

- `2` = Approved
- `3` = Rejected (wajib sertakan rejectionReason)

**Response:** `200 OK`

---

### 4. Cancel Booking

**DELETE** `/api/bookings/{id}`

**Headers:** `Authorization: Bearer {token}`

**Access Control:**

- **Student**: Hanya bisa cancel booking sendiri yang status Pending
- **Admin/Staff**: Bisa cancel booking apa saja

**Response:** `204 No Content`

---

## 📊 Booking Status Flow

```
User creates booking
        ↓
   [1: PENDING] ━━━━━━━━━━━━━━━┐
        ↓                      ↓
  Admin/Staff Review      User Cancel
        ↓                      ↓
   ┌────┴────┐          [4: CANCELLED]
   ↓         ↓
[2: APPROVED] [3: REJECTED]
```

**Status Codes:**

1. **Pending** - Menunggu approval
2. **Approved** - Disetujui oleh Admin/Staff
3. **Rejected** - Ditolak (dengan rejection reason)
4. **Cancelled** - Dibatalkan oleh user

---

## 🧪 Testing dengan Swagger

1. Buka browser ke `http://localhost:5001/swagger`
2. Login via endpoint `POST /api/auth/login`
3. Copy token dari response
4. Klik tombol **"Authorize"** (🔒 di pojok kanan atas)
5. Masukkan: `Bearer {your-token}`
6. Klik **Authorize** → **Close**
7. Test semua endpoint!

---

## 🗄️ Database Schema

### Users Table

```
- Id (UUID, PK)
- Username (string, unique)
- Email (string, unique)
- PasswordHash (string)
- FullName (string)
- Role (int: 1=Student, 2=Staff, 3=Admin)
- CreatedAt (datetime)
- UpdatedAt (datetime)
```

### Rooms Table

```
- Id (UUID, PK)
- Name (string)
- Location (string)
- Capacity (int)
- Description (string, nullable)
- Amenities (string, nullable)
- IsAvailable (bool)
- CreatedAt (datetime)
- UpdatedAt (datetime)
```

### Bookings Table

```
- Id (UUID, PK)
- UserId (UUID, FK)
- RoomId (UUID, FK)
- BookingDate (date)
- StartTime (time)
- EndTime (time)
- Purpose (string)
- Description (string, nullable)
- Status (int: 1-4)
- RejectionReason (string, nullable)
- CreatedAt (datetime)
- UpdatedAt (datetime)
```

---

## 🔒 Security

- **Password Hashing**: BCrypt dengan work factor 12
- **JWT Expiry**: Default 24 jam (configurable)
- **Role-Based Access**: Endpoint protection menggunakan `[Authorize(Roles = "...")]`
- **Input Validation**: Data Annotations di semua DTOs

---

## 📝 License

MIT License - feel free to use for educational purposes.

---

## 👤 Author

Developed by **Faira** - [GitHub](https://github.com/Fairanova)

---

## 🙏 Acknowledgments

- ASP.NET Core Documentation
- Entity Framework Core Documentation
- JWT.io for JWT debugging
