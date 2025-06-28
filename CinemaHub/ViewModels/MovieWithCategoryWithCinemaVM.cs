using CinemaHub.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CinemaHub.ViewModels
{
    public class MovieWithCategoryWithCinemaVM
    {
        public List<Category> Categories { get; set; } = null!;
        public List<Cinema> Cinemas { get; set; } = null!;
        public List<SelectListItem>? Actors { get; set; }
        public Movie Movie { get; set; } = null!;
    }
}
