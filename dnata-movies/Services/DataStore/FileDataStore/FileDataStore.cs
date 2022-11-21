using DnataMovies.Controllers;
using Models.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Services.DataStore.FileDataStore
{
    public class FileDataStore : IDataStore
    {
        IWebHostEnvironment hostEnvironment;
        ILogger<MoviesServiceController> logger;

        private MovieRepository movieRepository;
        private UserRepository userRepository;
        private RatingRepository ratingRepository;
        private GenreRepository genreRepository;

        public FileDataStore(IWebHostEnvironment hostEnvironment, ILogger<MoviesServiceController> logger) {

            this.hostEnvironment = hostEnvironment;
            this.logger = logger;

            this.movieRepository = new MovieRepository();
            this.userRepository = new UserRepository();
            this.ratingRepository = new RatingRepository();
            this.genreRepository = new GenreRepository(); 
        }

        public void Load() {

            List<Genre> genres = new List<Genre>();
            List<Movie> movies = new List<Movie>();
            List<User> users = new List<User>();
            List<MovieRating> ratings = new List<MovieRating>();

            try {
                using (StreamReader file = File.OpenText(hostEnvironment.ContentRootPath + @"\Resources\Genres.json")) {
                    genres = JsonConvert.DeserializeObject<IEnumerable<Genre>>(file.ReadToEnd()).ToList();
                    genres.ForEach(g => this.genreRepository.Add(g));
                }

                using (TextReader file = File.OpenText(hostEnvironment.ContentRootPath + @"\Resources\Movies.json")) {

                    String fileContents = file.ReadToEnd();
                    JArray result = JArray.Parse(fileContents);

                    foreach (var arrayItem in result.Children()) {

                        Movie movieItem = new Movie();
                        foreach (JProperty childItem in arrayItem.Children()) {
                            switch (childItem.Name) {
                                case "id":
                                    movieItem.Id = Guid.Parse(childItem.Value.ToString());
                                    break;
                                case "title":
                                    movieItem.Title = childItem.Value.ToString();
                                    break;
                                case "releaseYear":
                                    movieItem.YearOfRelease = Convert.ToInt16(childItem.Value);
                                    break;
                                case "runningTime":
                                    movieItem.RunningTime = Convert.ToInt16(childItem.Value);
                                    break;
                                case "genres":
                                    List<Genre> list = new List<Genre>();

                                    foreach (var genreItem in childItem.Value) {
                                        list.Add(genres.Where(g => g.Id == Guid.Parse(genreItem.ToString())).First());
                                    }

                                    movieItem.Genres = list;
                                    break;
                            }
                        }
                        movies.Add(movieItem);
                    }
                    movies.ForEach(mv => this.movieRepository.Add(mv));
                }

                using (StreamReader file = File.OpenText(hostEnvironment.ContentRootPath + @"\Resources\Users.json")) {
                    users = JsonConvert.DeserializeObject<IEnumerable<User>>(file.ReadToEnd()).ToList();
                    users.ForEach(u => this.userRepository.Add(u));
                }

                using (StreamReader file = File.OpenText(hostEnvironment.ContentRootPath + @"\Resources\Ratings.json")) {
                    ratings = JsonConvert.DeserializeObject<IEnumerable<MovieRating>>(file.ReadToEnd()).ToList();
                    ratings.ForEach(r => this.ratingRepository.Add(r));
                }
            }
            catch (FileNotFoundException fnfEx) {
                logger.LogError(String.Format("File could not be found when loading data: {0}", fnfEx.Message));
            }
            catch (Exception ex) {
                logger.LogError(String.Format("Unknown exception encountered when loading data: {0}", ex.Message));
            }
        }

        public void AddMovie(Movie movie) {
            this.movieRepository.Add(movie);
        }

        public void AddUser(User user) {
            this.userRepository.Add(user);
        }

        public void DeleteMovie(Movie movie) {
            this.movieRepository.Delete(movie);
        }

        public void DeleteUser(User user) {
            this.userRepository.Delete(user);
        }

        public IEnumerable<Movie> GetAllMovies() {
            return this.movieRepository.GetAllMovies();
        }

        public IEnumerable<MovieRating> GetAllRatings() {
            return this.ratingRepository.GetAll();
        }

        public Movie GetMovie(Guid id) {
            return this.movieRepository.Get(id);
        }

        public User GetUser(Guid id) {
            return this.userRepository.Get(id);
        }

        public void UpdateMovie(Movie movie) {
            this.movieRepository.Update(movie);
        }

        public void UpdateUser(User user) {
            this.userRepository.Update(user);
        }

        public void setRating(Guid userId, Guid movieId, int rating) {
            this.ratingRepository.setRating(userId, movieId, rating);
        }
    }
}