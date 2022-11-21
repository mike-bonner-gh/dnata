namespace Models.Data
{
    public class Genre
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public Genre(Guid id, string title)
        {
            Id = id;
            Title = title;
        }
    }
}