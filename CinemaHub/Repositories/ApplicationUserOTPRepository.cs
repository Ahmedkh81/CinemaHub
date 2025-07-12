using CinemaHub.Data;
using CinemaHub.Models;
using CinemaHub.Repositories.IRepositories;

namespace CinemaHub.Repositories
{
    public class ApplicationUserOTPRepository : Repository<ApplicationUserOTP>, IApplicationUserOTPRepository
    {
        public ApplicationUserOTPRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
