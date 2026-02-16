using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using RoomBookingApi.Models.DTOs;
using System.Security.Claims;

namespace RoomBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BookingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get list of bookings (Student sees only own, Admin/Staff sees all)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] BookingStatus? status = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var query = _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .AsQueryable();

        // Filtering based on role
        // Students can only see their own bookings
        if (userRole == UserRole.Student.ToString())
        {
            query = query.Where(b => b.UserId == Guid.Parse(userId));
        }

        // Filter by status if provided
        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        var totalItems = await query.CountAsync();
        var bookings = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BookingDto
            {
                Id = b.Id,
                UserId = b.UserId,
                UserFullName = b.User.FullName,
                RoomId = b.RoomId,
                RoomName = b.Room.RoomName,
                RoomCode = b.Room.RoomCode,
                BookingDate = b.BookingDate,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Purpose = b.Purpose,
                Status = b.Status,
                RejectionReason = b.RejectionReason,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Data = bookings
        });
    }

    /// <summary>
    /// Get booking details by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<BookingDto>> GetBooking(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var booking = await _context.Bookings
            .Include(b => b.Room)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (booking == null) return NotFound();

        // Authorization check: Student can only view own booking
        if (userRole == UserRole.Student.ToString() && booking.UserId != Guid.Parse(userId))
        {
            return Forbid(); // Or NotFound to hide existence
        }

        return Ok(new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            UserFullName = booking.User.FullName,
            RoomId = booking.RoomId,
            RoomName = booking.Room.RoomName,
            RoomCode = booking.Room.RoomCode,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Purpose = booking.Purpose,
            Status = booking.Status,
            RejectionReason = booking.RejectionReason,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        });
    }

    /// <summary>
    /// Create a new booking
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        // Validation 1: Date must be today or future
        if (dto.BookingDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return BadRequest(new { message = "Booking date cannot be in the past" });
        }

        // Validation 2: Time validation
        if (dto.StartTime >= dto.EndTime)
        {
            return BadRequest(new { message = "End time must be after start time" });
        }

        // Validation 3: Room must exist and be active
        var room = await _context.Rooms.FindAsync(dto.RoomId);
        if (room == null || !room.IsActive)
        {
            return BadRequest(new { message = "Room not found or inactive" });
        }

        // Validation 4: Check for overlapping bookings
        // Overlap condition: (StartA < EndB) AND (EndA > StartB)
        // Only consider non-Rejected/non-Cancelled bookings (Approved & Pending block the room)
        var isOverlapping = await _context.Bookings.AnyAsync(b =>
            b.RoomId == dto.RoomId &&
            b.BookingDate == dto.BookingDate &&
            b.Status != BookingStatus.Rejected && 
            b.Status != BookingStatus.Cancelled &&
            b.StartTime < dto.EndTime && 
            b.EndTime > dto.StartTime
        );

        if (isOverlapping)
        {
            return BadRequest(new { message = "Room is already booked for the selected time slot" });
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Parse(userId),
            RoomId = dto.RoomId,
            BookingDate = dto.BookingDate,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Purpose = dto.Purpose,
            Status = BookingStatus.Pending, // Default status
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Load related data for response
        await _context.Entry(booking).Reference(b => b.Room).LoadAsync();
        await _context.Entry(booking).Reference(b => b.User).LoadAsync();

        return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, new BookingDto
        {
            Id = booking.Id,
            UserId = booking.UserId,
            UserFullName = booking.User!.FullName,
            RoomId = booking.RoomId,
            RoomName = booking.Room!.RoomName,
            RoomCode = booking.Room.RoomCode,
            BookingDate = booking.BookingDate,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            Purpose = booking.Purpose,
            Status = booking.Status,
            CreatedAt = booking.CreatedAt,
            UpdatedAt = booking.UpdatedAt
        });
    }

    /// <summary>
    /// Update booking status (Approve/Reject) - Admin/Staff only
    /// </summary>
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin,Staff")]
    public async Task<IActionResult> UpdateStatus(Guid id, BookingStatusDto dto)
    {
        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        // Validation: Cannot change status of cancelled bookings
        if (booking.Status == BookingStatus.Cancelled)
        {
            return BadRequest(new { message = "Cannot change status of a cancelled booking" });
        }

        // Update status
        booking.Status = dto.Status;
        if (dto.Status == BookingStatus.Rejected)
        {
            booking.RejectionReason = dto.RejectionReason;
        }
        else
        {
            booking.RejectionReason = null; // Clear reason if approved
        }
        
        booking.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Cancel a booking
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(Guid id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var booking = await _context.Bookings.FindAsync(id);
        if (booking == null) return NotFound();

        // Authorization: 
        // - Admin can cancel any booking
        // - User can only cancel own booking IF status is Pending
        if (userRole != UserRole.Admin.ToString())
        {
            if (booking.UserId != Guid.Parse(userId))
            {
                return Forbid();
            }

            if (booking.Status != BookingStatus.Pending)
            {
                return BadRequest(new { message = "You can only cancel pending bookings" });
            }
        }

        booking.Status = BookingStatus.Cancelled;
        booking.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        return NoContent();
    }
}


// 1️⃣ Pending (Status: 1)
// Arti: Menunggu persetujuan
// Kapan: Status default saat mahasiswa/user pertama kali membuat booking ruangan
// Aksi: Admin/Staff perlu review dan approve/reject
// Contoh: Mahasiswa booking ruangan untuk diskusi kelompok → Status: Pending
// 2️⃣ Approved (Status: 2)
// Arti: Disetujui oleh Admin/Staff
// Kapan: Setelah Admin/Staff menyetujui booking yang Pending
// Aksi: Ruangan sudah terkonfirmasi boleh digunakan
// Contoh: Admin approve booking → Status: Approved
// 3️⃣ Rejected (Status: 3)
// Arti: Ditolak oleh Admin/Staff
// Kapan: Setelah Admin/Staff menolak booking (misal bentrok waktu, alasan tidak valid, dll)
// Aksi: Booking tidak bisa digunakan. RejectionReason akan berisi alasan penolakan
// Contoh: Admin reject dengan alasan "Waktu bentrok" → Status: Rejected
// 4️⃣ Cancelled (Status: 4)
// Arti: Dibatalkan oleh user sendiri
// Kapan: User membatalkan booking mereka sendiri (sebelum atau setelah approved)
// Aksi: Booking tidak jadi digunakan
// Contoh: Mahasiswa batal pakai ruangan → Status: Cancelled
