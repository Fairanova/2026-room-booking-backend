namespace RoomBookingApi.Models;

public class Booking
{
    public Guid Id { get; set; }
    
    // Foreign Keys
    public Guid UserId { get; set; }
    public Guid RoomId { get; set; }
    
    // Booking Details
    public DateOnly BookingDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Purpose { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    // Status Management
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    
    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User User { get; set; } = null!;
    public Room Room { get; set; } = null!;
}
