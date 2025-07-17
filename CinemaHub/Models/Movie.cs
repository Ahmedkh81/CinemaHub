using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(700, MinimumLength = 20)]
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        [Required]
        public string TrailerUrl { get; set; } = string.Empty;
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public int? AvailableTickets { get; set; }
        public MovieStatus MovieStatus
        {
            get
            {
                var now = DateTime.Now;

                if (now < StartDate)
                    return MovieStatus.Upcoming;
                else if (now >= StartDate && now <= EndDate)
                    return MovieStatus.Available;
                else
                    return MovieStatus.Expired;
            }
        }
        public int CinemaId { get; set; }
        public int CategoryId { get; set; }
        [ValidateNever]
        public Cinema Cinema { get; set; } = null!;
        [ValidateNever]
        public Category Category { get; set; } = null!;
        [ValidateNever]
        public List<MovieActor> MovieActors { get; set; } = new();
        public ICollection<MovieSchedule> MovieSchedules { get; set; } = new List<MovieSchedule>();
    }
}
