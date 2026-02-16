using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models.DTOs;

public class CreateBookingDto
{
    [Required(ErrorMessage = "Room is required")]
    public Guid RoomId { get; set; }

    [Required(ErrorMessage = "Date is required")]
    public DateOnly BookingDate { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    public TimeOnly StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    public TimeOnly EndTime { get; set; }

    [Required(ErrorMessage = "Purpose is required")]
    [StringLength(500, ErrorMessage = "Purpose cannot exceed 500 characters")]
    public string Purpose { get; set; } = string.Empty;
}
