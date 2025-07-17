namespace CinemaHub.Models
{
    public class MovieSchedule
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public DateTime ShowTime { get; set; }
        public ICollection<Cart> Carts { get; set; }
    }
}
