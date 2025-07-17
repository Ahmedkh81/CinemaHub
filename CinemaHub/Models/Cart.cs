using Microsoft.EntityFrameworkCore;

namespace CinemaHub.Models
{
    [PrimaryKey(nameof(ApplicationUserId), nameof(MovieId), nameof(MovieScheduleId))]
    public class Cart
    {
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public int MovieScheduleId { get; set; } 
        public MovieSchedule MovieSchedule { get; set; } 
        public int Count { get; set; }
    }
}
