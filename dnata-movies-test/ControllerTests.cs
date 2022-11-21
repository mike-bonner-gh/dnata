using DnataMovies.Controllers;
using DnataMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Data;
using Moq;
using Services.DataStore;

namespace dnata_movies_test
{
    public class ControllerTests
    {
        private ILogger<MoviesServiceController> logger;
        private MoviesServiceController moviesServiceController;
        private IDataStore dataStore;

        [SetUp]
        public void Setup() {
            logger = Mock.Of<ILogger<MoviesServiceController>>();
            dataStore = new MockDataStore();
            dataStore.Load();

            moviesServiceController = new MoviesServiceController(logger, dataStore);
        }

        [Test]
        public void TestSearchWithoutCriteriaReturnsHttp400() {

            var result = moviesServiceController.SearchMovies(new MoviesQuery());

            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
            Assert.Pass();
        }

        [Test]
        public void TestSearchWithNoResults() {

            var result = moviesServiceController.SearchMovies(new MoviesQuery() { YearOfRelease = 2001 });

            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            Assert.Pass();
        }

        [Test]
        public void TestSearchByYearOfRelease() {

            Movie movie = new Movie() { Id = Guid.NewGuid(), RunningTime = 102, Title = "Movie 1", YearOfRelease = 1992 };
            Movie movie2 = new Movie() { Id = Guid.NewGuid(), RunningTime = 105, Title = "Movie 2", YearOfRelease = 2002 };
            Movie movie3 = new Movie() { Id = Guid.NewGuid(), RunningTime = 90, Title = "Movie 4", YearOfRelease = 2010 };
            Movie movie4 = new Movie() { Id = Guid.NewGuid(), RunningTime = 74, Title = "Unique Film", YearOfRelease = 2019 };

            dataStore.AddMovie(movie);
            dataStore.AddMovie(movie2);
            dataStore.AddMovie(movie3);
            dataStore.AddMovie(movie4);

            var result = moviesServiceController.SearchMovies(new MoviesQuery() { YearOfRelease = 2002 });

            Assert.IsInstanceOf<IEnumerable<Movie>>(result.Value);
            Assert.IsTrue(result?.Value?.Count() == 1);
            Assert.Pass();
        }

        [Test]
        public void TestSearchByTitleAndYearOfRelease() {

            Movie movie = new Movie() { Id = Guid.NewGuid(), RunningTime = 102, Title = "Movie 1", YearOfRelease = 1992 };
            Movie movie2 = new Movie() { Id = Guid.NewGuid(), RunningTime = 105, Title = "Movie 2", YearOfRelease = 2002 };
            Movie movie3 = new Movie() { Id = Guid.NewGuid(), RunningTime = 90, Title = "Movie 4", YearOfRelease = 2010 };
            Movie movie4 = new Movie() { Id = Guid.NewGuid(), RunningTime = 74, Title = "Unique Film", YearOfRelease = 2019 };

            dataStore.AddMovie(movie);
            dataStore.AddMovie(movie2);
            dataStore.AddMovie(movie3);
            dataStore.AddMovie(movie4);

            var result = moviesServiceController.SearchMovies(new MoviesQuery() { Title = "Movie 1", YearOfRelease = 1992 });

            Assert.IsInstanceOf<IEnumerable<Movie>>(result.Value);
            Assert.IsTrue(result?.Value?.Count() == 1);

            Assert.Pass();
        }

        [Test]
        public void TestSearchByPartialTitle() {

            Movie movie = new Movie() { Id = Guid.NewGuid(), RunningTime = 102, Title = "Movie 1", YearOfRelease = 1992 };
            Movie movie2 = new Movie() { Id = Guid.NewGuid(), RunningTime = 105, Title = "Movie 2", YearOfRelease = 2002 };
            Movie movie3 = new Movie() { Id = Guid.NewGuid(), RunningTime = 90, Title = "Movie 4", YearOfRelease = 2010 };
            Movie movie4 = new Movie() { Id = Guid.NewGuid(), RunningTime = 74, Title = "Unique Film", YearOfRelease = 2019 };

            dataStore.AddMovie(movie);
            dataStore.AddMovie(movie2);
            dataStore.AddMovie(movie3);
            dataStore.AddMovie(movie4);

            var result = moviesServiceController.SearchMovies(new MoviesQuery() { Title = "unique" });

            Assert.IsInstanceOf<IEnumerable<Movie>>(result.Value);
            Assert.IsTrue(result?.Value?.Count() == 1);

            Assert.Pass();
        }

