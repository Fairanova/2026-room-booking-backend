using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models.DTOs;

public class BookingStatusDto
{
    [Required(ErrorMessage = "Status is required")]
    public BookingStatus Status { get; set; }

    [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
    public string? RejectionReason { get; set; }
}
