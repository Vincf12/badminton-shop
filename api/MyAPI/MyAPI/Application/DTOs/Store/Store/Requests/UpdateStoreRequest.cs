namespace MyAPI.Application.DTOs.Store.Store.Requests;

public class UpdateStoreRequest
{
    public string StoreName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Province { get; set; }

    public string? Ward { get; set; }

    public string? AddressDetail { get; set; }

    public bool IsPickupPoint { get; set; }

    public bool IsActive { get; set; }

    public TimeOnly? OpeningTime { get; set; }

    public TimeOnly? ClosingTime { get; set; }
}
