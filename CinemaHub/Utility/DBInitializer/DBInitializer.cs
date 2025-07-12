using CinemaHub.Data;
using CinemaHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CinemaHub.Utility.DBInitializer
{
    public class DBInitializer : IDBInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public DBInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }

            if (_roleManager.Roles.IsNullOrEmpty())
            {
                _roleManager.CreateAsync(new(Sd.SuperAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new(Sd.Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new(Sd.Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new(Sd.Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new(Sd.Customer)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser()
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "SuperAdmin",
                    Email = "ahmedkmuhamedd@gmail.com",
                    EmailConfirmed = true,
                }, "Cinema@2025").GetAwaiter().GetResult();

                var user = _userManager.FindByNameAsync("SuperAdmin").GetAwaiter().GetResult();
                if (user != null)
                {
                    _userManager.AddToRoleAsync(user, Sd.SuperAdmin).GetAwaiter().GetResult();
                }
            }
        }
    }
}
