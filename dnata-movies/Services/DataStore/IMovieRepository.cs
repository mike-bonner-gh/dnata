using Models.Data;

namespace Services.DataStore
{
    public interface IMovieRepository<T> : IRepository<T>
    {
        public IEnumerable<Movie> GetMoviesByGenre(String genre);
    }
}