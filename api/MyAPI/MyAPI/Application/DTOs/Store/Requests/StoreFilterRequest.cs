namespace MyAPI.Application.DTOs.Store.Requests;
public class StoreFilterRequest
{
    public string? Keyword { get; set; }

    public string? Province { get; set; }

    public string? Ward { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsPickupPoint { get; set; }

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}