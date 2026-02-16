using RoomBookingApi.Data;
using RoomBookingApi.Models;

namespace RoomBookingApi.Data;

public static class DbSeeder
{
    public static void SeedData(ApplicationDbContext context)
    {
        // Check if data already exists
        if (context.Users.Any() || context.Rooms.Any())
        {
            return; // Database already seeded
        }

        // Seed Users
        var users = new List<User>
        {
            new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@university.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                FullName = "Administrator",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "staff001",
                Email = "staff001@university.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff123"),
                FullName = "John Staff",
                Role = UserRole.Staff,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Username = "student001",
                Email = "student001@university.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student123"),
                FullName = "Jane Student",
                Role = UserRole.Student,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();

        // Seed Rooms
        var rooms = new List<Room>
        {
            new Room
            {
                Id = Guid.NewGuid(),
                RoomCode = "A101",
                RoomName = "Ruang Kuliah A101",
                Building = "Gedung A",
                Floor = 1,
                Capacity = 50,
                Facilities = new[] { "Projector", "AC", "Whiteboard", "Sound System" },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomCode = "A201",
                RoomName = "Ruang Kuliah A201",
                Building = "Gedung A",
                Floor = 2,
                Capacity = 40,
                Facilities = new[] { "Projector", "AC", "Whiteboard" },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomCode = "B101",
                RoomName = "Lab Komputer B101",
                Building = "Gedung B",
                Floor = 1,
                Capacity = 30,
                Facilities = new[] { "Projector", "AC", "30 Computers", "Whiteboard" },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomCode = "C301",
                RoomName = "Aula C301",
                Building = "Gedung C",
                Floor = 3,
                Capacity = 100,
                Facilities = new[] { "Projector", "AC", "Sound System", "Stage", "Microphone" },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Room
            {
                Id = Guid.NewGuid(),
                RoomCode = "D201",
                RoomName = "Ruang Meeting D201",
                Building = "Gedung D",
                Floor = 2,
                Capacity = 20,
                Facilities = new[] { "TV Screen", "AC", "Whiteboard", "Video Conference" },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Rooms.AddRange(rooms);
        context.SaveChanges();

        // Seed Sample Bookings
        var bookings = new List<Booking>
        {
            new Booking
            {
                Id = Guid.NewGuid(),
                UserId = users[2].Id, // student001
                RoomId = rooms[0].Id, // A101
                BookingDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                StartTime = new TimeOnly(10, 0),
                EndTime = new TimeOnly(12, 0),
                Purpose = "Presentasi Tugas Akhir",
                Description = "Presentasi kelompok untuk mata kuliah Software Engineering",
                Status = BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Booking
            {
                Id = Guid.NewGuid(),
                UserId = users[1].Id, // staff001
                RoomId = rooms[3].Id, // C301 (Aula)
                BookingDate = DateOnly.FromDateTime(DateTime.Today.AddDays(7)),
                StartTime = new TimeOnly(13, 0),
                EndTime = new TimeOnly(16, 0),
                Purpose = "Seminar Nasional IT",
                Description = "Seminar tentang AI dan Machine Learning",
                Status = BookingStatus.Approved,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        context.Bookings.AddRange(bookings);
        context.SaveChanges();

        Console.WriteLine("Database seeded successfully!");
        Console.WriteLine($"- {users.Count} users created");
        Console.WriteLine($"- {rooms.Count} rooms created");
        Console.WriteLine($"- {bookings.Count} bookings created");
    }
}
