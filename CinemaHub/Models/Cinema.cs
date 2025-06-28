using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace CinemaHub.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;
        public string CinemaLogo { get; set; } = string.Empty;
        [Required]
        public string Location { get; set; } = string.Empty;
        [ValidateNever]
        public ICollection<Movie> Movies { get; } = new List<Movie>();
    }
}