        [Test]
        public void TestSearchByGenre() {

            Genre actionGenre = new Genre(Guid.NewGuid(), "Action");
            Genre horrorGenre = new Genre(Guid.NewGuid(), "Horror");

            Movie movie = new Movie() {
                Id = Guid.NewGuid(),
                RunningTime = 102,
                Title = "Movie 1",
                YearOfRelease = 1992,
                Genres = new List<Genre>() { actionGenre, horrorGenre }
            };

            Movie movie2 = new Movie() {
                Id = Guid.NewGuid(),
                RunningTime = 105,
                Title = "Movie 2",
                YearOfRelease = 2002,
                Genres = new List<Genre>() { actionGenre }
            };

            dataStore.AddMovie(movie);
            dataStore.AddMovie(movie2);

            var result = moviesServiceController.SearchMovies(new MoviesQuery() { Genres = new List<String>() { "action" } });

            Assert.IsInstanceOf<IEnumerable<Movie>>(result.Value);
            Assert.IsTrue(result?.Value?.Count() == 2);
            Assert.Pass();
        }

        [Test]
        public void TestSearchByGenres() {

            Genre actionGenre = new Genre(Guid.NewGuid(), "Action");
            Genre horrorGenre = new Genre(Guid.NewGuid(), "Horror");
            Genre suspenseGenre = new Genre(Guid.NewGuid(), "Suspense");

            Movie movie = new Movie() {
                Id = Guid.NewGuid(),
                RunningTime = 102,
                Title = "Movie 1",
                YearOfRelease = 1992,
                Genres = new List<Genre>() { actionGenre, horrorGenre }
            };

            Movie movie2 = new Movie() {
                Id = Guid.NewGuid(),
                RunningTime = 105,
                Title = "Movie 2",
                YearOfRelease = 2002,
                Genres = new List<Genre>() { actionGenre }
            };

            Movie movie3 = new Movie() {
                Id = Guid.NewGuid(),
                RunningTime = 91,
                Title = "Movie 3",
                YearOfRelease = 2016,
                Genres = new List<Genre>() { suspenseGenre }
            };

            dataStore.AddMovie(movie);
            dataStore.AddMovie(movie2);
            dataStore.AddMovie(movie3);

            var result = moviesServiceController.SearchMovies(new MoviesQuery() { Genres = new List<String>() { "Action", "Horror" } });

            Assert.IsInstanceOf<IEnumerable<Movie>>(result.Value);
            Assert.IsTrue(result?.Value?.Count() == 2);
            Assert.Pass();
        }

        [Test]
        public void TestSetRatingOnInvalidMovie() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);

            dataStore.AddMovie(movie1);
            dataStore.AddUser(user1);

