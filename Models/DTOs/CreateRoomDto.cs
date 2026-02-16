using System.ComponentModel.DataAnnotations;

namespace RoomBookingApi.Models.DTOs;

public class CreateRoomDto
{
    [Required(ErrorMessage = "Room code is required")]
    [StringLength(20, ErrorMessage = "Room code cannot exceed 20 characters")]
    public string RoomCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Room name is required")]
    [StringLength(100, ErrorMessage = "Room name cannot exceed 100 characters")]
    public string RoomName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Building is required")]
    [StringLength(50, ErrorMessage = "Building cannot exceed 50 characters")]
    public string Building { get; set; } = string.Empty;

    [Required(ErrorMessage = "Floor is required")]
    [Range(1, 100, ErrorMessage = "Floor must be greater than 0")]
    public int Floor { get; set; }

    [Required(ErrorMessage = "Capacity is required")]
    [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
    public int Capacity { get; set; }

    public string[]? Facilities { get; set; }
}
