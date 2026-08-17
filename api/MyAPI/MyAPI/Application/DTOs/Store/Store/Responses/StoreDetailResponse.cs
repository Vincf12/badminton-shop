namespace MyAPI.Application.DTOs.Store.Store.Responses;

public class StoreDetailResponse
{
    public int StoreId { get; set; }

    public string StoreCode { get; set; } = null!;

    public string StoreName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Province { get; set; }

    public string? Ward { get; set; }

    public string? AddressDetail { get; set; }

    public bool IsActive { get; set; }

    public bool IsPickupPoint { get; set; }

    public TimeOnly? OpeningTime { get; set; }

    public TimeOnly? ClosingTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
