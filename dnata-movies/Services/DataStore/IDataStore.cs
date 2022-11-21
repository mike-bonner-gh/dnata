using Models.Data;

namespace Services.DataStore
{
    public interface IDataStore
    {
        public void AddMovie(Movie movie);
        public Movie GetMovie(Guid id);
        public void UpdateMovie(Movie movie);
        public void DeleteMovie(Movie movie);
        public IEnumerable<Movie> GetAllMovies();
        public IEnumerable<MovieRating> GetAllRatings();
        public void AddUser(User user);
        public User GetUser(Guid id);
        public void UpdateUser(User user);
        public void DeleteUser(User user);
        public void Load();
        public void setRating(Guid userId, Guid movieId, int rating);
    }
}