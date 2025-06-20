namespace CinemaHub.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string TrailerUrl { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
        public Cinema Cinema { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<Actor> Actors { get; } = new List<Actor>();
    }
}
