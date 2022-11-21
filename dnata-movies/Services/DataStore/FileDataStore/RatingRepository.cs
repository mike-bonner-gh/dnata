using Models.Data;

namespace Services.DataStore.FileDataStore
{
    public class RatingRepository : IRatingRepository<MovieRating>
    {
        private List<MovieRating> movieRatings;

        public RatingRepository() {
            this.movieRatings = new List<MovieRating>();

        }
        public void Add(MovieRating rating) {
            this.movieRatings.Add(rating);
        }

        public void Delete(MovieRating rating) {
            this.movieRatings.RemoveAll(r => r.Id == rating.Id);
        }

        public MovieRating Get(Guid id) {
            if (this.movieRatings.Where(r => r.Id == id).Count() == 1) {
                return this.movieRatings.Where(r => r.Id == id).First();
            }
            else {
                return null;
            }
        }

        public IEnumerable<MovieRating> GetAll() {
            return this.movieRatings;
        }

        public void Update(MovieRating rating) {
            if (this.movieRatings.Where(r => r.Id == rating.Id).Count() == 0) {
                this.movieRatings.Add(rating);

            }
            else {
                this.movieRatings.RemoveAll(r => r.Id == r.Id);
                this.movieRatings.Add(rating);
            }
        }

        public void setRating(Guid userId, Guid movieId, int rating) {
            if (this.movieRatings.Where(mvr => mvr.UserId == userId && mvr.MovieId == movieId).Count() == 0) {
                this.Add(new MovieRating() { Id = Guid.NewGuid(), UserId = userId, MovieId = movieId, Rating = rating });
            }
            else {
                MovieRating movieRating = this.movieRatings.Where(mvr => mvr.UserId == userId && mvr.MovieId == movieId).First();
                movieRating.Rating = rating;
                this.Update(movieRating);
            }
        }
    }
}