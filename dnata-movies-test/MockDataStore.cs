using Models.Data;
using Services.DataStore;

namespace dnata_movies_test
{
    internal class MockDataStore : IDataStore
    {
        private List<User> users;
        private List<Movie> movies;
        private List<Genre> genres;
        private List<MovieRating> movieRatings;

        public MockDataStore() {
            this.users = new List<User>();
            this.movies = new List<Movie>();
            this.genres = new List<Genre>();
            this.movieRatings = new List<MovieRating>();
        }

        public void Load() {
        }

        public void AddMovie(Movie movie) {
            this.movies.Add(movie);
        }

        public Movie GetMovie(Guid id) {
            if (this.movies.Where(mv => mv.Id == id).Count() == 1) {
                return this.movies.Where(mv => mv.Id == id).First();
            }
            else {
                return null;
            }
        }

        public void UpdateMovie(Movie movie) {
            if (this.movies.Where(mv => mv.Id == movie.Id).Count() == 0) {
                this.movies.Add(movie);

            }
            else {
                this.movies.RemoveAll(mv => mv.Id == movie.Id);
                this.movies.Add(movie);
            }
        }

        public void DeleteMovie(Movie movie) {
            this.movies.RemoveAll(mv => mv.Id == movie.Id);
        }

        public IEnumerable<Movie> GetAllMovies() {
            return movies;
        }

        public void AddUser(User user) {
            this.users.Add(user);
        }

        public User GetUser(Guid id) {
            if (this.users.Where(us => us.Id == id).Count() == 1) {
                return this.users.Where(us => us.Id == id).First();
            }
            else {
                return null;
            }
        }

        public void UpdateUser(User user) {
            if (this.users.Where(us => us.Id == user.Id).Count() == 0) {
                this.users.Add(user);

            }
            else {
                this.users.RemoveAll(us => us.Id == user.Id);
                this.users.Add(user);
            }
        }

        public void DeleteUser(User user) {
            this.users.RemoveAll(us => us.Id == user.Id);
        }

        public void AddMovieRating(MovieRating rating) {
            this.movieRatings.Add(rating);
        }

        public void UpdateMovieRating(MovieRating rating) {
            if (this.movieRatings.Where(r => r.Id == rating.Id).Count() == 0) {
                this.movieRatings.Add(rating);

            }
            else {
                this.movieRatings.RemoveAll(r => r.Id == r.Id);
                this.movieRatings.Add(rating);
            }
        }

        public List<MovieRating> GetTop5MoviesByUser(Guid userId) {
            return this.movieRatings.Where(mr => mr.UserId == userId).OrderByDescending(mr => mr.Rating).Take(5).ToList();
        }

        public List<MovieRating> GetTop5Movies() {
            List<MovieRating> result = new List<MovieRating>();

            var ratings = from r in this.movieRatings
                          group r by r.MovieId into grp
                          select new {
                              MovieId = grp.Key,
                              AverageRating = grp.Average(ed => ed.Rating)
                          };

            var orderedList = ratings.OrderByDescending(rt => rt.AverageRating).ToList();

            orderedList.ForEach(r => result.Add(new MovieRating() { Id = Guid.NewGuid(), MovieId = r.MovieId, Rating = r.AverageRating }));

            return result;
        }

        public void setRating(Guid userId, Guid movieId, int rating) {
            if (this.movieRatings.Where(mvr => mvr.UserId == userId && mvr.MovieId == movieId).Count() == 0) {
                this.AddMovieRating(new MovieRating() { Id = Guid.NewGuid(), UserId = userId, MovieId = movieId, Rating = rating });
            }
            else {
                MovieRating movieRating = this.movieRatings.Where(mvr => mvr.UserId == userId && mvr.MovieId == movieId).First();
                movieRating.Rating = rating;
                this.UpdateMovieRating(movieRating);
            }
        }

        public IEnumerable<MovieRating> GetAllRatings() {
            throw new NotImplementedException();
        }
    }
}