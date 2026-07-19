using Microsoft.EntityFrameworkCore;

namespace MyAPI.Application.Services.Customer
{
    public class AddressService : IAddressService
    {
        private readonly AppDbContext _context;

        public AddressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AddressDto>> GetAddressesAsync(int userId)
        {
            return await _context.Addresses
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.AddressId)
                .Select(a => MapAddress(a))
                .ToListAsync();
        }

        public async Task<ServiceResult<object>> CreateAddressAsync(int userId, AddressUpsertDto dto)
        {
            if (dto.IsDefault)
            {
                await ClearDefaultAddressesAsync(userId);
            }

            var address = new Address
            {
                UserId = userId,
                RecipientName = dto.RecipientName.Trim(),
                Phone = dto.Phone.Trim(),
                AddressDetail = NormalizeOptional(dto.AddressDetail),
                Ward = NormalizeOptional(dto.Ward),
                Province = NormalizeOptional(dto.Province),
                IsDefault = dto.IsDefault
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.Ok(new
            {
                message = "Them dia chi thanh cong.",
                addressId = address.AddressId
            });
        }

        public async Task<ServiceResult<object>> UpdateAddressAsync(int userId, int id, AddressUpsertDto dto)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);
            if (address == null)
            {
                return ServiceResult<object>.NotFound("Dia chi khong ton tai.");
            }

            if (dto.IsDefault)
            {
                await ClearDefaultAddressesAsync(userId);
            }

            address.RecipientName = dto.RecipientName.Trim();
            address.Phone = dto.Phone.Trim();
            address.AddressDetail = NormalizeOptional(dto.AddressDetail);
            address.Ward = NormalizeOptional(dto.Ward);
            address.Province = NormalizeOptional(dto.Province);
            address.IsDefault = dto.IsDefault;

            await _context.SaveChangesAsync();
            return ServiceResult<object>.OK("Cap nhat dia chi thanh cong.");
        }

        public async Task<ServiceResult<object>> DeleteAddressAsync(int userId, int id)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);
            if (address == null)
            {
                return ServiceResult<object>.NotFound("Dia chi khong ton tai.");
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Xoa dia chi thanh cong.");
        }

        public async Task<ServiceResult<object>> SetDefaultAddressAsync(int userId, int id)
        {
            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == id && a.UserId == userId);
            if (address == null)
            {
                return ServiceResult<object>.NotFound("Dia chi khong ton tai.");
            }

            await ClearDefaultAddressesAsync(userId);
            address.IsDefault = true;
            await _context.SaveChangesAsync();

            return ServiceResult<object>.OK("Dat dia chi mac dinh thanh cong.");
        }

        private async Task ClearDefaultAddressesAsync(int userId)
        {
            var oldDefaults = await _context.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();

            foreach (var address in oldDefaults)
            {
                address.IsDefault = false;
            }
        }

        private static string? NormalizeOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static AddressDto MapAddress(Address address)
        {
            return new AddressDto
            {
                AddressId = address.AddressId,
                RecipientName = address.RecipientName,
                Phone = address.Phone,
                Province = address.Province,
                Ward = address.Ward,
                AddressDetail = address.AddressDetail,
                IsDefault = address.IsDefault
            };
        }
    }
}


