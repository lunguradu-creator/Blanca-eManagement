using Microsoft.AspNetCore.Identity;

namespace Blanca_eManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Color { get; set; }
    }
}