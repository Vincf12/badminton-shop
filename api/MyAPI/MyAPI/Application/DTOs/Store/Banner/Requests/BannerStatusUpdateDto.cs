using System.ComponentModel.DataAnnotations;

namespace MyAPI.Application.DTOs.Store.Banner.Requests
{
    public class BannerStatusUpdateDto
    {
        public bool IsActive { get; set; }
    }
}
