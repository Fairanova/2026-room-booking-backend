namespace RoomBookingApi.Models.DTOs;

public class RoomDto
{
    public Guid Id { get; set; }
    public string RoomCode { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public string Building { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public string[] Facilities { get; set; } = Array.Empty<string>();
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
