namespace Models.Data
{
    public class MovieRating
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MovieId { get; set; }
        public double Rating { get; set; }
    }
}