            var result = moviesServiceController.SetUserRating(user1.Id, Guid.NewGuid(), 3);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.Pass();
        }

        [Test]
        public void TestSetRatingOnInvalidUser() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);

            dataStore.AddMovie(movie1);
            dataStore.AddUser(user1);

            var result = moviesServiceController.SetUserRating(Guid.NewGuid(), movie1.Id, 3);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            Assert.Pass();
        }

        [Test]
        public void TestSetOutOfLowerBoundRating() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);

            dataStore.AddMovie(movie1);
            dataStore.AddUser(user1);

            var result = moviesServiceController.SetUserRating(user1.Id, movie1.Id, 0);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.Pass();
        }

        [Test]
        public void TestSetOutOfUpperBoundRating() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);

            dataStore.AddMovie(movie1);
            dataStore.AddUser(user1);

            var result = moviesServiceController.SetUserRating(user1.Id, movie1.Id, 6);

            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.Pass();
        }

        [Test]
        public void TestSetOfValidRating() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);

            dataStore.AddMovie(movie1);
            dataStore.AddUser(user1);

            var result = moviesServiceController.SetUserRating(user1.Id, movie1.Id, 4);

            Assert.IsInstanceOf<OkResult>(result);
            Assert.Pass();
        }

        [Test]
        public void TestGetRatingsForInvalidUser() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);

            dataStore.AddMovie(movie1);
            dataStore.AddUser(user1);

            var result = moviesServiceController.GetTop5MoviesByUser(Guid.NewGuid());

            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            Assert.Pass();
        }

        [Test]
        public void TestGetRatingsWithNoMovies() {

            User user1 = new User(Guid.NewGuid(), "User 1");

            dataStore.AddUser(user1);

            var result = moviesServiceController.GetTop5Movies();

            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            Assert.Pass();
        }

        [Test]
        public void TestGetRatingsWithNoMoviesByUser() {

            User user1 = new User(Guid.NewGuid(), "User 1");

            dataStore.AddUser(user1);

            var result = moviesServiceController.GetTop5MoviesByUser(user1.Id);

            Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
            Assert.Pass();
        }

        [Test]
        public void TestOverallRatingOrder() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            User user2 = new User(Guid.NewGuid(), "User 2");
            User user3 = new User(Guid.NewGuid(), "User 3");

            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);
            Movie movie2 = new Movie(Guid.NewGuid(), "Movie 2", 1996, 60);
            Movie movie3 = new Movie(Guid.NewGuid(), "Movie 3", 1999, 74);
            Movie movie4 = new Movie(Guid.NewGuid(), "Movie 4", 2001, 76);
            Movie movie5 = new Movie(Guid.NewGuid(), "Movie 5", 2020, 90);
            
            dataStore.AddMovie(movie1);
            dataStore.AddMovie(movie2);
            dataStore.AddMovie(movie3);
            dataStore.AddMovie(movie4);
            dataStore.AddMovie(movie5);

            dataStore.AddUser(user1);
            dataStore.AddUser(user2);
            dataStore.AddUser(user3);

            dataStore.setRating(user1.Id, movie1.Id, 3);
            dataStore.setRating(user1.Id, movie2.Id, 5);
            dataStore.setRating(user1.Id, movie3.Id, 4);

            dataStore.setRating(user2.Id, movie1.Id, 4);
            dataStore.setRating(user2.Id, movie2.Id, 4);
            
            //Movie 1 should be 4.05
            //Movie 2 should be 4.8

            var result = moviesServiceController.GetTop5Movies();

            Assert.IsInstanceOf<IEnumerable<Movie>>(result.Value);
            Assert.IsTrue(result?.Value?.First().Id == movie2.Id);
            Assert.Pass();
        }

        [Test]
        public void TestRatingRoundingDown() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            User user2 = new User(Guid.NewGuid(), "User 2");
            User user3 = new User(Guid.NewGuid(), "User 3");

            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);

            dataStore.AddMovie(movie1);
            dataStore.AddUser(user1);

            dataStore.setRating(user1.Id, movie1.Id, 3);
            dataStore.setRating(user2.Id, movie1.Id, 4);
            dataStore.setRating(user3.Id, movie1.Id, 3);

            var result = moviesServiceController.GetTop5Movies();

            Assert.IsInstanceOf<IEnumerable<Movie>>(result.Value);
            Assert.IsTrue(result?.Value?.First().Id == movie1.Id);
            Assert.IsTrue(result?.Value?.First().Rating == 3.5);
            Assert.Pass();
        }

        [Test]
        public void TestRatingRoundingUp() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            User user2 = new User(Guid.NewGuid(), "User 2");
            User user3 = new User(Guid.NewGuid(), "User 3");
            User user4 = new User(Guid.NewGuid(), "User 4");
            User user5 = new User(Guid.NewGuid(), "User 5");

            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);

            dataStore.AddMovie(movie1);
            dataStore.AddUser(user1);

            dataStore.setRating(user1.Id, movie1.Id, 3);
            dataStore.setRating(user2.Id, movie1.Id, 4);
            dataStore.setRating(user3.Id, movie1.Id, 3);
            dataStore.setRating(user4.Id, movie1.Id, 2);
            dataStore.setRating(user5.Id, movie1.Id, 2);

            var result = moviesServiceController.GetTop5Movies();

            Assert.IsInstanceOf<IEnumerable<Movie>>(result.Value);
            Assert.IsTrue(result?.Value?.First().Id == movie1.Id);
            Assert.IsTrue(result?.Value?.First().Rating == 3.0);
            Assert.Pass();
        }

        [Test]
        public void TestRatingOrderByUser() {

            User user1 = new User(Guid.NewGuid(), "User 1");
            User user2 = new User(Guid.NewGuid(), "User 2");
            User user3 = new User(Guid.NewGuid(), "User 3");

            Movie movie1 = new Movie(Guid.NewGuid(), "Movie 1", 1992, 90);
            Movie movie2 = new Movie(Guid.NewGuid(), "Movie 2", 1996, 60);
            Movie movie3 = new Movie(Guid.NewGuid(), "Movie 3", 1999, 74);
            Movie movie4 = new Movie(Guid.NewGuid(), "Movie 4", 2001, 76);
            Movie movie5 = new Movie(Guid.NewGuid(), "Movie 5", 2020, 90);

            dataStore.AddMovie(movie1);
            dataStore.AddMovie(movie2);
            dataStore.AddMovie(movie3);
            dataStore.AddMovie(movie4);
            dataStore.AddMovie(movie5);

            dataStore.AddUser(user1);
            dataStore.AddUser(user2);
            dataStore.AddUser(user3);

            dataStore.setRating(user1.Id, movie1.Id, 3);
            dataStore.setRating(user1.Id, movie2.Id, 5);
            dataStore.setRating(user1.Id, movie3.Id, 4);

            dataStore.setRating(user2.Id, movie1.Id, 4);
            dataStore.setRating(user2.Id, movie2.Id, 4);


            var result = moviesServiceController.GetTop5MoviesByUser(user1.Id);

            Assert.IsInstanceOf<IEnumerable<Movie>>(result.Value);
            Assert.IsTrue(result?.Value?.First().Id == movie2.Id);
            Assert.Pass();
        }
    }
}