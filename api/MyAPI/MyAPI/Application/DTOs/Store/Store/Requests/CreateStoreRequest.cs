namespace MyAPI.Application.DTOs.Store.Store.Requests;

public class CreateStoreRequest
{
    public string StoreCode { get; set; } = string.Empty;

    public string StoreName { get; set; } = string.Empty;

    public string? Phone { get; set; } = null;

    public string? Province { get; set; } = null;

    public string? Ward { get; set; } = null;

    public string? AddressDetail { get; set; } = null;
    public bool IsActive { get; set; } = true;

    public bool IsPickupPoint { get; set; } = true;

    public TimeOnly? OpeningTime { get; set; }

    public TimeOnly? ClosingTime { get; set; }
}
