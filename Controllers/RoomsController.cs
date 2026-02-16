using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBookingApi.Data;
using RoomBookingApi.Models;
using RoomBookingApi.Models.DTOs;

namespace RoomBookingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RoomsController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get list of rooms with pagination and filtering
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? building = null,
        [FromQuery] int? minCapacity = null,
        [FromQuery] bool? isActive = true)
    {
        var query = _context.Rooms.AsQueryable();

        // Filtering
        if (isActive.HasValue)
        {
            query = query.Where(r => r.IsActive == isActive.Value);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.ToLower();
            query = query.Where(r => r.RoomName.ToLower().Contains(search) || 
                                     r.RoomCode.ToLower().Contains(search));
        }

        if (!string.IsNullOrEmpty(building))
        {
            query = query.Where(r => r.Building == building);
        }

        if (minCapacity.HasValue)
        {
            query = query.Where(r => r.Capacity >= minCapacity.Value);
        }

        // Pagination
        var totalItems = await query.CountAsync();
        var rooms = await query
            .OrderBy(r => r.RoomCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RoomDto
            {
                Id = r.Id,
                RoomCode = r.RoomCode,
                RoomName = r.RoomName,
                Building = r.Building,
                Floor = r.Floor,
                Capacity = r.Capacity,
                Facilities = r.Facilities,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();

        return Ok(new
        {
            TotalItems = totalItems,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            Data = rooms
        });
    }

    /// <summary>
    /// Get room details by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomDto>> GetRoom(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        return Ok(new RoomDto
        {
            Id = room.Id,
            RoomCode = room.RoomCode,
            RoomName = room.RoomName,
            Building = room.Building,
            Floor = room.Floor,
            Capacity = room.Capacity,
            Facilities = room.Facilities,
            IsActive = room.IsActive,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt
        });
    }

    /// <summary>
    /// Get available rooms for a specific time slot
    /// </summary>
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRooms(
        [FromQuery] DateOnly date,
        [FromQuery] TimeOnly startTime,
        [FromQuery] TimeOnly endTime)
    {
        // Validation
        if (startTime >= endTime)
        {
            return BadRequest(new { message = "EndTime must be after StartTime" });
        }

        // Find rooms that do NOT have overlapping bookings
        // Status: Approved and Pending bookings block the room
        var bookedRoomIds = await _context.Bookings
            .Where(b => b.BookingDate == date &&
                        (b.Status == BookingStatus.Approved || b.Status == BookingStatus.Pending) &&
                        b.StartTime < endTime && 
                        b.EndTime > startTime)
            .Select(b => b.RoomId)
            .Distinct()
            .ToListAsync();

        var availableRooms = await _context.Rooms
            .Where(r => r.IsActive && !bookedRoomIds.Contains(r.Id))
            .OrderBy(r => r.RoomCode)
            .Select(r => new RoomDto
            {
                Id = r.Id,
                RoomCode = r.RoomCode,
                RoomName = r.RoomName,
                Building = r.Building,
                Floor = r.Floor,
                Capacity = r.Capacity,
                Facilities = r.Facilities,
                IsActive = r.IsActive,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            })
            .ToListAsync();

        return Ok(availableRooms);
    }

    /// <summary>
    /// Create a new room (Admin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<RoomDto>> CreateRoom(CreateRoomDto dto)
    {
        // Check uniqueness of RoomCode
        if (await _context.Rooms.AnyAsync(r => r.RoomCode == dto.RoomCode))
        {
            return BadRequest(new { message = "Room code already exists" });
        }

        var room = new Room
        {
            Id = Guid.NewGuid(),
            RoomCode = dto.RoomCode,
            RoomName = dto.RoomName,
            Building = dto.Building,
            Floor = dto.Floor,
            Capacity = dto.Capacity,
            Facilities = dto.Facilities ?? Array.Empty<string>(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, new RoomDto
        {
            Id = room.Id,
            RoomCode = room.RoomCode,
            RoomName = room.RoomName,
            Building = room.Building,
            Floor = room.Floor,
            Capacity = room.Capacity,
            Facilities = room.Facilities,
            IsActive = room.IsActive,
            CreatedAt = room.CreatedAt,
            UpdatedAt = room.UpdatedAt
        });
    }

    /// <summary>
    /// Update an existing room (Admin only)
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRoom(Guid id, UpdateRoomDto dto)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        // Check uniqueness of RoomCode if changed
        if (room.RoomCode != dto.RoomCode && 
            await _context.Rooms.AnyAsync(r => r.RoomCode == dto.RoomCode))
        {
            return BadRequest(new { message = "Room code already exists" });
        }

        room.RoomCode = dto.RoomCode;
        room.RoomName = dto.RoomName;
        room.Building = dto.Building;
        room.Floor = dto.Floor;
        room.Capacity = dto.Capacity;
        room.Facilities = dto.Facilities ?? Array.Empty<string>();
        room.IsActive = dto.IsActive;
        room.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Delete (soft delete) a room (Admin only)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
        {
            return NotFound();
        }

        // Soft delete
        room.IsActive = false;
        room.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
