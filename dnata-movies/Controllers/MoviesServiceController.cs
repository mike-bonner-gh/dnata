using DnataMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Models.Data;
using Services.DataStore;

namespace DnataMovies.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MoviesServiceController : Controller
    {
        private readonly ILogger<MoviesServiceController> logger;
        private readonly IDataStore dataStore;

        public MoviesServiceController(ILogger<MoviesServiceController> logger, IDataStore dataStore) {

            this.logger = logger;
            this.dataStore = dataStore;

            dataStore.Load();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Movie>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/movies/search")]
        public ActionResult<IEnumerable<Movie>> SearchMovies(MoviesQuery moviesQuery) {

            IEnumerable<Movie> searchItems;

            if (String.IsNullOrWhiteSpace(moviesQuery.Title) && !moviesQuery.YearOfRelease.HasValue && (moviesQuery.Genres == null || moviesQuery.Genres.Count() == 0)) {
                return BadRequest("At least one search criteria must be supplied");
            }

            var items = dataStore.GetAllMovies();
            var genreItems = new List<Movie>();

            if (!String.IsNullOrEmpty(moviesQuery.Title)) {
                items = items.Where(mv => mv.Title.ToLower().Contains(moviesQuery.Title.ToLower()));
            }

            if (moviesQuery.YearOfRelease.HasValue) {
                items = items.Where(mv => mv.YearOfRelease == moviesQuery.YearOfRelease);
            }

            if (moviesQuery.Genres != null && moviesQuery.Genres.Count() > 0) {

                var itemListForGenres = dataStore.GetAllMovies();

                foreach (var searchItem in itemListForGenres) {
                    if (searchItem.ContainsGenres(moviesQuery.Genres)) {
                        genreItems.Add(searchItem);
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(moviesQuery.Title) || moviesQuery.YearOfRelease.HasValue) {
                searchItems = items.ToList();
            }
            else {
                searchItems = new List<Movie>();
            }

            var result = searchItems.Concat(genreItems).Distinct();

            if (result.Count() == 0) {
                return NotFound("No results have been found for your query");
            }
            else {
                return new ActionResult<IEnumerable<Movie>>(result.ToList());
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Movie>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/movies/top5")]
        public ActionResult<IEnumerable<Movie>> GetTop5Movies() {

            List<MovieRating> result = new List<MovieRating>();
            List<Movie> movies = dataStore.GetAllMovies().ToList();

            var ratings = from r in dataStore.GetAllRatings()
                          group r by r.MovieId into grp
                          select new {
                              MovieId = grp.Key,
                              AverageRating = grp.Average(ed => ed.Rating)
                          };

            var orderedList = ratings.OrderByDescending(rt => rt.AverageRating).ToList();

            orderedList.ForEach(r => result.Add(new MovieRating() { Id = Guid.NewGuid(), MovieId = r.MovieId, Rating = r.AverageRating }));

            var setToRetrive = new HashSet<Guid>(ratings.Select(rbu => rbu.MovieId));
            IEnumerable<Movie> result2 = MergeAndSortRatingsAndMovies(movies, result, setToRetrive);

            if (result.Count() == 0) {
                return NotFound("No movies could be found");
            }
            else {
                return result2.ToList();
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Movie>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/user/{userId}/top5")]
        public ActionResult<IEnumerable<Movie>> GetTop5MoviesByUser(Guid userId) {

            if (dataStore.GetUser(userId) == null) {
                return NotFound(String.Format("User {0} could not be found", userId));
            }

            List<MovieRating> ratingsByUser = dataStore.GetAllRatings().Where(mr => mr.UserId == userId).OrderByDescending(mr => mr.Rating).Take(5).ToList();
            List<Movie> movies = dataStore.GetAllMovies().ToList();

            var setToRetrive = new HashSet<Guid>(ratingsByUser.Select(rbu => rbu.MovieId));
            IEnumerable<Movie> result = MergeAndSortRatingsAndMovies(movies, ratingsByUser, setToRetrive).ToList();

            if (result.Count() == 0) {
                return NotFound("No movies could be found");
            }
            else {
                return result.ToList();
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("/user/{userId}/{movieId}/{rating}")]
        public ActionResult SetUserRating(Guid userId, Guid movieId, int rating) {

            if (dataStore.GetMovie(movieId) == null) {
                return NotFound(String.Format("Movie {0} could not be found", movieId));
            } else if (dataStore.GetUser(userId) == null) {
                return NotFound(String.Format("User {0} could not be found", userId));
            } else if (rating < 1 || rating > 5) {
                return BadRequest("Rating must be an integer between 1 and 5");
            } else {
                dataStore.setRating(userId, movieId, rating);
                return new OkResult();
            }
        }
        private IEnumerable<Movie> MergeAndSortRatingsAndMovies(List<Movie> movies, List<MovieRating> ratings, HashSet<Guid> candidates) {

            List<Movie> mergedList = movies.Where(mv => candidates.Contains(mv.Id)).ToList();
            mergedList.ForEach(tp => tp.setRating(ratings.Where(rbu => rbu.MovieId == tp.Id).Select(rbu => rbu.Rating).First()));

            mergedList.ForEach(ml => ml.setRating(RoundRating(ml.Rating)));
            return mergedList.OrderByDescending(ml => ml.Rating).ThenBy(ml => ml.Title).ToList();
        }

        private double RoundRating(double rating) {
            return Math.Round(Convert.ToDouble(rating) * 2, MidpointRounding.AwayFromZero) / 2;
        }
    }
}