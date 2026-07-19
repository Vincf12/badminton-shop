
namespace MyAPI.Application.Interfaces.Commerce
{
    public interface IAddressService
    {
        Task<IEnumerable<AddressDto>> GetAddressesAsync(int userId);
        Task<ServiceResult<object>> CreateAddressAsync(int userId, AddressUpsertDto dto);
        Task<ServiceResult<object>> UpdateAddressAsync(int userId, int id, AddressUpsertDto dto);
        Task<ServiceResult<object>> DeleteAddressAsync(int userId, int id);
        Task<ServiceResult<object>> SetDefaultAddressAsync(int userId, int id);
    }
}


