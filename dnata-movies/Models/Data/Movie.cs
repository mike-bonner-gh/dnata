using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Models.Data
{
    public class Movie
    {
        private double rating;

        [JsonProperty("id")]
        public Guid Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("releaseYear")]
        public int YearOfRelease { get; set; }

        [JsonProperty("runningTime")]
        public int RunningTime { get; set; }

        [JsonProperty("genres")]
        [System.Text.Json.Serialization.JsonIgnore]
        public IEnumerable<Genre> Genres { get; set; }

        [JsonProperty("averageRating")]
        public double Rating {
            get {
                return rating;
            }
        }

        public Movie() {
            Genres = new List<Genre>();
            Title = string.Empty;
        }

        public Movie(Guid id, String title, int yearOfRelease, int runningTime) {
            this.Id = id;
            this.Title = title;
            this.YearOfRelease = yearOfRelease;
            this.RunningTime = runningTime;
            Genres = new List<Genre>();
        }

        internal void setRating(double rating) {
            this.rating = rating;
        }

        internal bool ContainsGenre(String genre) {
            foreach (var item in Genres) {
                if (item.Title.ToLower() == genre) {
                    return true;
                }
            }

            return false;
        }

        internal bool ContainsGenres(List<String> genres) {
            foreach (var item in Genres) {
                if (String.Join(",", genres).ToLower().Contains(item.Title.ToLower())) { 
                    return true;
                }
            }

            return false;
        }
    }
}