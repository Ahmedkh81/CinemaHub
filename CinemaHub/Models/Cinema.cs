namespace CinemaHub.Models
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CinemaLogo { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public ICollection<Movie> Movies { get; } = new List<Movie>();
    }
}
