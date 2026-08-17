namespace MyAPI.Application.DTOs.Store.Store.Responses;

public class StoreSummaryResponse
{
    public int StoreId { get; set; }

    public string StoreName { get; set; } = string.Empty;
    public string StoreCode { get; set; } = string.Empty;
    public string? Ward { get; set; }
    public string? Province { get; set; }
}
