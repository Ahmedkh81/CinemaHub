using Microsoft.AspNetCore.Identity;

namespace CinemaHub.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}
