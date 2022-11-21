using Models.Data;

namespace Services.DataStore.FileDataStore
{
    public class MovieRepository : IRepository<Movie>
    {
        private List<Movie> movies;

        public MovieRepository() {
            this.movies = new List<Movie>();
        }

        public void Add(Movie movie) {
            this.movies.Add(movie);
        }

        public void Delete(Movie movie) {
            this.movies.RemoveAll(mv => mv.Id == movie.Id);
        }

        public IEnumerable<Movie> GetAllMovies() {
            return this.movies;
        }

        public Movie Get(Guid id) {
            if (this.movies.Where(mv => mv.Id == id).Count() == 1) {
                return this.movies.Where(mv => mv.Id == id).First();
            }
            else {
                return null;
            }
        }
        public IEnumerable<Movie> GetAll() {
            return this.movies;
        }

        public void Update(Movie movie) {
            if (this.movies.Where(mv => mv.Id == movie.Id).Count() == 0) {
                this.movies.Add(movie);

            }
            else {
                this.movies.RemoveAll(mv => mv.Id == movie.Id);
                this.movies.Add(movie);
            }
        }

        public IEnumerable<Movie> GetMoviesByGenre(String genre) {

            List<Movie> result = new List<Movie>();

            foreach (Movie movie in this.movies) {
                if (movie.ContainsGenre(genre)) {
                    result.Add(movie);
                }
            }

            return result;
        }
    }
}