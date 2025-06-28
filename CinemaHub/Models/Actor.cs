using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [StringLength(700, MinimumLength = 10)]
        public string Bio { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        [Required]
        [StringLength(700, MinimumLength = 10)]
        public string News { get; set; } = string.Empty;
        [ValidateNever]
        public List<MovieActor> MovieActors { get; set; } = new();
    }
}
